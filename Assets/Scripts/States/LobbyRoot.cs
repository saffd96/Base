using System;
using Core;
using Lobby;
using Services;
using Support;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Button _playButton;
        [SerializeField] private Transform _playButtonTransform;
        [SerializeField] private RawImage _visualBackground;
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;
        [SerializeField] private float _uvMovingByX;

        protected override void OnInit()
        {
            base.OnInit();

            var playConfirmWindow = ActiveModel.WindowResolver.GetPlayConfirmWindowModel(ActiveModel.OnGameAction);

            _playButton
                .OnClickAsObservable()
                .SafeSubscribe(_ => ActiveModel.WindowsService.Open(playConfirmWindow, false))
                .AddTo(Disposables);

            new ScrollingBackground(_visualBackground, _uvMovingByX)
                .Init()
                .AddTo(Disposables);

            new StartButtonLoop(_playButtonTransform, _endValue, _duration)
                .Init()
                .AddTo(Disposables);
        }
    }
}