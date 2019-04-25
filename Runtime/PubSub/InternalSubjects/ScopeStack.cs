using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lib.Pooling;
using UnityEngine.Assertions;
using Utility.AssertN;

namespace Lib.DataFlow
{
    internal class ScopeStack : IDisposable, IScope
    {
        Stack<Action> _stack;
        bool _disposed;
        public static Pool<ScopeStack> Pool { get; } = new Pool<ScopeStack>(() => new ScopeStack(), subs => subs._SetNew());

        public ScopeStack()
        {
            _stack = new Stack<Action>();
            _SetNew();
        }

        void _SetNew()
        {
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
                var dispose = _stack.Pop();
                dispose.Invoke();
            }
        }

        public void OnDispose(Action dispose)
        {
            if (_disposed)
            {
                dispose.Invoke(); //todo probable reason
                return;
            }

//            Assert.IsTrue(_stack.Count < 300);

            _stack.Push(dispose);
        }
    }
}