using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Core.Framework.Services
{
    public class DownloadDependency
    {
        public class DownloadInfo
        {
            public double TotalMegaBytes;
            public double DownloadedMegaBytes;
            public float Progress;
            public string DownloadSpeed;
        }

        private readonly SignalBus _signalBus;

        public DownloadDependency(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void DownloadAssets(
            List<string> keys,
            Addressables.MergeMode mergeMode)
        {
            var handle = Addressables.DownloadDependenciesAsync(keys, mergeMode);
            SharedDownloadAssetsThread(handle).Forget();
        }

        private async UniTaskVoid SharedDownloadAssetsThread(AsyncOperationHandle handle)
        {
            var downloadStatus = GetDownloadStatusObj(handle);
            Stopwatch downloadTimeCounter = new Stopwatch();
            downloadTimeCounter.Start();

            while (!handle.GetDownloadStatus().IsDone)
            {
                UpdateProgress(downloadStatus, handle, downloadTimeCounter);
                await UniTask.NextFrame();
            }

            downloadTimeCounter.Stop();

            DownloadDone(handle);
            Addressables.Release(handle);
        }

        private DownloadInfo GetDownloadStatusObj(AsyncOperationHandle handle)
        {
            DownloadInfo downloadStatus = new DownloadInfo()
            {
                TotalMegaBytes =
                    handle.GetDownloadStatus().TotalBytes /
                    AddressableLoader.ONE_MEGABYTE_TO_BYTE
            };
            return downloadStatus;
        }

        private void UpdateProgress(
            DownloadInfo status, 
            AsyncOperationHandle handle, 
            Stopwatch downloadTimeCounter)
        {
            long downloadedBytes = handle.GetDownloadStatus().DownloadedBytes;
            float percent = handle.GetDownloadStatus().Percent;

            status.DownloadedMegaBytes = downloadedBytes / AddressableLoader.ONE_MEGABYTE_TO_BYTE;
            status.Progress = percent;

            double KBsPerSecond =
                status.DownloadedMegaBytes 
                * AddressableLoader.ONE_MEGABYTE_TO_KILOBYTE / downloadTimeCounter.Elapsed.TotalSeconds;

            if (KBsPerSecond >= AddressableLoader.ONE_MEGABYTE_TO_KILOBYTE)
                status.DownloadSpeed =
                       string.Format("({0:0.00 MB/s})", KBsPerSecond / AddressableLoader.ONE_MEGABYTE_TO_KILOBYTE);
            else
                status.DownloadSpeed =
                        string.Format("({0:0.00 KB/s})", KBsPerSecond);

            _signalBus.Fire(new LoadingProgressSignal(status));
        }

        private void DownloadDone(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Failed 
                || handle.Status == AsyncOperationStatus.None)
                _signalBus.Fire(new AddressableErrorSignal($"Download Error with Status: {handle.Status}"));
        }
    }
}
