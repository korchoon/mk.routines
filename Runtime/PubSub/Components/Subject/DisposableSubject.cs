using System;
using System.Collections.Generic;
using Lib.Async;
using UnityEngine.Assertions;

namespace Lib.DataFlow
{
    internal class DisposableSubject : IDisposable, IScope
    {
        Stack<Action> _stack;
        bool _disposed;

        public DisposableSubject()
        {
            _stack = new Stack<Action>();
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            while (_stack.Count > 0)
            {
                var dispose = _stack.Pop();
                dispose.Invoke();
            }
        }
        
        public void OnDispose(Action dispose)
        {
            if (_disposed)
            {
                dispose();
                return;
            }
            
//            Assert.IsTrue(_stack.Count < 300);
                
            _stack.Push(dispose);
        }
    }
}