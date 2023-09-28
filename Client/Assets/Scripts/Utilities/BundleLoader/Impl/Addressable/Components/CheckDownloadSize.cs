using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Core.Framework.Services
{
    public class CheckDownloadSize
    {
        private readonly SignalBus _signalBus;
        private double _totalCapacity;

        public CheckDownloadSize(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Init()
        {
            _totalCapacity = 0.0f;
        }

        public async UniTaskVoid CheckMultipleKeys(List<string> keys)
        {
            Init();
            await GetDownloadSizeAsync(keys);
        }

        private async UniTask GetDownloadSizeAsync(List<string> keys)
        {
            var handle = Addressables.GetDownloadSizeAsync(keys);

            if (!handle.GetDownloadStatus().IsDone)
                await UniTask.NextFrame();

            SharedMethodGetDownloadSizeAsync(handle);
        }

        private void SharedMethodGetDownloadSizeAsync(AsyncOperationHandle<long> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _totalCapacity = handle.Result / AddressableLoader.ONE_MEGABYTE_TO_BYTE;
                _signalBus.Fire(new CheckDownloadSizeStatusSignal(_totalCapacity));
            }
            else if (handle.Status == AsyncOperationStatus.Failed 
                || handle.Status == AsyncOperationStatus.None)
                _signalBus.Fire(new AddressableErrorSignal($"GetDownloadSize error with status: {handle.Status}"));

            Addressables.Release(handle);
        }
    }
}
