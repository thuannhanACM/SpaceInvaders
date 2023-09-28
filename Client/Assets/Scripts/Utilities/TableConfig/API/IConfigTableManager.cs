
using Core.Business;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Core.Framework.Utilities
{
    public interface IConfigTableManager
    {

        UniTask<TDefinition> GetDefinition<TDefinition>(string id, string fileId = "") where TDefinition : class, IGameDefinition;

        UniTask<TDefinition[]> GetDefinitions<TDefinition>(string[] ids, string fileId = "") where TDefinition : class, IGameDefinition;
        UniTask<TDefinition[]> GetDefinitionAll<TDefinition>(string fileId = "") where TDefinition : class, IGameDefinition;
    }
}
