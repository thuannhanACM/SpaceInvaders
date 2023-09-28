using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
using Game.Gameplay;
using Zenject;

namespace Core.Framework
{
    public class GamePlayScreenController : IScreenController
    {
        private readonly ILogger _logger;
        private readonly SignalBus _signalBus;
        private readonly GameManager _gameManager;

        public ScreenName Name => ScreenName.GamePlay;
        public bool IsAllowChangeScreen(ScreenName newScreen) => true;

        public GamePlayScreenController(
            GameManager gameManager,
            SignalBus signalBus,
            ILogger logger)
        {
            _signalBus = signalBus;
            _logger = logger;
            _gameManager = gameManager;
        }

        public async void Enter()
        {
            GameStore.CreateModule(new BattleHUDModel());
            await _gameManager.LoadBattle("");
            GameStore.RemoveModule(ModuleName.Loading);
        }

        public void Out()
        {
            GameStore.RemoveModule(ModuleName.BattleHUD);
            return;
        }
    }
}
