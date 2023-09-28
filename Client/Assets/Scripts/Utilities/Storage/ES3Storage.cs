using Core.Infrastructure;

#if USE_ES3
namespace Core.Framework.Utilities
{
    public class ES3Storage : IStorage
    {

        public T Load<T>(
            string key,
            string filePath)
        {
            string dir = FileUtil.GetWritablePath(filePath);
            if (ES3.FileExists(dir) && ES3.KeyExists(key, dir))
            {
                return ES3.Load<T>(key, dir);
            }

            return default(T);
        }

        public void Save<T>(
            string key, T value,
            string filePath)
        {
            string dir = FileUtil.GetWritablePath(filePath);
            ES3.Save(key, value, dir);
        }
    }
}
#endif
