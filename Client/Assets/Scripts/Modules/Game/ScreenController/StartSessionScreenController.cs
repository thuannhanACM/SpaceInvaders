using Core.Framework.Utilities;
using Zenject;

namespace Core.Framework
{
    public class StartSessionScreenController : IScreenController
    {
        private readonly IBundleLoader _bundleLoader;
        private readonly SignalBus _signalBus;
        private readonly AtlasShareManager _atlasShareManager;

        public ScreenName Name => ScreenName.SessionStart;
        public bool IsAllowChangeScreen(ScreenName newScreen)
        {
            return newScreen != ScreenName.Restart;
        }

        public StartSessionScreenController(
            AtlasShareManager atlasShareManager,
            SignalBus signalBus,
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader)
        {
            _bundleLoader = bundleLoader;
            _signalBus = signalBus;
            _atlasShareManager = atlasShareManager;
        }

        public async void Enter()
        {
            //Debug.Log("test: " + Quaternion.LookRotation(Vector3.forward, Vector3.left).eulerAngles);
            //Debug.Log("test: " + Quaternion.LookRotation(Vector3.forward, Vector3.up).eulerAngles);
            //Debug.Log("test: " + Quaternion.LookRotation(new Vector3(1, 1, 0), Vector3.up).eulerAngles);
            await _atlasShareManager.Initialize();
            _signalBus.Fire(
                new GameActionSignal<IModuleContextModel>(
                    GameAction.ModuleCreate,
                    new SplashScreenModel()));
        }

        public void Out()
        {
            return;
        }
    }
}
