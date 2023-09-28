using Core.Business;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework.Utilities
{
    public class ConfigTableLoaderWithBundle : IConfigTableLoader
    {
        private readonly ILogger _logger;
        private readonly IBundleLoader _bundleLoader;

        public ConfigTableLoaderWithBundle(
            ILogger logger,
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader)
        {
            _bundleLoader = bundleLoader;
            _logger = logger;
        }

        public string GetFilename<TDefinition>(string fileId)
        {
            return ConfigFileName.GetFileName<TDefinition>(fileId);
        }

        public async UniTask<IConfigTable<TDefinition>> LoadDefinitions<TDefinition>(string filePath) where TDefinition : IGameDefinition
        {
            string text = (await _bundleLoader.LoadAssetAsync<TextAsset>(filePath)).text;
            return new ConfigTable<TDefinition>().LoadTable(text, filePath);
        }
    }
}
