
using UnityEngine;

namespace Core.Framework.Helpers
{
    public interface ICoreInputHandler
    {
        Vector2 Direction { get; }
        void HandleKeyInput();
    }
}
