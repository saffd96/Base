using System;
using System.Collections.Generic;
using Core;
using R3;
using UnityEngine;

namespace States
{
    public class GameMachine : DisposableClass
    {
        private readonly Dictionary<Type, IGameState> _states = new();
        private IGameState _currentState;

        private readonly Subject<Type> _stateChangedSubject = new();
        public Observable<Type> StateChanged => _stateChangedSubject;

        public Type CurrentStateType => _currentState?.GetType();

        protected override void OnInit()
        {
            base.OnInit();

            Disposable
                .Create(OnDisposed)
                .AddTo(ref Disposables);
        }

        public void AddState<T>(T state) where T : IGameState
        {
            var stateType = typeof(T);

            if (_states.TryAdd(stateType, state))
                return;

            Debug.LogWarning($"[GameMachine] State {stateType.Name} is already registered. Overwriting.");
            _states[stateType] = state;
        }

        public void ChangeState<T>() where T : IGameState
        {
            var stateType = typeof(T);

            if (!_states.TryGetValue(stateType, out var state))
            {
                Debug.LogError($"[GameMachine] State {stateType.Name} not found. Check registration in InitStates().");
                return;
            }

            if (_currentState == state)
            {
                Debug.LogWarning($"[GameMachine] State {stateType.Name} is already active. Re-initialization is not allowed.");
                return;
            }

            _currentState?.Deinit();
            _currentState = state;
            _currentState.Init();

            _stateChangedSubject.OnNext(stateType);

            Debug.Log($"[GameMachine] State changed to {stateType.Name}");
        }

        protected override void OnDisposed()
        {
            _currentState?.Deinit();
            _stateChangedSubject.OnCompleted();
        }
    }
}