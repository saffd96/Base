using System;
using R3;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base class for MonoBehaviour with IDisposable support and automatic subscription cleanup.
    /// </summary>
    public class DisposableBehaviour : MonoBehaviour
    {
        protected DisposableBag Disposables;

        protected virtual void Awake()
        {
            Disposables = new DisposableBag();
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDestroy()
        {
            Disposables.Clear();
        }
    }

    /// <summary>
    /// Base class for MonoBehaviour with model and IDisposable support.
    /// Used for initialization via Init(model).
    /// </summary>
    public class DisposableBehaviour<T> : MonoBehaviour
    {
        protected DisposableBag Disposables;
        protected T ActiveModel { get; private set; }

        public IDisposable Init(T model)
        {
            Disposables = new DisposableBag();
            ActiveModel = model;
            OnInit();
            return Disposables;
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDestroy()
        {
            Disposables.Clear();
        }
    }
}