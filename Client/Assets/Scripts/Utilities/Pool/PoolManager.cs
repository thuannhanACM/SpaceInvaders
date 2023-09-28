using Core.Business;
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework.Utilities
{
    public class PoolManager : IPoolManager
    {
        private readonly DiContainer _container;
        private readonly IBundleLoader _bundleLoader;
        private readonly TickableManager _tickableManager;
        private readonly ILogger _logger;

        private HashSet<string> _poolDic = new HashSet<string>();

        public PoolManager(
            TickableManager tickableManager,
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader,
            DiContainer container,
            ILogger logger)
        {
            _tickableManager = tickableManager;
            _bundleLoader = bundleLoader;
            _container = container;
            _logger = logger;
        }
        public async UniTask<T> GetObject<T>(string prefPath) where T : IPoolObject
        {
            T result = await GetObjectInternal<T>(prefPath);
            result.SetupPoolHolder();
            await result.Reinitialize();
            result.ModelObj.SetActive(true);
            SubcribeITickable(result);
            return result;
        }

        private async UniTask<T> GetObjectInternal<T>(string prefPath) where T : IPoolObject
        {
            PoolObject pool = GetPoolObject<T>(prefPath);
            T result = (T)pool.Spawn();
            if (result.ModelObj == null)
                await CreateUGoForObj(result, prefPath);
            return result;
        }

        public async UniTask<T> GetObject<T,D>(string prefPath, D definition) 
            where T : IPoolObject, IPoolObject<D>
        {
            T result = await GetObjectInternal<T>(prefPath);
            result.SetupPoolHolder();
            result.InitDefinition(definition);
            await result.Reinitialize();
            result.ModelObj.SetActive(true);
            SubcribeITickable(result);
            return result;
        }

        private async UniTask CreateUGoForObj<T>(T result, string prefPath) where T : IPoolObject
        {
            GameObject UGo = await InstantiateUGameObject(prefPath);
            if (UGo == null)
                _logger.Error($"Cannot instantiate UGame Object for prefPath {prefPath}");
            AssignUGoToObj(UGo, result, prefPath);
        }

        private async UniTask<GameObject> InstantiateUGameObject(string prefPath)
        {
            GameObject go = await _bundleLoader.InstantiateAssetAsync(prefPath);
            return go;
        }

        private T AssignUGoToObj<T>(GameObject UGo, T result, string prefPath) where T : IPoolObject
        {
            result.ModelObj = UGo;
            result.ModelObj.name = prefPath;
            result.ModelObj.SetActive(false);
            
            return result;
        }

        private void SubcribeITickable<T>(T obj)
        {
            ITickable iTic = obj as ITickable;
            if (iTic != null)
                _tickableManager.Add(iTic);
        }

        public void Despawn(IPoolObject obj)
        {
            AsyncDespawn(obj).Forget();
        }

        private async UniTaskVoid AsyncDespawn(IPoolObject obj)
        {
            PoolObject pool = _container.ResolveId<PoolObject>(obj.ModelObj.name);
            UnSubcribeITickable(obj);
            await UniTask.WaitForEndOfFrame();
            pool.Despawn(obj);
        }

        private void UnSubcribeITickable<T>(T obj)
        {
            ITickable iTic = obj as ITickable;
            if (iTic != null)
                _tickableManager.Remove(iTic);
        }

        private PoolObject GetPoolObject<T>(string prefPath) where T : IPoolObject
        {
            if (_poolDic.Contains(prefPath))
                return _container.ResolveId<PoolObject>(prefPath);
            else
                return CreateNewPool<T>(prefPath);
        }

        private PoolObject CreateNewPool<T>(string prefPath) where T: IPoolObject
        {
            _poolDic.Add(prefPath);
            _container.BindMemoryPool<IPoolObject, PoolObject>().WithId(prefPath).WithInitialSize(10).ExpandByDoubling().To<T>().AsCached();

            return _container.ResolveId<PoolObject>(prefPath);
        }

        public void ClearPool(IPoolObject obj)
        {
            ClearPool(obj.ModelObj.name);
        }

        public void ClearPool(string prefabPath)
        {
            PoolObject pool = _container.ResolveId<PoolObject>(prefabPath);
            pool.Clear();
        }

    }
}
