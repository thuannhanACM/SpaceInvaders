
using Core.Business;

namespace Core.Framework
{
    public class OnApplicationQuitSignal
    {
        public OnApplicationQuitSignal()
        {}
    }

    public class OnLoginSuccess
    {
        public string WalletAddress { get; private set; }
        public string AnalyticsEnvironment { get; private set; }
        public OnLoginSuccess(string walletAddress, string analyticsEnvironment)
        {
            WalletAddress = walletAddress;
            AnalyticsEnvironment = analyticsEnvironment;
        }
    }
}
