using System;

namespace Core.Framework
{
    public interface IGameReducer<T>
    {
        GameReducerInfo<T>[] RegisHandler();
    }

    public struct GameReducerInfo<T>
    {
        public GameAction Action;
        public Action<T> Handler;
    }
}
