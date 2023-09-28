
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Core.Framework.Utilities
{
    public class CsvFileReader : IFileReader
    {
        private readonly IBundleLoader _bundleLoader;
        public CsvFileReader(
           [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader addressable)
        {
            _bundleLoader = addressable;
        }

        public async UniTask<T> Read<T>(string filePath)
        {
            string dir = FileUtil.GetJsonFilePath(filePath);
            await UniTask.Delay(1);
            if (FileUtil.CheckFileExist(dir))
            {
                byte[] data = FileUtil.LoadFile(dir);
                string text = System.Text.Encoding.UTF8.GetString(data);
                return GetContent<T>(text);
            }
                
#if UNITY_EDITOR
            throw new FireNotFound(dir);
#else
            return default;
#endif
        }

        public async UniTask<T> ReadByAddressable<T>(string addressPath)
        {
            TextAsset text = await _bundleLoader.LoadAssetAsync<TextAsset>(addressPath);
            return GetContent<T>(text.text);
        }

        private T GetContent<T>(string text)
        {
            T content = JsonConvert.DeserializeObject<T>(text);
            return content;
        }
    }
}
