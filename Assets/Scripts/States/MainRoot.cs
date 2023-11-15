using System;
using Services;
using Support;
using UniRx;
using UnityEngine;

namespace States
{
    public class MainRoot : MonoBehaviour
    {
        [SerializeField] private WindowsService _windowsService;

        private GameMachine _gameMachine;

        private readonly CompositeDisposable _rootDisposable = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartRoot();
        }

        private void StartRoot()
        {
            Observable.ReturnUnit()
                .ContinueWith(_ => InitServices())
                .ContinueWith(_ => InitStates())
                .SafeSubscribe(_ => LoadStage())
                .AddTo(_rootDisposable);
        }

        private IObservable<Unit> InitStates()
        {
            _gameMachine = new GameMachine();

            var windowResolver = new WindowResolver(_windowsService);

            _gameMachine
                .Init()
                .AddTo(_rootDisposable);

            _gameMachine.AddState(new LobbyState(_gameMachine, _windowsService, windowResolver));
            _gameMachine.AddState(new GameState(_gameMachine, _windowsService, windowResolver));

            return Observable.ReturnUnit();
        }

        private IObservable<Unit> InitServices()
        {
            _windowsService.Init(new WindowsService.Model()).AddTo(_rootDisposable);

            return Observable.ReturnUnit();
        }

        private void LoadStage()
        {
            _gameMachine.ChangeState<LobbyState>();
        }

        private void OnDestroy()
        {
            _rootDisposable.Clear();
        }
    }
}