// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.Async;
using Lib.Async.Debugger;
using Lib.Attributes;
using Lib.Pooling;
using Utility.Asserts;

namespace Lib.DataFlow
{
    internal class Subject<T> : ISub<T>, IPub<T>
    {
        Subscribers<T> _next;

        static readonly Pool<Subscribers<T>> NextPool = new Pool<Subscribers<T>>(() => new Subscribers<T>(), subs => subs.Reset());

        internal bool Completed => _next == null || _next.Completed;

        public Subject(IScope scope)
        {
            _Subject<T>.Register(this, StackTraceHolder.New(3).GetName(false));
            Asr.IsFalse(scope.Completed);
            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
        }

        void _Dispose()
        {
            if (Completed) return;

            _next.Dispose();

            _Subject<T>.Next(s => s.Dispose, StackTraceHolder.New(1).GetName(false), this);
            NextPool.Release(ref _next);
        }

        public void OnNext(Action<T> pub, IScope scope)
        {
            if (Completed)
            {
                Asr.Fail("Tried to subscribe to ISub which is completed");
                return;
            }

            Asr.IsFalse(scope.Completed);
            _Subject<T>.Next(s => s.OnNext, StackTraceHolder.New(1).GetName(false), this);

            _next.Sub(pub, scope);
        }


        public void Next(T msg)
        {
            if (Completed)
            {
                Asr.Fail("Tried to publish Next to IPub which is completed");
                return;
            }

            _Subject<T>.Next(s => s.Next1, StackTraceHolder.New(1).GetName(false), this);
            _next.Next(msg);
        }
    }

    internal class Subject : ISub, IPub
    {
        Subscribers _next;

        static readonly Pool<Subscribers> NextPool = new Pool<Subscribers>(() => new Subscribers(), subs => subs.Reset());

        public Subject(IScope scope)
        {
            _Subject.Register(this, StackTraceHolder.New(3).GetName(false));
            Asr.IsFalse(scope.Completed);

            scope.OnDispose(_Dispose);
            _next = NextPool.Get();
        }

        void _Dispose()
        {
            if (Completed) return;

            _Subject.Next(s => s.Dispose, StackTraceHolder.New(1).GetName(false), this);
            _next.Dispose();
            NextPool.Release(ref _next);
        }


        public void OnNext(Action pub, IScope scope)
        {
            if (Completed)
            {
                Asr.Fail("Tried to subscribe to ISub which is completed");
                return;
            }

            Asr.IsFalse(scope.Completed);

            _Subject.Next(s => s.OnNext, StackTraceHolder.New(1).GetName(false), this);
            _next.Sub(pub, scope);
        }


        public void Next()
        {
            if (Completed)
            {
                Asr.Fail("Tried to publish Next to IPub which is completed");
                return;
            }

            _Subject.Next(s => s.Next1, StackTraceHolder.New(1).GetName(false), this);
            _next.Next();
        }

        internal bool Completed => _next == null || _next.Completed;
    }
}