using System;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Support
{
    public static class SceneExtensions
    {
        private static readonly string[] SceneNames =
        {
            "Main",    // SceneType.Main
            "Lobby",   // SceneType.Lobby
            "Game"     // SceneType.Game
        };

        public static string GetSceneName(this SceneType sceneType)
        {
            var index = (int)sceneType;
            if (index < 0 || index >= SceneNames.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(sceneType), $"Unknown scene type: {sceneType}");
            }
            return SceneNames[index];
        }

        public static Observable<Unit> LoadScene(this SceneType sceneType)
        {
            var sceneName = sceneType.GetSceneName();
            
            return Observable.Create<Unit>(observer =>
            {
                var isDisposed = false;

                var operation = SceneManager.LoadSceneAsync(sceneName);
                operation.completed += Handler;

                return Disposable.Create(() =>
                {
                    isDisposed = true;
                    operation.completed -= Handler;
                });

                void Handler(AsyncOperation asyncOperation)
                {
                    if (isDisposed) return;
                    
                    asyncOperation.completed -= Handler;

                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                }
            });
        }

        public static T LoadSceneRoot<T>() where T : MonoBehaviour
        {
            return Object.FindFirstObjectByType<T>();
        }

        public static Observable<Unit> ObserveSceneLoaded(this SceneType sceneType)
        {
            var sceneName = sceneType.GetSceneName();
            
            return Observable.Create<Unit>(observer =>
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                if (scene.isLoaded)
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

                return Disposable.Create(() => SceneManager.sceneLoaded -= Handler);
            });
        }
    }
}