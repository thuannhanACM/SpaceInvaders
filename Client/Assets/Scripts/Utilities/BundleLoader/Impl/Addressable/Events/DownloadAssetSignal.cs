

using static Core.Framework.Services.DownloadDependency;

namespace Core.Framework.Services
{
    public class CheckDownloadSizeStatusSignal
    {
        public double Total;

        public CheckDownloadSizeStatusSignal(double totalCapacity)
        {
            Total = totalCapacity;
        }
    }

    public class LoadingProgressSignal
    {
        public DownloadInfo DownloadInfo;

        public LoadingProgressSignal(DownloadInfo progress)
        {
            DownloadInfo = progress;
        }
    }

    public class AddressableErrorSignal
    {
        public string ErrorMessage;

        public AddressableErrorSignal(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
