using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Framework
{
    public abstract class BasePoolObj<T> : BasePoolObj, IPoolObject<T>
    {
        public abstract void InitDefinition(T def);
    }
    public abstract class BasePoolObj : IPoolObject
    {
        [Inject(Id = BundleLoaderName.Addressable)]
        protected readonly IBundleLoader _bundle;
        [Inject(Id = PoolName.Object)]
        protected readonly IPoolManager _poolManager;
        [Inject(Id = "PoolObjHolder")]
        public Transform PoolHolder { get; set; }
        
        public GameObject ModelObj { get { return _modelObj; } set { _modelObj = value; } }
        protected GameObject _modelObj { get; set; }
        public Transform transform { get { return _modelObj.transform; } }        

        public abstract UniTask Reinitialize();        
        public virtual void SelfDespawn()
        {
            _poolManager.Despawn(this);
        }

        public virtual void Destroy()
        {
            _bundle.ReleaseAsset(_modelObj.name);
            _bundle.ReleaseInstance(_modelObj);
            if (_modelObj != null)
                Object.Destroy(_modelObj);
        }

        public void BackToPool()
        {
            SetupPoolHolder();
        }
        public void SetupPoolHolder()
        {
#if UNITY_EDITOR
            _modelObj.transform.SetParent(PoolHolder);
#endif
        }

        public T GetComponentInChildren<T>()
        {
            return _modelObj.GetComponentInChildren<T>();
        }
    }
}
