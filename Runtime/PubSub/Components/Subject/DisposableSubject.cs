using System;
using System.Collections.Generic;
using Lib.Async;
using Lib.Pooling;
using UnityEngine.Assertions;
using Utility.AssertN;

namespace Lib.DataFlow
{
    internal class DisposableSubject : IDisposable, IScope
    {
        Stack<Action> _stack;
        bool _disposed;
        public static Pool<DisposableSubject> Pool { get; } = new Pool<DisposableSubject>(() => new DisposableSubject(), subs => subs.Reset());

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

        void Reset()
        {
            _disposed = false;
            Asr.IsTrue(_stack.Count == 0);
            _stack.Clear();
        }
    }
}