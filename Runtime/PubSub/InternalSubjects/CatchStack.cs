// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Lib.DataFlow
{
    internal class CatchStack : IDisposeWith<Exception>, IErrorScope<Exception>
    {
        Stack<Action<Exception>> _stack;
        bool _disposed;

        public CatchStack(out IErrorScope<Exception> scope)
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