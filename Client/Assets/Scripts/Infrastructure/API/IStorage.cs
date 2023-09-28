
namespace Core.Infrastructure
{
    public interface IStorage
    {
        T Load<T>(string key, string filePath);

        void Save<T>(string key, T value, string filePath);
    }
}
