using System;
using Game.Proto;
using Lib.DataFlow;

namespace Lib.Utility
{
    internal class ActionOnDispose2 : IAwait, IDisposable
    {
        Action _onDispose;

        public ActionOnDispose2(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            _onDispose?.Invoke();
            _onDispose = null;
        }

        public bool IsCompleted { get; private set; }
    }
}