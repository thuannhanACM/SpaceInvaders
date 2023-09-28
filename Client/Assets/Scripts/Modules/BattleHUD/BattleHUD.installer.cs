using Zenject;

namespace Core.Framework
{
    public partial class BattleHUD
    {
        public class Installer : Installer<Installer>
        {
            public override void InstallBindings()
            {
                Container.Bind<BattleHUD>().AsTransient();
            }
        }
    }
}
