using Zenject;

namespace Core.Framework
{
    public partial class Loading
    {
        public class Installer : Installer<Installer>
        {
            public override void InstallBindings()
            {
                Container.Bind<Loading>().AsTransient();
            }
        }
    }
}