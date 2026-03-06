using Core;
using R3;
using Services.WindowService;
using Support;
using UnityEngine;

namespace States
{
    /// <summary>
    /// Game root object. Initializes the bootstrapper.
    /// </summary>
    public class MainRoot : MonoBehaviour
    {
        [SerializeField] private WindowsService _windowsService;

        private GameBootstrapper _bootstrapper;

        private DisposableBag _rootDisposable = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartBootstrap();
        }

        private void StartBootstrap()
        {
            var bootstrapper = new GameBootstrapper(_windowsService);
            _bootstrapper = bootstrapper;

            bootstrapper
                .Bootstrap()
                .SafeSubscribe(_ => { })
                .AddTo(ref _rootDisposable);
        }

        private void OnDestroy()
        {
            _rootDisposable.Clear();
            _bootstrapper?.Dispose();
        }
    }
}