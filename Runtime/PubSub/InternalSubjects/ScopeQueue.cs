using System;
using System.Collections.Generic;
using Lib.Pooling;
using Utility.AssertN;

namespace Lib.DataFlow
{
    internal class ScopeQueue : IDisposable, IScope
    {
        Queue<Action> _stack;
        bool _disposed;
        public static Pool<ScopeQueue> Pool { get; } = new Pool<ScopeQueue>(() => new ScopeQueue(), subs => subs._SetNew());

        public ScopeQueue()
        {
            _stack = new Queue<Action>();
            _SetNew();
        }

        void _SetNew()
        {
            _Scope.Register(this);
            
            _disposed = false;
            Asr.IsTrue(_stack.Count == 0);
            _stack.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            while (_stack.Count > 0)
            {
                var dispose = _stack.Dequeue();
                dispose.Invoke();
            }
        }

        public void OnDispose(Action dispose)
        {
            if (_disposed)
            {
                dispose.Invoke(); 
                return;
            }

//            Assert.IsTrue(_stack.Count < 300);

            _stack.Enqueue(dispose);
        }
    }
}