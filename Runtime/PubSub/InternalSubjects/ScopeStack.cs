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
        static Pool<Stack<Action>> StackPool { get; } = new Pool<Stack<Action>>(() => new Stack<Action>(), subs => subs.Clear());
        public static Pool<ScopeStack> ScopePool { get; } = new Pool<ScopeStack>(() => new ScopeStack(), subs => subs.Reset());

        public ScopeStack()
        {
            _stack = StackPool.Get();
            Reset();
        }

        void Reset()
        {
            _disposed = false;
            Asr.IsTrue(_stack.Count == 0);
//            _stack.Clear();
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

            StackPool.Release(_stack);
        }

        public void OnDispose(Action dispose)
        {
            if (_disposed)
            {
                dispose.Invoke();
                return;
            }

            _stack.Push(dispose);
        }
    }
}