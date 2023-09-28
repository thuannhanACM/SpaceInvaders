using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;
using static Core.Framework.Services.DownloadDependency;

namespace Core.Framework.Services
{
    public class AddressableLoader : IBundleLoader
    {
        // CONVERSION.
        public const float ONE_GIGABYTE_TO_MEGABYTE = 1024.0f;
        public const float ONE_MEGABYTE_TO_BYTE = 1048576.0f;
        public const float ONE_MEGABYTE_TO_KILOBYTE = 1024.0f;

        private readonly CheckDownloadSize _check;
        private readonly DownloadDependency _download;
        private readonly SignalBus _signalBus;

        private Dictionary<string, AsyncOperationHandle> _loadedOperations =
            new Dictionary<string, AsyncOperationHandle>();

        private bool IsInitialized = false;

        public AddressableLoader(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _check = new CheckDownloadSize(signalBus);
            _download = new DownloadDependency(signalBus);

            UseNewCachePath();
            InitializeAndCheckCatalogUpdates();
        }

        private void UseNewCachePath()
        {
            string fullAppDataPath = Application.persistentDataPath + "/AssetBundles";
            if(!Directory.Exists(fullAppDataPath))
                Directory.CreateDirectory(fullAppDataPath);

            Cache cache = Caching.GetCacheByPath(fullAppDataPath);
            if (cache == null || !cache.valid)
                cache = Caching.AddCache(fullAppDataPath);

            if(cache.valid)
                Caching.currentCacheForWriting = cache;
        }

        private void InitializeAndCheckCatalogUpdates()
        {
            Addressables.InitializeAsync().Completed += op =>
            {
                CheckForCatalogUpdates();
                IsInitialized = true;
            };
        }

        private void CheckForCatalogUpdates()
        {
            if (IsInitialized)
                return;

            List<string> catalogsToUpdate = new List<string>();
            Addressables.CheckForCatalogUpdates(true).Completed += op =>
            {
                catalogsToUpdate.AddRange(op.Result);
                UpdateCatalogs(catalogsToUpdate);
            };
        }

        private void UpdateCatalogs(List<string> catalogsToUpdate)
        {
            if (catalogsToUpdate.Count > 0)
                Addressables.UpdateCatalogs(catalogsToUpdate, true);
        }

        public void CheckPreloadAssets(List<string> keys)
        {
            _check.CheckMultipleKeys(keys).Forget();
        }

        public void DownloadPreloadAssets(List<string> keys)
        {
            _download.DownloadAssets(keys, Addressables.MergeMode.Union);
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string path)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                throw new AddressableRunOnEditorMode(path);
#endif
            try
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(path);

                while (!handle.IsDone)
                    await UniTask.NextFrame();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                    return handle.Result;
                else if (handle.Status == AsyncOperationStatus.Failed 
                    ||handle.Status == AsyncOperationStatus.None)
                    throw new AddressableCannotInstantiateAsset($"Path: {path} | status: {handle.Status}");

                return null;
            }
            catch
            {
                throw new MissingAddressableAssetAtPath(path);
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                throw new AddressableRunOnEditorMode(path);
#endif
            try
            {
                AssetReference a = new AssetReference(path);
                AsyncOperationHandle<T> handle = a.LoadAssetAsync<T>();

                while (!handle.IsDone)
                    await UniTask.NextFrame();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    CacheLoadedOperation(path, handle);
                    return handle.Result;
                }
                else if ( handle.Status == AsyncOperationStatus.Failed 
                    || handle.Status == AsyncOperationStatus.None)
                    throw new AddressableCannotLoadAsset($"Path: {path} | status: {handle.Status}");

                return null;
            }
            catch
            {
                throw new MissingAddressableAssetAtPath(path);
            }
        }

        private void CacheLoadedOperation<T>(string path, AsyncOperationHandle<T> handle) where T : Object
        {
            if (!_loadedOperations.ContainsKey(path))
                _loadedOperations.Add(path, handle);
            else
                _loadedOperations[path] = handle;
        }

        public void ReleaseAsset(string path)
        {
            if (_loadedOperations.ContainsKey(path))
            {
                if (_loadedOperations[path].IsValid())
                    Addressables.Release(_loadedOperations[path]);
            }
        }

        public void ReleaseAsset(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        public void ReleaseInstance(GameObject instanceByAddressable)
        {
            if (instanceByAddressable != null)
                Addressables.ReleaseInstance(instanceByAddressable);
        }

        public async UniTask<SceneInstance> LoadSceneAndActiveAsync(string sceneName, LoadSceneMode mode)
        {
            DownloadInfo status = new DownloadInfo();
            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, mode, activateOnLoad: true);

                while (!handle.IsDone)
                {
                    status.Progress = handle.PercentComplete;
                    _signalBus.Fire(new LoadingProgressSignal(status));

                    await UniTask.NextFrame();
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                    return handle.Result;
                else if (handle.Status == AsyncOperationStatus.Failed
                    || handle.Status == AsyncOperationStatus.None)
                    throw new AddressableCannotLoadSceneAndActive($"Scene: {sceneName} | Status: {handle.Status}");

                return default;
            }
            catch
            {
                throw new AddressableLoadMissingScene(sceneName);
            }
        }

        public async UniTask UnLoadScene(SceneInstance scene)
        {
            try
            {
                await Addressables.UnloadSceneAsync(scene, true);
            }
            catch (System.Exception error)
            {
                throw error;
            }
        }

        private class MissingAddressableAssetAtPath : System.Exception
        {
            public MissingAddressableAssetAtPath(string message) : base(message) { }
        }

        private class AddressableRunOnEditorMode : System.Exception
        {
            public AddressableRunOnEditorMode(string message) : base(message) { }
        }
        private class AddressableLoadMissingScene : System.Exception
        {
            public AddressableLoadMissingScene(string message) : base(message) { }
        }
        private class AddressableCannotInstantiateAsset : System.Exception
        {
            public AddressableCannotInstantiateAsset(string message) : base(message) { }
        }
        private class AddressableCannotLoadAsset : System.Exception
        {
            public AddressableCannotLoadAsset(string message) : base(message) { }
        }

        private class AddressableCannotLoadSceneAndActive : System.Exception
        {
            public AddressableCannotLoadSceneAndActive(string message) : base(message) { }
        }

    }
}
