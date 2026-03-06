using System;
using Core;
using Lobby;
using R3;
using Support;
using UnityEngine;
using UnityEngine.UI;
using Services.WindowService;

namespace States
{
    public class LobbyRoot : DisposableBehaviour<LobbyRoot.Model>
    {
        public class Model
        {
            public readonly Action OnGameAction;
            public readonly WindowsService WindowsService;
            public readonly WindowResolver WindowResolver;

            public Model(Action onGameAction, WindowsService windowsService, WindowResolver windowResolver)
            {
                OnGameAction = onGameAction;
                WindowsService = windowsService;
                WindowResolver = windowResolver;
            }
        }

        [Header("Lobby Specific")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Transform _playButtonTransform;
        [SerializeField] private RawImage _visualBackground;
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;
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

            // Play button initialization
            if (_playButton != null)
            {
                var playConfirmWindow = ActiveModel.WindowResolver.GetPlayConfirmWindowModel(
                    ActiveModel.OnGameAction, true);

                _playButton
                    .OnClickAsObservable()
                    .SafeSubscribe(_ => ActiveModel.WindowsService.Open(playConfirmWindow, false))
                    .AddTo(ref Disposables);
            }

            // Play button animation
            if (_playButtonTransform != null)
            {
                new StartButtonLoop(_playButtonTransform, _endValue, _duration)
                    .Init()
                    .AddTo(ref Disposables);
            }
        }
    }
}