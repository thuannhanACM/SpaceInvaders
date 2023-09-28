
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{
    public class SplashScreenView: View<SplashScreen>
    {
        [SerializeField]
        private Button _clickToContinue;

        private ILogger _logger;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            ILogger logger)
        {
            _logger = logger;
            _signalBus = signalBus;            
        }

        public override void OnReady()
        {
            _clickToContinue.onClick.RemoveAllListeners();
            _clickToContinue.onClick.AddListener(ClickToContinue);
        }

        private void ClickToContinue()
        {
            _clickToContinue.interactable = false;
            _signalBus.Fire(
                new GameActionSignal<IModuleContextModel>(
                    GameAction.ModuleCreate,
                    new LoadingModel() { State = LoadingModel.ViewState.PreDownloadAsset }));

            _signalBus.Fire(
                new GameActionSignal<IModuleContextModel>(
                    GameAction.ModuleRemove,
                    Module.ContextModel));            
        }
    }
}
