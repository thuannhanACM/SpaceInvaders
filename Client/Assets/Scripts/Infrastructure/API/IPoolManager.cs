
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Infrastructure
{
    public interface IPoolManager
    {
        UniTask<T> GetObject<T>(string prefPath) where T : IPoolObject;
        UniTask<T> GetObject<T, D>(string prefPath, D definition)
            where T : IPoolObject, IPoolObject<D>;
        void Despawn(IPoolObject obj);
        void ClearPool(IPoolObject obj);
        void ClearPool(string prefabPath);
    }

    public interface IPoolObject
    {
        GameObject ModelObj { get; set; }
        UniTask Reinitialize();
        void Destroy();
        void BackToPool();
        void SetupPoolHolder();
    }
    public interface IPoolObject<T>
    {
        void InitDefinition(T def);
    }
}
