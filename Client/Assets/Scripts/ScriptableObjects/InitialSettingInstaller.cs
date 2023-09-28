using UnityEngine;
using Zenject;

namespace Core.Framework
{
    [CreateAssetMenu(fileName = "InitialSetting", menuName = "Configs/InitialConfig", order = 1)]
    public class InitialSettingInstaller: ScriptableObjectInstaller<InitialSettingInstaller>
    {
        public GameStore.Atlas Atlas;

        public override void InstallBindings()
        {
            Container.BindInstance(Atlas);
        }
    }
}
