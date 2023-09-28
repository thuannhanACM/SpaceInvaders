using Core.Infrastructure;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{
    public partial class SplashScreen : BaseModule<SplashScreenView, SplashScreenModel>
    {
        private readonly ILogger _logger;
        
        public SplashScreen(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnViewReady()
        {
            //_viewContext.RegisObj(this);
            //_viewContext.RegisObjType<FullScreenMode>();
            //_viewContext.RegisObjType<AudioMixer>();

            //_config = _viewContext.ConfigModel.GetConfigs<SettingUIConfig>();

            //AsyncViewReady();
        }

        private void AsyncViewReady()
        {
            //_audioMixer = await _bundleLoader.LoadAssetAsync<AudioMixer>(_config.AudioPaths[0]);
            //_viewContext.Call(LuaAction.OnDoneLoadAudioMixer);
        }

        protected override void OnDisposed()
        {

        }

        public void LuaCall_StartSession()
        {
        }

    }
}