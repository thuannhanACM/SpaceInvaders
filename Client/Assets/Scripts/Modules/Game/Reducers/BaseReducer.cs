using Core.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Core.Framework
{
    public class Reducer<T>: IDisposable
    {
        private readonly List<IGameReducer<T>> _reducers;
        private readonly SignalBus _signalBus;

        private GameReducerInfo<T>[] _reducerInfos;

        public Reducer(List<IGameReducer<T>> reducers, SignalBus signalBus)
        {
            _reducers = reducers;
            _signalBus = signalBus;
            ReducerRegister();
            _signalBus.Subscribe<GameActionSignal<T>>(Dispatch);
        }

        public void Dispose()
        {
            _signalBus.Subscribe<GameActionSignal<T>>(Dispatch);
        }

        private void ReducerRegister()
        {
            List<GameReducerInfo<T>> tmp = new List<GameReducerInfo<T>>();
            foreach (var reducer in _reducers)
                tmp.AddRange(reducer.RegisHandler());
            _reducerInfos = tmp.ToArray();
        }

        private void Dispatch(GameActionSignal<T> signal)
        {
            if (_reducerInfos.IsNullOrEmpty())
                return;

            GameReducerInfo<T>[] reducers = _reducerInfos.Where(_ => _.Action == signal.Action).ToArray();
            if (!reducers.IsNullOrEmpty())
                foreach (var reducer in reducers)
                    reducer.Handler(signal.NewModel);
        }

        
    }
}
