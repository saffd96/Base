using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Support
{
    public static class SceneExtensions
    {
        public static IObservable<Unit> LoadScene(string sceneName)
        {
            return Observable.Create<Unit>(subject =>
            {
                var disposable = new CompositeDisposable();

                void Handler(AsyncOperation operation)
                {
                    operation.completed -= Handler;

                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                }

                var operation = SceneManager.LoadSceneAsync(sceneName);
                operation.completed += Handler;

                Disposable
                    .Create(() => operation.completed -= Handler)
                    .AddTo(disposable);

                return disposable;
            });
        }

        public static T LoadSceneRoot<T>() where T : MonoBehaviour
        {
            return Object.FindObjectOfType<T>();
        }

        public static IObservable<Unit> ObservableSceneLoaded(string sceneName)
        {
            return Observable.Create<Unit>(observer =>
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                void Handler(Scene loadedScene, LoadSceneMode mode)
                {
                    if (loadedScene.name == sceneName)
                    {
                        SceneManager.sceneLoaded -= Handler;
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    }
                }

                SceneManager.sceneLoaded += Handler;

                return Disposable.Create(() => { SceneManager.sceneLoaded -= Handler; });
            });
        }
    }
}