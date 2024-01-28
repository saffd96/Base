using Core;
using Lobby;
using R3;
using Support;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
    public class GameRoot : DisposableBehaviour<LobbyRoot.Model>
    {
        [SerializeField] private RawImage _visualBackground;
        [SerializeField] private Button _quitButton;
        [SerializeField] private float _uvMovingByX;

        protected override void OnInit()
        {
            base.OnInit();

            var playConfirmWindow =
                ActiveModel.WindowResolver.GetPlayConfirmWindowModel(ActiveModel.OnGameAction, false);
            
            _quitButton
                .OnClickAsObservable()
                .SafeSubscribe(_ => ActiveModel.WindowsService.Open(playConfirmWindow, false))
                .AddTo(ref Disposables);

            new ScrollingBackground(_visualBackground, _uvMovingByX)
                .Init()
                .AddTo(ref Disposables);
        }
    }
}