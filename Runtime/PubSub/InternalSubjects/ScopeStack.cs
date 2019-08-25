// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Reactors.Async.Debugger;
using Reactors.Attributes;
using Reactors.Pooling;
using Utility.Asserts;

namespace Reactors.DataFlow
{
    internal class ScopeStack : IDisposable, IScope
    {
        List<Action> _stack;
        public bool Disposing { get; private set; }
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
            if (Disposing) return; // todo turn off and reproduce

            Disposing = true;

            for (var i = _stack.Count - 1; i >= 0; i--)
            {
                Asr.IsNotNull(_stack);
                var t = _stack[i];
                _stack[i] = null;
                Asr.IsNotNull(t);
                t.Invoke();

                if (Completed) return; //todo reproduce case
            }

            Completed = true;

            _stack.Clear();
            Pool.Release(ref _stack);

            _Scope.Next(scope => scope.Dispose, this);
            _Scope.Deregister(this);
        }

        public bool Completed { get; private set; }

        public void Subscribe(Action dispose)
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
            if (Disposing || Completed)
            {
//                Asr.Fail("Cannot unsubscribe during or after disposal");
                return;
            }

            var any = _stack.Remove(dispose);
            Asr.IsTrue(any, "Delegate not found: make sure it's the same which was passed to OnDispose");

            _Scope.Next(s => s.Unsubscribe, (StackTraceHolder.New(1), dispose), this);
        }
    }
}