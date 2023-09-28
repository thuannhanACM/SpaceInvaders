﻿using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.Framework.Services
{
    public class ResourceLoader : IBundleLoader
    {
        public void CheckPreloadAssets(List<string> keys)
        {
            throw new System.NotImplementedException();
        }

        public void DownloadPreloadAssets(List<string> keys)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string path)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                throw new ResourceRunOnEditorMode(path);
#endif
            try
            {
                GameObject pref = await LoadAssetAsync<GameObject>(path);
                GameObject result = Object.Instantiate(pref);
                return result;
            }
            catch
            {
                throw new MissinResourceAssetAtPath(path);
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                throw new ResourceRunOnEditorMode(path);
#endif
            try
            {
                var request = await Resources.LoadAsync<T>(path);
                return (request as T);
            }
            catch
            {
                throw new MissinResourceAssetAtPath(path);
            }
        }

        public void ReleaseAsset(string path)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseAsset(AsyncOperationHandle handle)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseInstance(GameObject instanceByAddressable)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<SceneInstance> LoadSceneAndActiveAsync(string sceneName, LoadSceneMode mode)
        {
            throw new System.NotImplementedException();
        }

        public UniTask UnLoadScene(SceneInstance scene)
        {
            throw new System.NotImplementedException();
        }

        private class MissinResourceAssetAtPath : System.Exception
        {
            public MissinResourceAssetAtPath(string message) : base(message) { }
        }

        private class ResourceRunOnEditorMode : System.Exception
        {
            public ResourceRunOnEditorMode(string message) : base(message) { }
        }
    }
}
