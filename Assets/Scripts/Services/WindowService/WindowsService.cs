using System;
using System.Collections.Generic;
using Core;
using R3;
using UnityEngine;

namespace Services
{
    public class WindowsService : DisposableBehaviour<WindowsService.Model>
    {
        public class Model
        {
        }

        [SerializeField] private List<WindowBase> _prefabs;
        [SerializeField] private Transform _anchor;

        private const int EnvironmentOrder = 1;
        private readonly List<WindowBase> _instances = new();
        private readonly Stack<WindowBase> _windowsStack = new();

        public Observable<Unit> ObserveWindowOpen<T>() =>
            CurrentWindow.Where(window => window != null && window.GetType() == typeof(T))
                .Select(_ => _)
                .Take(1)
                .AsUnitObservable();

        public Observable<Unit> ObserveWindowClose<T>() =>
            CurrentWindow.Pairwise()
                .Where(pair => pair.Previous != null && pair.Previous.GetType() == typeof(T))
                .Select(_ => _)
                .Take(1)
                .AsUnitObservable();

        public Observable<Unit> ObserveWindowFullClose<T>()
        {
            return _windowClosedObservable.Where(closingWindowType => closingWindowType == typeof(T))
                .Take(1)
                .AsUnitObservable();
        }

        public ReadOnlyReactiveProperty<WindowBase> CurrentWindow => _currentWindow;

        private readonly ReactiveCommand<Type> _windowClosedObservable = new();
        private readonly ReactiveProperty<WindowBase> _currentWindow = new();

        private DisposableBag _loadingDisposable = new();

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Open(object model, bool isDisplacing = true)
        {
            if (isDisplacing)
            {
                while (_windowsStack.Count > 0)
                {
                    var closingWindow = _windowsStack.Pop();

                    closingWindow.Close();

                    _windowClosedObservable.Execute(closingWindow.GetType());
                }
            }

            var currentWindow = GetOrCreateWindow(model.GetType());

            currentWindow.SetOrder(_windowsStack.Count + EnvironmentOrder);

            currentWindow.Open(model);

            _windowsStack.Push(currentWindow);

            OnWindowsQueueChanged();
        }

        public void Close()
        {
            if (_windowsStack.Count > 0)
            {
                var currentWindow = _windowsStack.Pop();

                currentWindow.Close();

                _windowClosedObservable.Execute(currentWindow.GetType());

                OnWindowsQueueChanged();
            }
        }

        protected override void OnInit()
        {
            Disposable.Create(OnDisposed).AddTo(ref Disposables);
        }

        private WindowBase GetOrCreateWindow(Type modelType)
        {
            var instance = _instances.Find(instanceArg => instanceArg.ModelType == modelType);

            if (instance == null)
            {
                instance = CreateWindowInstance(modelType);
            }

            return instance;
        }

        private WindowBase CreateWindowInstance(Type modelType)
        {
            var prefab = _prefabs.Find(windowInstanceArg => windowInstanceArg.ModelType == modelType);

            var instance = GameObject.Instantiate(prefab, _anchor);

            _instances.Add(instance);

            return instance;
        }

        private void OnDisposed()
        {
            _loadingDisposable.Clear();

            while (_windowsStack.Count > 0)
            {
                Close();
            }

            foreach (var instance in _instances)
            {
                GameObject.Destroy(instance.gameObject);
            }

            _instances.Clear();
        }

        private void OnWindowsQueueChanged()
        {
            _currentWindow.Value = _windowsStack.Count > 0 ? _windowsStack.Peek() : null;
        }
    }
}