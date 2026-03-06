using Core;
using Lobby;
using R3;
using Services.WindowService;
using Support;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
    public class GameRoot : DisposableBehaviour<GameRoot.Model>
    {
        public class Model
        {
            public readonly System.Action OnGameAction;
            public readonly WindowsService WindowsService;
            public readonly WindowResolver WindowResolver;

            public Model(System.Action onGameAction, WindowsService windowsService, WindowResolver windowResolver)
            {
                OnGameAction = onGameAction;
                WindowsService = windowsService;
                WindowResolver = windowResolver;
            }
        }

        [SerializeField] private RawImage _visualBackground;
        [SerializeField] private Button _quitButton;
        [SerializeField] private float _uvMovingByX;

        protected override void OnInit()
        {
            base.OnInit();

            // Background initialization
            if (_visualBackground != null)
            {
                new ScrollingBackground(_visualBackground, _uvMovingByX)
                    .Init()
                    .AddTo(ref Disposables);
            }

            // Quit button initialization
            if (_quitButton != null)
            {
                var playConfirmWindow = ActiveModel.WindowResolver.GetPlayConfirmWindowModel(
                    ActiveModel.OnGameAction, false);

                _quitButton
                    .OnClickAsObservable()
                    .SafeSubscribe(_ => ActiveModel.WindowsService.Open(playConfirmWindow, false))
                    .AddTo(ref Disposables);
            }
        }
    }
}