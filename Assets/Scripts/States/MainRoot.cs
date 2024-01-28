using R3;
using Services;
using Support;
using UnityEngine;

namespace States
{
    public class MainRoot : MonoBehaviour
    {
        [SerializeField] private WindowsService _windowsService;

        private GameMachine _gameMachine;

        private DisposableBag _rootDisposable = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartRoot();
        }

        private void StartRoot()
        {
            Observable.ReturnUnit()
                .Concat(InitServices())
                .Concat(InitStates())
                .SafeSubscribe(_ => LoadStage())
                .AddTo(ref _rootDisposable);
        }

        private Observable<Unit> InitStates()
        {
            _gameMachine = new GameMachine();

            var windowResolver = new WindowResolver(_windowsService);

            _gameMachine
                .Init()
                .AddTo(ref _rootDisposable);

            _gameMachine.AddState(new LobbyState(_gameMachine, _windowsService, windowResolver));
            _gameMachine.AddState(new GameState(_gameMachine, _windowsService, windowResolver));

            return Observable.ReturnUnit();
        }

        private Observable<Unit> InitServices()
        {
            _windowsService.Init(new WindowsService.Model()).AddTo(ref _rootDisposable);

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