using System;
using R3;
using Services;
using Support;

namespace States
{
    public class LobbyState : GameStateBase<Unit>
    {
        private readonly GameMachine _gameMachine;
        private readonly WindowsService _windowsService;
        private readonly WindowResolver _windowResolver;
        private DisposableBag _rootDisposable = new();

        private const string StateSceneName = "Lobby";

        public LobbyState(GameMachine gameMachine, WindowsService windowsService, WindowResolver windowResolver)
        {
            _gameMachine = gameMachine;
            _windowsService = windowsService;
            _windowResolver = windowResolver;
        }

        protected override void Init()
        {
            SceneExtensions.LoadScene(StateSceneName)
                .SafeSubscribe(_ => OnSceneLoaded())
                .AddTo(ref _rootDisposable);
        }

        private void OnSceneLoaded()
        {
            InitControllers()
                .AddTo(ref _rootDisposable);
        }

        private IDisposable InitControllers()
        {
            var subscriptions = new CompositeDisposable();

            var lobbyRoot = SceneExtensions.LoadSceneRoot<LobbyRoot>();

            lobbyRoot
                .Init(new LobbyRoot.Model(OnGameStartRequested, _windowsService, _windowResolver))
                .AddTo(subscriptions);

            return subscriptions;
        }

        private void OnGameStartRequested()
        {
            _gameMachine.ChangeState<GameState>();
        }

        protected override void Deinit()
        {
            _rootDisposable.Clear();
        }
    }
}