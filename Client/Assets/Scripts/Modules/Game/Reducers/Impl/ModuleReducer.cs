using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

#pragma warning disable 0618
namespace Core.Framework
{
    public class ModuleReducer: IGameReducer<IModuleContextModel>
    {
        private readonly DiContainer _container;
        private readonly GameStore.State GState;
        private readonly ILogger _logger;

        public ModuleReducer(
            DiContainer container, 
            GameStore.State gState,
            ILogger logger)
        {
            _container = container;
            GState = gState;
            _logger = logger;
        }
        public GameReducerInfo<IModuleContextModel>[] RegisHandler()
        {
            return new GameReducerInfo<IModuleContextModel>[]
            {
                new GameReducerInfo<IModuleContextModel>(){Action = GameAction.ModuleCreate, Handler = CreateModule },
                new GameReducerInfo<IModuleContextModel>(){Action = GameAction.ModuleRemove, Handler = RemoveModule }
            };
        }

        private async void CreateModule(IModuleContextModel model)
        {
            if (GState.HasModel(model))
                _logger.Warning($"Dupplicate Found on Module: {model.ModuleName}");

            GState.PreaddModelToAvoidDup(model);

            IBaseModule module = await CreateModuleAndView(model);

            GState.BindModuleToModel(model, module);
        }

        private async UniTask<IBaseModule> CreateModuleAndView(IModuleContextModel model)
        {
            BaseModuleFactory baseContextFactory = _container.ResolveId<BaseModuleFactory>(model.ModuleName);
            IBaseModule module = baseContextFactory.Create();

            GameObject pref = await module.Initialize(model);
            return await CreateView(model, module, pref);
        }

        private async UniTask<IBaseModule> CreateView(IModuleContextModel model, IBaseModule module, GameObject pref)
        {
            ViewFactory viewFactory = _container.ResolveId<ViewFactory>(model.ModuleName);
            IView view = viewFactory.Create(pref, model);
            await module.CreateView(model, view);
            return module;
        }

        private void RemoveModule(IModuleContextModel model)
        {
            if (GState.HasModel(model))
                GState.RemoveModel(model);
            else
                _logger.Warning($"Try to remove non exist module: {model.ModuleName}");
        }
    }

    public class ModuleReducerByName : IGameReducer<ModuleName>
    {
        private readonly GameStore.State GState;
        private readonly ILogger _logger;

        public ModuleReducerByName(
            DiContainer container,
            GameStore.State gState,
            ILogger logger)
        {
            GState = gState;
            _logger = logger;
        }
        public GameReducerInfo<ModuleName>[] RegisHandler()
        {
            return new GameReducerInfo<ModuleName>[]
            {
                new GameReducerInfo<ModuleName>(){Action = GameAction.ModuleRemove, Handler = RemoveModule },
                new GameReducerInfo<ModuleName>(){Action = GameAction.ModuleHide, Handler = HideModule },
                new GameReducerInfo<ModuleName>(){Action = GameAction.ModuleShow, Handler = ShowModule },
            };
        }

        private void RemoveModule(ModuleName module)
        {
            if(module == ModuleName.All)
                GState.RemoveAllModules();
            else if (GState.HasModel(module))
                GState.RemoveModel(module);
            else
                _logger.Warning($"Try to remove non exist module: {module}");
        }

        private void HideModule(ModuleName moduleName)
        {
            if (GState.TryGetModule(moduleName, out IBaseModule module))
                module.ContextView.Hide();
            else
                _logger.Warning($"Try to remove non exist module: {moduleName}");
        }

        private void ShowModule(ModuleName moduleName)
        {
            if (GState.TryGetModule(moduleName, out IBaseModule module))
                module.ContextView.Show();
            else
                _logger.Warning($"Try to remove non exist module: {moduleName}");
        }

    }
}
#pragma warning restore 0618
