using Zenject;

namespace Core.Framework
{
    public partial class SplashScreen
    {
        public class Installer : Installer<Installer>
        {
            public override void InstallBindings()
            {
                Container.Bind<SplashScreen>().AsTransient();
            }
        }
    }
}