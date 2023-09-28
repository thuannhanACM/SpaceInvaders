
using Cysharp.Threading.Tasks;

namespace Core.Framework
{
    public class AutoDespawnPoolObject : BasePoolObj<float>
    {
        float _despawnTime;
        public override void InitDefinition(float despawnTime)
        {
            _despawnTime = despawnTime;
        }

        public override UniTask Reinitialize()
        {
            return StartDespawn();
        }
        private async UniTask StartDespawn()
        {
            await UniTask.Delay((int)(_despawnTime * 1000));
            SelfDespawn();
        }
    }
}
