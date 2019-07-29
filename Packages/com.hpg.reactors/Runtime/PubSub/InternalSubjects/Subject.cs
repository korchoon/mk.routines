// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.Async;
using Lib.Attributes;
using Lib.Pooling;
using Utility.Asserts;

namespace Lib.DataFlow
{
    [NonPerformant(PerfKind.GC)]
    internal class Subject<T> : ISub<T>, IPub<T>
    {
        Subscribers<T> _next;

        static readonly Pool<Subscribers<T>> NextPool = new Pool<Subscribers<T>>(() => new Subscribers<T>(), subs => subs.Reset());

        internal bool Completed => _next == null || _next.Completed;

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
        }

        void _Dispose()
        {
            if (Completed) return;

            _next.Dispose();

            NextPool.Release(ref _next);
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
            Asr.IsFalse(Completed);
            if (Completed)
                return;

            _next.Sub(pub, scope);
        }


        public void Next(T msg)
        {
            Asr.IsFalse(Completed);
            if (Completed) return;

            _next.Next(msg);
        }
    }

    internal class Subject : ISub, IPub
    {
        Subscribers _next;

        static readonly Pool<Subscribers> NextPool = new Pool<Subscribers>(() => new Subscribers(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
        }

        void _Dispose()
        {
            if (Completed) return;

            _next.Dispose();
            NextPool.Release(ref _next);
        }


        public void OnNext(Action pub, IScope scope)
        {
            Asr.IsFalse(Completed);
            if (Completed)
                return;

            _next.Sub(pub, scope);
        }


        public void Next()
        {
            Asr.IsFalse(Completed);
            if (Completed) return;

            _next.Next();
        }

        internal bool Completed => _next == null || _next.Completed;
    }
}