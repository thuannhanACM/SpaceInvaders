using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine.SocialPlatforms;
using Zenject.Asteroids;
using Zenject;

namespace Core.Framework
{
    //TODO: Implement navigation later.
    public partial class GameStore
    {
        //public enum NavigationUIView
        //{
        //    Lobby = 0,
        //    ShowRewardPopup
        //}
        //public abstract class BaseNavigationPage
        //{
        //    public string Id;
        //    public ModuleName Name;
        //    public Action<IModuleContextModel> Callback;
        //    public abstract UniTask CreateModule(GameStore gameStore);
        //    public abstract void Refresh(IModuleContextModel data);
        //}
        //public class NavigationToPage : BaseNavigationPage
        //{
        //    public NavigationToPage(Action<IModuleContextModel> callBack)
        //    {
        //        Callback = callBack;
        //    }

        //    public override UniTask CreateModule(GameStore gameStore)
        //    {
        //        return UniTask.CompletedTask;
        //    }

        //    public override void Refresh(IModuleContextModel data)
        //    {
        //        Callback.Invoke(data);
        //    }
        //}
        //public class NavigationToModule<TClass, TModel> : BaseNavigationPage
        //    where TClass : IBaseModule
        //    where TModel : IModuleContextModel, new()
        //{
        //    public NavigationToModule(string id, ModuleName name, Action<IModuleContextModel> callBack = null)
        //    {
        //        Id = id;
        //        Name = name;
        //        Callback = callBack;
        //    }

        //    public override UniTask CreateModule()
        //    {
        //        return GameStore.CreateModule<TClass, TModel>(Id, ViewName.Lua, Name);
        //    }

        //    public override void Refresh(IModuleContextModel data)
        //    {
        //        if (Callback == null)
        //            _gameStore.GState.GetModel<TModel>().Refresh();
        //        else
        //            Callback.Invoke(data);
        //    }
        //}
        //public class Navigation
        //{
        //    private bool _isOpeningModule;

        //    private Dictionary<NavigationUIView, BaseNavigationPage> _navigationPages = new();
        //    private void InitCallbacks()
        //    {
        //        //_navigationPages.Add(NavigationUIView.Shop,
        //        //    new NavigationToModule<IShop, ShopModel>(_gameSetting.ShopId, ModuleName.Shop));

        //        //_navigationPages.Add(NavigationUIView.Campaign, new NavigationToPage(OpenCampaign));

        //    }
        //    public delegate void UnlockNavigationDelegate(string view, bool isUnlock);
        //    private void NavigateTo(NavigationToViewSignal e)
        //    {
        //        LuaCall_NavigateTo(e.View, e.Data);
        //    }
        //    private void RunPreNavigationEvent(NavigationUIView destKey)
        //    {
        //        _signalBus.Fire(new OnNavigationViewChangeSignal(destKey));
        //    }
        //    private void RunNavigationSuccessEvent(NavigationUIView destKey)
        //    {
        //        _signalBus.Fire(new OnNavigationToViewSuccessSignal(destKey));
        //    }
        //    private void ShowOverlayLoading()
        //    {
        //        //_generalModuleCall.TriggerGeneralHelper(GeneralHelperSetting.LoadingOn);
        //    }
        //    private void CloseOverlayLoading()
        //    {
        //        //_generalModuleCall.TriggerGeneralHelper(GeneralHelperSetting.OffState);
        //    }
        //    #region Navigator Callback
        //    private void OpenCampaign(IModuleContextModel data)
        //    {
        //        //_gameStore.ChangeScreen(ScreenName.Campaign).Forget();
        //    }
        //    private async void OpenDuelBettingUI(IModuleContextModel data)
        //    {
        //        string freeHeroes = await _dataServiceClient.GetFreeHeroesMetaForDuelBetting();
        //        _gameStore.GState.GetModel<DuelBettingModel>().SetDefaultDatas(freeHeroes, (int)_userData.ServerData.UserData.Gem).Refresh();
        //    }
        //    private void OpenCrusadeUI(IModuleContextModel data)
        //    {
        //        ChangeScreen(ScreenName.GamePlay);
        //    }
        //    #endregion Navigator
        //    public async void NavigateTo(NavigationUIView view, IModuleContextModel data = null)
        //    {
        //        if (_isOpeningModule) return;
        //        _isOpeningModule = true;
        //        ShowOverlayLoading();
        //        if (_navigationPages.ContainsKey(view)
        //            && _navigationPages[view] != null)
        //        {
        //            RunPreNavigationEvent(view);
        //            await _navigationPages[view].CreateModule();
        //            _navigationPages[view].Refresh(data);
        //            RunNavigationSuccessEvent(view);
        //        }
        //        CloseOverlayLoading();
        //        _isOpeningModule = false;
        //    }
        //}

    }
}
