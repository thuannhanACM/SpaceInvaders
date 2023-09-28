using Core.Infrastructure;
using Zenject;

namespace Core.Framework
{
    public class ScreenReducer : IGameReducer<ScreenName>
    {
        private readonly SignalBus _signalBus;
        private readonly GameStore.State GState;
        private readonly DiContainer _container;
        private readonly ILogger _logger;

        public ScreenReducer(
            DiContainer container,
            GameStore.State gState,
            SignalBus signalBus,
            ILogger logger)
        {
            GState = gState;
            _logger = logger;
            _signalBus = signalBus;
            _container = container;
        }

        public GameReducerInfo<ScreenName>[] RegisHandler()
        {
            return new GameReducerInfo<ScreenName>[]
            {
                new GameReducerInfo<ScreenName>(){Action = GameAction.ScreenChange, Handler = ChangeScreen },
                new GameReducerInfo<ScreenName>(){Action = GameAction.ScreenReEnter, Handler = ReEnterScreen },
                new GameReducerInfo<ScreenName>(){Action = GameAction.ScreenForceChange, Handler = ForceChangeScreen }
            };
        }

        private void ChangeScreen(ScreenName newScreenName)
        {
            if (newScreenName == GState.CurrentScreen.Name || !GState.CurrentScreen.IsAllowChangeScreen(newScreenName))
                return;

            GState.CurrentScreen.Out();
            EnterNewScreen(newScreenName);
        }

        private void ForceChangeScreen(ScreenName newScreenName)
        {
            GState.CurrentScreen.Out();
            EnterNewScreen(newScreenName);
            ScreenName previousScreenName = GState.CurrentScreen != null ? GState.CurrentScreen.Name : ScreenName.SessionStart;
            _signalBus.Fire(new GameScreenForceChangeSignal(newScreenName, previousScreenName));
        }

        private void ReEnterScreen(ScreenName screen)
        {
            EnterNewScreen(screen);
        }

        private void EnterNewScreen(ScreenName newScreenName)
        {
            ScreenName previousScreenName = GState.CurrentScreen != null ? GState.CurrentScreen.Name : ScreenName.SessionStart;
            GState.CurrentScreen = _container.ResolveId<IScreenController>(newScreenName);
            GState.CurrentScreen.Enter();
            if (previousScreenName != newScreenName)
                _signalBus.Fire(new OnScreenChangeSignal(newScreenName, previousScreenName));
        }
    }
}
