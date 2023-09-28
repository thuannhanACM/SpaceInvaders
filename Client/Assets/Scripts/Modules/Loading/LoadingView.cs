
using Core.Business;
using Core.Framework.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{
    public class LoadingView : View<Loading>
    {
        #region Serializable
        [SerializeField]
        private TextMeshProUGUI _totalDownloadInfo;
        [SerializeField]
        private TextMeshProUGUI _downloadStatus;
        [SerializeField]
        private Button _btnAcceptDownload;
        [SerializeField]
        private GameObject _loadingGunContainer;
        [SerializeField]
        private GameObject _downloadInfoContainer;
        [SerializeField]
        private Button _btnContinue;
        #endregion

        private ILogger _logger;
        private IConfigTableManager _configTable;

        [Inject]
        public void Construct(
            IConfigTableManager configTableManager,
            ILogger logger)
        {
            _configTable = configTableManager;
            _logger = logger;
        }

        public void Start()
        {
            _logger.Log(name + "SplashScreenView Start");
        }

        public override void OnReady()
        {
            _btnAcceptDownload.onClick.RemoveAllListeners();
            _btnAcceptDownload.onClick.AddListener(AcceptDownload);

            _btnContinue.onClick.RemoveAllListeners();
            _btnContinue.onClick.AddListener(Continue);
            _btnContinue.gameObject.SetActive(false);
        }

        private void AcceptDownload()
        {
            _downloadInfoContainer.SetActive(false);
            _loadingGunContainer.SetActive(true);
            Module.DownloadPreloadAssets();
        }
        private void Continue()
        {
            Module.OpenLobbyScreen();
        }

        public void ShowDownloadInfo()
        {
            string content = string.Format("{0:0.00} MB", Module.Model.TotalMegaBytes);
            _totalDownloadInfo.text = content;
        }

        public async void ShowContinue()
        {
            await FirstLoadDefinitions();
            
            _downloadInfoContainer.SetActive(false);
            _btnContinue.gameObject.SetActive(true);
        }

        private async UniTask FirstLoadDefinitions()
        {
            await _configTable.GetDefinitionAll<ShipDefinition>();
            await _configTable.GetDefinitionAll<BulletDefinition>();
            await _configTable.GetDefinitionAll<AlienDefinition>();
        }

        public void StartDownload()
        {
            _loadingGunContainer.SetActive(true);
        }

        public void UpdateDownloadProcess()
        {
            string statusContent =
                string.Format("Downloading... {0:0.00 MB}/{1:0.00 MB} {2}",
                Module.Model.DownloadedMegaBytes,
                Module.Model.TotalMegaBytes,
                Module.Model.DownloadSpeed);
            _downloadStatus.text = statusContent;
        }

        public void ShowLoading()
        {
            _downloadInfoContainer.SetActive(false);
            _btnContinue.gameObject.SetActive(false);
        }

    }
}
