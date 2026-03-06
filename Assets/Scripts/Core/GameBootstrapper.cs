using System;
using R3;
using States;
using UnityEngine;
using Services.WindowService;

namespace Core
{
    /// <summary>
    /// Game bootstrapper — manages initialization of services and states.
    /// Extracted from MainRoot for better testability and separation of concerns.
    /// </summary>
    public class GameBootstrapper : DisposableClass
    {
        private readonly WindowsService _windowsService;
        private GameMachine _gameMachine;

        public GameBootstrapper(WindowsService windowsService)
        {
            _windowsService = windowsService;
        }

        public Observable<Unit> Bootstrap()
        {
            return Observable.ReturnUnit()
                .Concat(InitServices())
                .Concat(InitStates())
                .Do(_ => LoadFirstState())
                .Catch<Unit, Exception>(ex =>
                {
                    Debug.LogException(ex);
                    return Observable.ReturnUnit();
                });
        }

        private Observable<Unit> InitServices()
        {
            _windowsService.Init(new WindowsService.Model());
            return Observable.ReturnUnit();
        }

        private Observable<Unit> InitStates()
        {
            _gameMachine = new GameMachine();
            _gameMachine.Init().AddTo(ref Disposables);

            // Create WindowResolver after service initialization
            var windowResolver = new WindowResolver(_windowsService);

            // Create and pass dependencies to states directly
            var lobbyState = new LobbyState(_gameMachine, _windowsService, windowResolver);
            var gameState = new GameState(_gameMachine, _windowsService, windowResolver);

            _gameMachine.AddState(lobbyState);
            _gameMachine.AddState(gameState);

            return Observable.ReturnUnit();
        }

        private void LoadFirstState()
        {
            _gameMachine.ChangeState<LobbyState>();
        }
    }
}
