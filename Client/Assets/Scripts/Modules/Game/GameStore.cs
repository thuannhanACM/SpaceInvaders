using System;
using System.Collections.Generic;
using Zenject;
using System.Linq;
using Core.Infrastructure;
using Core.Infrastructure.Extensions;

namespace Core.Framework
{
    public partial class GameStore : IInitializable, IDisposable
    {
        private static SignalBus _signalBus;
        private readonly ILogger _logger;

        public State GState { get; set; }

        public GameStore(
            State gState,            
            SignalBus signalBus,
            ILogger logger,
            List<IGameReducer<IModuleContextModel>> contextModelReducer,
            List<IGameReducer<ModuleName>> moduleNameReducer,
            List<IGameReducer<ScreenName>> screenNameReducer)
        {
            _signalBus = signalBus;
            _logger = logger;
            GState = gState;

            new Reducer<IModuleContextModel>(contextModelReducer, signalBus);
            new Reducer<ModuleName>(moduleNameReducer, signalBus);
            new Reducer<ScreenName>(screenNameReducer, signalBus);
        }

        public void Initialize()
        {
            GState.CurrentScreen.Enter();
        }

        public void Dispose()
        {
            GState.OnQuit();
        }

        #region Helper function
        public static void CreateModule(IModuleContextModel model)
        {
            _signalBus.Fire(
               new GameActionSignal<IModuleContextModel>(GameAction.ModuleCreate, model));
        }
        public static void RemoveModule<T>(T model)
        {
            _signalBus.Fire(
               new GameActionSignal<T>(GameAction.ModuleRemove, model));
        }
        public static void ChangeScreen(ScreenName name)
        {
            _signalBus.Fire(
               new GameActionSignal<ScreenName>(GameAction.ScreenChange, name));
        }
        #endregion
    }
}
