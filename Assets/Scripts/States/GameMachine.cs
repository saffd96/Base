using System;
using System.Collections.Generic;
using Core;
using UniRx;

namespace States
{
    public class GameMachine : DisposableClass
    {
        private readonly Dictionary<Type, IGameState> _states = new();
        private IGameState _currentState;

        protected override void OnInit()
        {
            base.OnInit();

            Disposable
                .Create(OnDisposed)
                .AddTo(Disposables);
        }

        public void AddState(IGameState state)
        {
            _states.Add(state.GetType(), state);
        }

        public void ChangeState<T>()
        {
            _currentState?.Deinit();
            _currentState = _states[typeof(T)];
            _currentState.Init();
        }

        private void OnDisposed()
        {
            _currentState?.Deinit();
        }
    }
}