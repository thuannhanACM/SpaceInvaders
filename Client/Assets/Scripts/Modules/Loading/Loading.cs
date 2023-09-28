using Core.Framework.Services;
using Core.Infrastructure;
using System.Collections.Generic;
using Zenject;

namespace Core.Framework
{
    public partial class Loading : BaseModule<LoadingView, LoadingModel>
    {
        private readonly ILogger _logger;
        private readonly IBundleLoader _bundleLoader;
        private readonly SignalBus _signalBus;

        public Loading(
            ILogger logger,
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader,
            SignalBus signalBus)
        {
            _logger = logger;
            _bundleLoader = bundleLoader;
            _signalBus = signalBus;
        }

        protected override void OnViewReady() 
        { 
            
            if(Model.State == LoadingModel.ViewState.PreDownloadAsset)
            {
                _signalBus.Subscribe<CheckDownloadSizeStatusSignal>(OnDownloadStatus);
                _signalBus.Subscribe<AddressableErrorSignal>(OnAddressableErrorSignal);
                _signalBus.Subscribe<LoadingProgressSignal>(OnUpdateDownloadProgress);
                CheckDownloadSite();
            }
            else if (Model.State == LoadingModel.ViewState.Loading)
            {
                _view.ShowLoading();
            }
                
        }

        protected override void OnDisposed()
        {
            _signalBus.TryUnsubscribe<CheckDownloadSizeStatusSignal>(OnDownloadStatus);
            _signalBus.TryUnsubscribe<AddressableErrorSignal>(OnAddressableErrorSignal);
            _signalBus.TryUnsubscribe<LoadingProgressSignal>(OnUpdateDownloadProgress);
        }

        private void CheckDownloadSite()
        {
            List<string> bundleKeysGetFromCloudConfig = new List<string>() { };
            _bundleLoader.CheckPreloadAssets(bundleKeysGetFromCloudConfig);
        }

        private void OnDownloadStatus(CheckDownloadSizeStatusSignal dataSignal)
        {
            Model.TotalMegaBytes = dataSignal.Total;
            if (Model.TotalMegaBytes > 0.0f)
                _view.ShowDownloadInfo();
            else
                _view.ShowContinue();
        }

        public void DownloadPreloadAssets()
        {
            List<string> bundleKeysGetFromCloudConfig = new List<string>() { };
            _bundleLoader.DownloadPreloadAssets(bundleKeysGetFromCloudConfig);
        }

        private void OnUpdateDownloadProgress(LoadingProgressSignal data)
        {
            UpdateModelWithDownloadInfo(data);
            _view.UpdateDownloadProcess();

            if (Model.IsCompleted)
                OpenLobbyScreen();
        }

        private void UpdateModelWithDownloadInfo(LoadingProgressSignal data)
        {
            Model.Progress = data.DownloadInfo.Progress;
            Model.DownloadedMegaBytes = data.DownloadInfo.DownloadedMegaBytes;
            Model.DownloadSpeed = data.DownloadInfo.DownloadSpeed;
            Model.TotalMegaBytes = data.DownloadInfo.TotalMegaBytes;
        }

        public void OpenLobbyScreen()
        {
            GameStore.ChangeScreen(ScreenName.GamePlay);
        }

        private void OnAddressableErrorSignal(AddressableErrorSignal dataSignal)
        {
            throw new DownloadAssetException(dataSignal.ErrorMessage);
        }

        private class DownloadAssetException : System.Exception
        {
            public DownloadAssetException(string mess): base(mess)
            {

            }
        }
    }
}