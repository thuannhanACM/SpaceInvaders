using Core.Business;
using Core.Framework.Utilities;
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace Core.Framework
{
    public enum AudioMixerType
    {
        Music = 0,
        Sfx
    }

    public enum AudioActionType
    {
        START = 0,
        RESUME,
        PAUSE,
        STOP,
        RESTART,
    }

    public class AudioPoolManager : IPoolManager
    {
        private readonly DiContainer _container;
        private readonly IBundleLoader _bundleLoader;

        private Dictionary<string, IPoolObject> _channelGos = new Dictionary<string, IPoolObject>();

        public AudioPoolManager(
            [Inject(Id = BundleLoaderName.Addressable)] 
            IBundleLoader bundleLoader,
            DiContainer container)
        {
            _bundleLoader = bundleLoader;
            _container = container;
        }

        public bool IsPlayingBGAudio(string audioPath)
        {
            var activeMusicChannel = GetActiveChannels<AudioChannelPoolObject>(AudioMixerType.Music, Vector3.zero);
            return activeMusicChannel.Any(channel => channel.AudioPath == audioPath);
        }

        public async UniTask<T> GetChannel<T>(AudioMixerType type, Vector3 position, string prefPath, Transform objParent) where T : IPoolObject
        {
            var result = GetChannelOnPosition(type, position);
            if (result == null)
            {
                result = await GetUnactiveChannel<T>(type, position, prefPath, objParent);
            }
            else if (!result.ModelObj.activeSelf)
            {
                GetPoolObject<T>(result.ModelObj.name).Spawn();
                ReinitializeChannelPool<T>(result, position);
            }

            return (T)result;
        }

        public T[] GetActiveChannels<T>(AudioMixerType type, Vector3 position, string path = "") where T : IPoolObject
        {
            var result = GetActiveChannelsOnPosition(type, position, path);
            return result.Cast<T>().ToArray();
        }

        public async UniTask<T> GetUnactiveChannel<T>(AudioMixerType type, Vector3 position, string prefPath, Transform objParent) where T : IPoolObject
        {
            var result = GetUnactiveChannel(type);
            if (result == null)
                result = await SpawnNewChannelPool<T>(type, prefPath, objParent);
            else
                GetPoolObject<T>(result.ModelObj.name).Spawn();

            ReinitializeChannelPool<T>(result, position);
            return (T)result;
        }

        #region Get Channel Object
        private IPoolObject GetUnactiveChannel(AudioMixerType type)
        {
            var validChannelObj = GetChannelsOnType(type)
                .Where(poolObj => !poolObj.ModelObj.activeSelf).FirstOrDefault();
            return validChannelObj;
        }

        private IPoolObject[] GetActiveChannelsOnPosition(AudioMixerType type, Vector3 position, string path = "")
        {
            var validChannels = GetChannelsOnType(type)
                .Where(poolObj => poolObj.ModelObj.activeSelf)
                .Where(poolObj => CheckValidPosition(poolObj, position))
                .Where(poolObj => poolObj.ModelObj.name.Contains(path)).ToArray();
            return validChannels;
        }

        private IPoolObject GetChannelOnPosition(AudioMixerType type, Vector3 position)
        {
            var validChannelObj = GetChannelsOnType(type)
                .Where(poolObj => CheckValidPosition(poolObj, position)).FirstOrDefault();
            return validChannelObj;
        }

        private IPoolObject[] GetChannelsOnType(AudioMixerType type)
        {
            var objs = _channelGos.Values
                .Where(poolObj => poolObj != null && poolObj.ModelObj != null)
                .Where(poolObj => CheckAudioMixerType(poolObj, type)).ToArray();
            return objs;
        }
        #endregion Get Channel Object

        #region Channel Utilities
        private bool CheckValidPosition(IPoolObject poolObj, Vector3 position)
        {
            //var objPos = poolObj.ModelObj.transform.position;
            return true;
            //return position.x.IsBetweenRange(objPos.x - 3, objPos.x + 3) &&
            //    position.y.IsBetweenRange(objPos.y - 3, objPos.y + 3) &&
            //    position.y.IsBetweenRange(objPos.z - 3, objPos.z + 3);
        }

        private bool CheckAudioMixerType(IPoolObject poolObj, AudioMixerType type)
        {
            return (poolObj as AudioChannelPoolObject).AudioType == type;
        }

        private async UniTask<T> SpawnNewChannelPool<T>(AudioMixerType type, string prefPath, Transform objParent) where T : IPoolObject
        {
            string key = new StringBuilder("Audio-Channel-", 100).Append(_channelGos.Keys.Count).ToString();
            var result = CreateNewPool<T>(key).Spawn();
            _channelGos[key] = result;
            await CreateUGoChannelForObj(result, key, prefPath, type, objParent);
            return (T)result;
        }
        #endregion Channel Utilities

        #region Pool Default Handler
        private PoolObject GetPoolObject<T>(string key) where T : IPoolObject
        {
            return _container.ResolveId<PoolObject>(key);
        }

        private async void ReinitializeChannelPool<T>(IPoolObject result, Vector3 position) where T : IPoolObject
        {
            await result.Reinitialize();
            result.SetupPoolHolder();
            ((BasePoolObj)result).transform.position = new Vector3(position.x, position.y, position.z);
        }

        private async UniTask CreateUGoChannelForObj<T>(T result, string key, string prefPath, AudioMixerType type, Transform objParent) where T : IPoolObject
        {
            GameObject UGo = await InstantiateUGameObject(prefPath);
            UGo.transform.parent = objParent;
            AssignUGoToObj(UGo, result, key);
            (result as AudioChannelPoolObject)?.Initialize(type);
        }

        private T AssignUGoToObj<T>(GameObject UGo, T result, string prefPath) where T : IPoolObject
        {
            result.ModelObj = UGo;
            result.ModelObj.name = prefPath;
            return result;
        }

        private async UniTask<GameObject> InstantiateUGameObject(string prefPath)
        {
            GameObject go = await _bundleLoader.InstantiateAssetAsync(prefPath);
            return go;
        }

        private PoolObject CreateNewPool<T>(string key) where T : IPoolObject
        {
            _container.BindMemoryPool<IPoolObject, PoolObject>().WithId(key).WithInitialSize(10).ExpandByDoubling().To<T>().AsCached();
            return GetPoolObject<T>(key);
        }

        public void Despawn(IPoolObject obj)
        {
            PoolObject pool = _container.ResolveId<PoolObject>(obj.ModelObj.name);
            pool.Despawn(obj);
        }

        public UniTask<T> GetObject<T>(string prefPath) where T : IPoolObject
        {
            throw new System.NotImplementedException();
        }

        public void ClearPool(IPoolObject obj)
        {
            throw new System.NotImplementedException();
        }

        public void ClearPool(string prefabPath)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<T> GetObject<T, D>(string prefPath, D definition)
            where T : IPoolObject, IPoolObject<D>
        {
            throw new System.NotImplementedException();
        }
        #endregion Pool Default Handler
    }
}