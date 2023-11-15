using Core;
using Support;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class ScrollingBackground : DisposableClass
    {
        private readonly RawImage _background;
        private readonly float _x;
        private readonly float _y;

        public ScrollingBackground(RawImage background, float x, float y = 0)
        {
            _background = background;
            _x = x;
            _y = y;
        }

        protected override void OnInit()
        {
            base.OnInit();

            Observable
                .EveryUpdate()
                .SafeSubscribe(_ => MoveBackGround())
                .AddTo(Disposables);
        }

        private void MoveBackGround()
        {
            _background.uvRect =
                new Rect(_background.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _background.uvRect.size);
        }
    }
}