using System;
using System.Collections.Generic;
using Lib.Attributes;
using Lib.Pooling;
using Utility.Asserts;

namespace Lib.DataFlow
{
    internal class ScopeStack : IDisposable, IScope
    {
        List<Action> _stack;
        static Pool<List<Action>> Pool { get; } = new Pool<List<Action>>(() => new List<Action>(), subs => subs.Clear());

        public ScopeStack()
        {
            _Scope.Register(this);
            _Scope.Next(s => s.CtorStackTrace, StackTraceHolder.New(3), this);

            Completed = false;
            _stack = Pool.Get();
            Asr.IsTrue(_stack.Count == 0);
        }

        public void Dispose()
        {
            if (Completed) return;


            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                var t = _stack[i];
                t.Invoke();
            }

            Completed = true;
            
            _stack.Clear();
            Pool.Release(ref _stack);

            _Scope.Next(scope => scope.Dispose, this);
            _Scope.Deregister(this);
        }

        public bool Completed { get; private set; }

        public void OnDispose(Action dispose)
        {
            if (Completed)
            {
                dispose.Invoke();
                return;
            }

            _stack.Add(dispose);
            _Scope.Next(s => s.OnDispose, (StackTraceHolder.New(1), dispose), this);
        }

        [NonPerformant(PerfKind.TimeHeavy)]
        public void Unsubscribe(Action dispose)
        {
            if (Completed)
            {
                Asr.Fail("Cannot unsubscribe during or after disposal");
                return;
            }

            var any = _stack.Remove(dispose);
            Asr.IsTrue(any, "Delegate not found: make sure it's the same which was passed to OnDispose");

            _Scope.Next(s => s.Unsubscribe, (StackTraceHolder.New(1), dispose), this);
        }
    }
}