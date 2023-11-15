using Core;
using DG.Tweening;
using Support;
using UniRx;
using UnityEngine;

namespace Lobby
{
    public class StartButtonLoop : DisposableClass
    {
        private readonly float _endValue;
        private readonly float _duration;

        private readonly Transform _buttonTransform;

        public StartButtonLoop(Transform buttonTransform, float endValue, float duration)
        {
            _buttonTransform = buttonTransform;
            _endValue = endValue;
            _duration = duration;
        }

        protected override void OnInit()
        {
            base.OnInit();

            _buttonTransform.DOScale(_endValue, _duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo)
                .PlayAsObservable()
                .EmptySubscribe()
                .AddTo(Disposables);
        }
    }
}