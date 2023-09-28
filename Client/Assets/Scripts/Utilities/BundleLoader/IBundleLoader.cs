using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.Framework
{
    public interface IBundleLoader
    {
        void CheckPreloadAssets(List<string> keys);
        void DownloadPreloadAssets(List<string> keys);

        UniTask<GameObject> InstantiateAssetAsync(string path);
        UniTask<T> LoadAssetAsync<T>(string path) where T : Object;

        void ReleaseAsset(string path);
        void ReleaseAsset(AsyncOperationHandle handle);
        void ReleaseInstance(GameObject instanceByAddressable);

        UniTask<SceneInstance> LoadSceneAndActiveAsync(string sceneName, LoadSceneMode mode);
        UniTask UnLoadScene(SceneInstance scene);
    }
}
