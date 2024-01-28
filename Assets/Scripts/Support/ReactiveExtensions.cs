using System;
using DG.Tweening;
using R3;

namespace Support
{
    public static class ReactiveExtensions
    {
        public static IDisposable EmptySubscribe<T>(this Observable<T> source)
        {
            return source.SafeSubscribe(_ => Observable.Empty<Unit>());
        }

        public static IDisposable SafeSubscribe<T>(this Observable<T> source, Action<T> action)
        {
            return source
                .Catch<T, Exception>(exception =>
                {
                    UnityEngine.Debug.LogException(exception);

                    return Observable.Return<T>(default);
                })
                .Subscribe(action);
        }

        public static IDisposable SafeSubscribe<T>(this Observable<T> source, Action<T> onNext,
            Action<Result> onCompleted)
        {
            return source
                .Catch<T, Exception>(exception =>
                {
                    UnityEngine.Debug.LogException(exception);

                    return Observable.Return<T>(default);
                })
                .Subscribe(onNext, onCompleted);
        }

        public static Observable<T> SkipInitialization<T>(this Observable<T> source)
        {
            return source.Skip(1);
        }

        public static Observable<Sequence> PlayAsObservable(this Sequence sequence)
        {
            Action DisposableMethod(Sequence seq)
            {
                return () => seq.Kill();
            }
            
            IDisposable SeqMethod(Observer<Sequence> seq)
            {
                sequence.OnComplete(() =>
                {
                    seq.OnNext(sequence);
                    seq.OnCompleted();
                });
                sequence.Play();
                return Disposable.Create(DisposableMethod(sequence));
            }

            return Observable.Create<Sequence>(SeqMethod);
        }

        public static Observable<Tween> PlayAsObservable(this Tween tweener)
        {
            Action DisposableMethod(Tween seq)
            {
                return () => seq.Kill();
            }
            
            IDisposable SeqMethod(Observer<Tween> seq)
            {
                tweener.OnComplete(() =>
                {
                    seq.OnNext(tweener);
                    seq.OnCompleted();
                });
                tweener.Play();
                return Disposable.Create(DisposableMethod(tweener));
            }

            return Observable.Create<Tween>(SeqMethod);
        }
    }
}