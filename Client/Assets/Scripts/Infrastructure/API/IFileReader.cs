
using Cysharp.Threading.Tasks;

namespace Core.Infrastructure
{
    public interface IFileReader
    {
        UniTask<T> Read<T>(string filePath);
    }
}
