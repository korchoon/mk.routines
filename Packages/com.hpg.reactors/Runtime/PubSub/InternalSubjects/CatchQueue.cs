using System;
using System.Collections.Generic;

namespace Lib.DataFlow
{
    internal class CatchQueue : IDisposeWith<Exception>, IScope<Exception>
    {
        Queue<Action<Exception>> _stack;
        bool _disposed;

        public CatchQueue(out IScope<Exception> scope)
        {
            _stack = new Queue<Action<Exception>>();
            _disposed = false;
            scope = this;
        }

        public void DisposeWith(Exception err)
        {
            if (_disposed) return;

            _disposed = true;

            while (_stack.Count > 0)
            {
                var dispose = _stack.Dequeue();
                dispose.Invoke(err);
            }
        }

        public void OnDispose(Action<Exception> onErr)
        {
            if (_disposed)
                return;

            _stack.Enqueue(onErr);
        }
    }
}