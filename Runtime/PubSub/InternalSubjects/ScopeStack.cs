// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Lib.Pooling;
using Utility.Asserts;

namespace Lib.DataFlow
{
    internal class ScopeStack : IDisposable, IScope
    {
        HashSet<Action> _set;
        bool _disposed;
        static Pool<HashSet<Action>> HashSetPool { get; } = new Pool<HashSet<Action>>(() => new HashSet<Action>(), subs => subs.Clear());
        static Pool<List<Action>> ListPool { get; } = new Pool<List<Action>>(() => new List<Action>(), subs => subs.Clear());

        public ScopeStack()
        {
            _Scope.Register(this);
            _Scope.Next(s => s.CtorStackTrace, StackTraceHolder.New(3), this);
            
            _disposed = false;
            _set = HashSetPool.Get();
            Asr.IsTrue(_set.Count == 0);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            using (ListPool.GetScoped(out var tmpList))
            {
                tmpList.AddRange(_set);
                HashSetPool.Release(ref _set);

                // emulate stack
                var count = tmpList.Count;
                for (var i = count - 1; i >= 0; i--)
                    tmpList[i].Invoke();
            }
            
            _Scope.Next(scope => scope.Dispose, this);
            _Scope.Deregister(this);
        }

        public bool Completed => _disposed;

        public void OnDispose(Action dispose)
        {
            if (_disposed)
            {
                dispose.Invoke();
                return;
            }

            _set.Add(dispose);
            _Scope.Next(s => s.OnDispose, (StackTraceHolder.New(1), dispose), this);
        }

        public void Unsubscribe(Action dispose)
        {
            if (_disposed) return;

            _set.Remove(dispose);
            _Scope.Next(s => s.Unsubscribe, (StackTraceHolder.New(1), dispose), this);
        }
    }
}