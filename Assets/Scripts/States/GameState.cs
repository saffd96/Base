using System;
using R3;
using Services;
using Support;

namespace States
{
    public class GameState : GameStateBase<Unit>
    {
        private readonly GameMachine _gameMachine;
        private readonly WindowsService _windowsService;
        private readonly WindowResolver _windowResolver;
        
        private DisposableBag _rootDisposable = new();

        private const string StateSceneName = "Game";

        public GameState(GameMachine gameMachine, WindowsService windowsService, WindowResolver windowResolver)
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

            var gameRoot = SceneExtensions.LoadSceneRoot<GameRoot>();

            gameRoot
                .Init(new LobbyRoot.Model(OnExit, _windowsService, _windowResolver))
                .AddTo(subscriptions);

            return subscriptions;
        }

        private void OnExit()
        {
            _gameMachine.ChangeState<LobbyState>();
        }

        protected override void Deinit()
        {
            _rootDisposable.Clear();
        }
    }
}