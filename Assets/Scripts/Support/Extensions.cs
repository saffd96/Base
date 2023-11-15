using System;
using DG.Tweening;
using UniRx;

namespace Support
{
    public static class Extensions
    {
        public static IObservable<Sequence> PlayAsObservable(this Sequence sequence)
        {
            return Observable.Create<Sequence>(o =>
            {
                sequence.OnComplete(() =>
                {
                    o.OnNext(sequence);
                    o.OnCompleted();
                });
                sequence.Play();
                return Disposable.Create(() => { sequence.Kill(); });
            });
        }

        public static IObservable<Tween> PlayAsObservable(this Tween tweener)
        {
            return Observable.Create<Tween>(o =>
            {
                tweener.OnComplete(() =>
                {
                    o.OnNext(tweener);
                    o.OnCompleted();
                });
                return Disposable.Create(() => { tweener.Kill(); });
            });
        }
    }
}