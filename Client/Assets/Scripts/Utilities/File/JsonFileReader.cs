
using Core.Infrastructure;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Framework.Utilities
{
    public class JsonFileReader: IFileReader
    {
        public async UniTask<T> Read<T>(string filePath)
        {
            string dir = FileUtil.GetJsonFilePath(filePath);
            await UniTask.Delay(1);
            if (FileUtil.CheckFileExist(dir))
                return GetContent<T>(dir);
#if UNITY_EDITOR
            throw new FireNotFound(dir);
#else
            return default;
#endif
        }

        private T GetContent<T>(string dir)
        {
            byte[] data = FileUtil.LoadFile(dir);
            string json = System.Text.Encoding.UTF8.GetString(data);

            T content = JsonConvert.DeserializeObject<T>(json);
            return content;
        }        
    }
}
