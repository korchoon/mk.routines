using System;
using System.Collections.Generic;

namespace Lib.DataFlow
{
    internal class CatchSubject : IDisposeWith<Exception>, IScope<Exception>
    {
        Stack<Action<Exception>> _stack;
        bool _disposed;

        public CatchSubject(out IScope<Exception> scope)
        {
            _stack = new Stack<Action<Exception>>();
            _disposed = false;
            scope = this;
        }

        public void DisposeWith(Exception err)
        {
            if (_disposed) return;

            _disposed = true;

            while (_stack.Count > 0)
            {
                var dispose = _stack.Pop();
                dispose.Invoke(err);
            }
        }

        public void OnDispose(Action<Exception> onErr)
        {
            if (_disposed)
                return;

            _stack.Push(onErr);
        }
    }
}