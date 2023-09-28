
using Cysharp.Threading.Tasks;

namespace Core.Framework
{
    public class StandardPoolObject : BasePoolObj
    {
        public override UniTask Reinitialize()
        {
            return UniTask.CompletedTask;
        }
    }
}
