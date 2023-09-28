namespace Core.Framework
{
    public class LoadingModel : BaseModuleContextModel
    {
        public override string ViewId => "Assets/Bundles/Views/Loading/Loading.prefab";
        public override ModuleName ModuleName => ModuleName.Loading;

        public double TotalMegaBytes;
        public double DownloadedMegaBytes;
        public float Progress;
        public string DownloadSpeed;
        public bool IsCompleted => Progress >= 1;
        public ViewState State; 
        public enum ViewState  
        {
            PreDownloadAsset,
            Loading
        }
    }
}