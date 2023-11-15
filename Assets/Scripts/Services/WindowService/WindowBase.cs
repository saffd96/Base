using System;
using Support;
using UniRx;
using UnityEngine;

namespace Services
{
    public abstract class WindowBase<T> : WindowBase
    {
        [SerializeField] private SafeArea _safeArea;

        public sealed override Type ModelType => typeof(T);
        protected T ActiveModel { get; private set; }

        protected readonly CompositeDisposable Disposables = new();

        public sealed override void Open(object model)
        {
            ActiveModel = (T)model;

            _safeArea.Fit();

            OnOpen();

            this.gameObject.SetActive(true);
        }

        public sealed override void Close()
        {
            Disposables.Clear();

            ActiveModel = default;

            this.gameObject.SetActive(false);
        }

        protected abstract void OnOpen();
    }

    public abstract class WindowBase : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        public abstract Type ModelType { get; }

        public void SetOrder(int order)
        {
            _canvas.sortingOrder = order;
        }

        public abstract void Open(object model);
        public abstract void Close();
    }
}