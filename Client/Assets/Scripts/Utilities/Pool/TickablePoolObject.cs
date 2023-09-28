
using Core.Infrastructure;
using UnityEngine;
using Zenject;

namespace Core.Framework
{
    public abstract class TickablePoolObject : BasePoolObj, ITickable
    {
        public abstract void Tick();
    }
}
