using Core.Business;
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Framework.Utilities
{
    public class ConfigTableManager : IConfigTableManager
    {
        private readonly Dictionary<string, IConfigTable> _typeToCollection = new Dictionary<string, IConfigTable>();
        private readonly ILogger _logger;
        private readonly IConfigTableLoader _configTableLoader;

        public ConfigTableManager(
            IConfigTableLoader definitionLoader,
            ILogger logger)
        {
            _configTableLoader = definitionLoader;
            _logger = logger;
        }

        public async UniTask<TDefinition> GetDefinition<TDefinition>(string id, string fileId = "") where TDefinition : class, IGameDefinition
        {
            IConfigTable<TDefinition> table = (IConfigTable<TDefinition>)(await LoadDefinitions<TDefinition>(fileId));
            return DeepClone(table.FindRecordById(id));
        }

        public async UniTask<TDefinition[]> GetDefinitions<TDefinition>(string[] ids, string fileId = "")
            where TDefinition : class, IGameDefinition
        {
            TDefinition[] result = new TDefinition[ids.Length];
            for (int i = 0; i < ids.Length; i++)
                result[i] = await GetDefinition<TDefinition>(ids[i], fileId);

            return result;
        }

        public async UniTask<TDefinition[]> GetDefinitionAll<TDefinition>(string fileId = "")
            where TDefinition : class, IGameDefinition
        {
            IConfigTable<TDefinition> table = (IConfigTable<TDefinition>)(await LoadDefinitions<TDefinition>(fileId));

            return table.All;
        }

        private async UniTask<IConfigTable> LoadDefinitions<TDefinition>(string fileId) where TDefinition : class, IGameDefinition
        {
            IConfigTable table;
            string filePath = _configTableLoader.GetFilename<TDefinition>(fileId);
            if (_typeToCollection.TryGetValue(filePath, out table))
                return await TryToGetAlreadyLoadedOrWaitForDefinition<TDefinition>(table, filePath);
            else
                return await LoadNewDefinitions<TDefinition>(fileId);
        }

        private async UniTask<IConfigTable> TryToGetAlreadyLoadedOrWaitForDefinition<TDefinition>(IConfigTable table, string filePath)
            where TDefinition : class, IGameDefinition
        {
            if (table != null)
                return table;
            else
                return await WaitToGetExistDefinition<TDefinition>(filePath);
        }

        private async UniTask<IConfigTable> LoadNewDefinitions<TDefinition>(string fileId) where TDefinition : class, IGameDefinition
        {
            IConfigTable table;
            string filePath = _configTableLoader.GetFilename<TDefinition>(fileId);
            _typeToCollection.Add(filePath, null);
            table = (IConfigTable)(await _configTableLoader.LoadDefinitions<TDefinition>(filePath));
            _typeToCollection[filePath] = table;

            return table;
        }

        private async UniTask<IConfigTable> WaitToGetExistDefinition<TDefinition>(string filePath)
        {
            int totalWaitms = 1000;
            int waitMs = 10;
            for (int i = 0; i < totalWaitms / waitMs; i++)
            {
                await UniTask.Delay(waitMs);
                if (_typeToCollection[filePath] != null)
                    return _typeToCollection[filePath];
            }

            throw new WaitingAlreadyLoadedConfigTableTimeOut(filePath);
        }

        private static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        private class WaitingAlreadyLoadedConfigTableTimeOut : Exception
        {
            public WaitingAlreadyLoadedConfigTableTimeOut(string message) : base(message)
            {
            }
        }
    }
}
