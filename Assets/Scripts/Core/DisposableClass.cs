using System;
using R3;

namespace Core
{
    public class DisposableClass
    {
        protected DisposableBag Disposables;

        public IDisposable Init()
        {
            Disposables = new DisposableBag();

            OnInit();

            return Disposables;
        }

        protected virtual void OnInit()
        {
        }
    }
}