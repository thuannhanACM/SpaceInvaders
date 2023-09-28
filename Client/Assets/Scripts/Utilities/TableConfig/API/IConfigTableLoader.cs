

using Core.Business;
using Cysharp.Threading.Tasks;

namespace Core.Framework.Utilities
{
    public interface IConfigTableLoader
    {
        string GetFilename<TDefinition>(string fileId);
        UniTask<IConfigTable<TDefinition>> LoadDefinitions<TDefinition>(string fileId) where TDefinition : IGameDefinition;
    }
}
