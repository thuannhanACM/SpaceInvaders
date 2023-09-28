using UnityEngine;
using Core.Framework.Services;
using Core.Framework.Utilities;
using System.Diagnostics;
using Zenject;
using Core.Infrastructure.Logger;
using Core.Infrastructure;
using Core.Business;
using Game.Gameplay;

namespace Core.Framework
{
    public enum ModuleName
    {
        GameStore,
        SplashScreen,
        Loading,
        BattleHUD,

        All
    }
    public enum BundleLoaderName
    {
        Resource,
        Addressable
    }
    public enum PoolName
    {
        Object,
        Audio
    }

    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Application.targetFrameRate = 60;

            Container.Bind<GameStore.State>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStore>().AsSingle();

            GameRootInstaller.Install(Container);
            BussinessInstaller.Install(Container);

            UnityEngine.Debug.LogFormat("GameInstaller took {0:0.00} seconds", stopwatch.Elapsed.TotalSeconds);
            stopwatch.Stop();
        }

        private void OnApplicationQuit()
        {
            Container.Resolve<SignalBus>().Fire(new OnApplicationQuitSignal());
        }
    }

    public class GameRootInstaller : Installer<GameRootInstaller>
    {
        public override void InstallBindings()
        {
            InstallReducers();
            InstallSingletons();
            InstallGameSignal();
            InstallGameState();
            InstallModules();
            InstallServices();
            InstallShareLogger();            
            InstallUtilities();
#if UNITY_EDITOR
            Container.BindInterfacesTo<PoolCleanupChecker>().AsSingle();
#endif
        }

        private void InstallReducers()
        {
            Container.BindInterfacesTo<ModuleReducer>().AsSingle();
            Container.BindInterfacesTo<ModuleReducerByName>().AsSingle();
            Container.BindInterfacesTo<ScreenReducer>().AsSingle();
        }

        private void InstallSingletons()
        {
            Container.BindInterfacesTo<ConfigTableLoaderWithBundle>().AsSingle();
            Container.BindInterfacesTo<ConfigTableManager>().AsSingle();
        }

        private void InstallGameState()
        {
            Container.Bind<IScreenController>().WithId(ScreenName.SessionStart).To<StartSessionScreenController>().AsSingle();
            Container.Bind<IScreenController>().WithId(ScreenName.GamePlay).To<GamePlayScreenController>().AsSingle();
        }

        private void InstallModules()
        {
            Container.BindFactory<IBaseModule, BaseModuleFactory>()
                .WithId(ModuleName.SplashScreen).To<SplashScreen>()
                .FromSubContainerResolve()
                .ByInstaller<SplashScreen.Installer>();
            Container.BindFactory<GameObject, IModuleContextModel, IView, ViewFactory>()
                .WithId(ModuleName.SplashScreen)
                .FromFactory<ViewCustomFactory<SplashScreenView>>();

            Container.BindFactory<IBaseModule, BaseModuleFactory>()
                .WithId(ModuleName.Loading).To<Loading>()
                .FromSubContainerResolve()
                .ByInstaller<Loading.Installer>();
            Container.BindFactory<GameObject, IModuleContextModel, IView, ViewFactory>()
                .WithId(ModuleName.Loading)
                .FromFactory<ViewCustomFactory<LoadingView>>();

            Container.BindFactory<IBaseModule, BaseModuleFactory>()
                .WithId(ModuleName.BattleHUD).To<BattleHUD>()
                .FromSubContainerResolve()
                .ByInstaller<BattleHUD.Installer>();
            Container.BindFactory<GameObject, IModuleContextModel, IView, ViewFactory>()
                .WithId(ModuleName.BattleHUD)
                .FromFactory<ViewCustomFactory<BattleHUDView>>();
        }

        private void InstallShareLogger()
        {
            Container.BindInterfacesTo<UnityConsoleLogger>().AsSingle();
            Container.BindInterfacesTo<ErrorHandler>().AsSingle().NonLazy();
        }

        private void InstallServices()
        {
            Container.Bind<IBundleLoader>().WithId(BundleLoaderName.Resource).To<ResourceLoader>().AsSingle();
            Container.Bind<IBundleLoader>().WithId(BundleLoaderName.Addressable).To<AddressableLoader>().AsSingle();
            Container.Bind<IPoolManager>().WithId(PoolName.Object).To<PoolManager>().AsSingle();
            Container.Bind<AudioPoolManager>().AsSingle();
        }

        private void InstallGameSignal()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignalWithInterfaces<GameActionSignal>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<GameActionSignal<IModuleContextModel>>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<GameActionSignal<ModuleName>>().OptionalSubscriber().RunAsync();
            Container.DeclareSignalWithInterfaces<GameActionSignal<ScreenName>>().OptionalSubscriber().RunAsync();

            Container.DeclareSignal<BattleSignal>().OptionalSubscriber();

            Container.DeclareSignal<OnScreenChangeSignal>().OptionalSubscriber();
            Container.DeclareSignal<GameScreenForceChangeSignal>().OptionalSubscriber();
            Container.DeclareSignal<CheckDownloadSizeStatusSignal>().OptionalSubscriber();
            Container.DeclareSignal<LoadingProgressSignal>().OptionalSubscriber();
            Container.DeclareSignal<AddressableErrorSignal>().OptionalSubscriber();

            Container.DeclareSignal<GameAudioSignal>().OptionalSubscriber();
            Container.DeclareSignal<PlayOneShotAudioSignal>().OptionalSubscriber();
            Container.DeclareSignal<OnLoginSuccess>().OptionalSubscriber();
            Container.DeclareSignal<OnApplicationQuitSignal>().OptionalSubscriber();

        }

        private void InstallUtilities()
        {
            Container.BindInterfacesTo<JsonFileReader>().AsSingle();
            Container.Bind<AtlasShareManager>().AsSingle();
            Container.Bind<SoundDataLoader>().AsSingle();
        }
    }

    public class BussinessInstaller : Installer<BussinessInstaller>
    {
        public override void InstallBindings()
        {
            InstallConfigTablesNonLazy();
        }

        private void InstallConfigTablesNonLazy()
        {
            
        }
    }
}
