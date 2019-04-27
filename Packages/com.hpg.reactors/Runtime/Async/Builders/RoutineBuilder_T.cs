using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using JetBrains.Annotations;
using Lib.DataFlow;
using Utility;
using Utility.AssertN;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    public class RoutineBuilder<T>
    {
        Action _continuation;
        IBreakableAwaiter _innerAwaiter;

        RoutineBuilder()
        {
            _RoutineBuilder.Register(this);
            _RoutineBuilder.Next(trc => trc.CtorTrace, StackTraceHolder.New(3), this);
            Task = new Routine<T>();
        }

        [UsedImplicitly]
        public static RoutineBuilder<T> Create() => new RoutineBuilder<T>();

        [UsedImplicitly] public Routine<T> Task { get; }


        [UsedImplicitly]
        public void SetResult(T value)
        {
            Task._res = value;
            Task._dispose.Dispose();
            _RoutineBuilder.Next(d => d.AfterSetResult, this);
        }

        [UsedImplicitly]
        public void SetException(Exception e)
        {
            Task.PubErr.DisposeWith(e);
            if (!(e is RoutineStoppedException))
            {
                Debug.LogException(e);
                SchPub.PubError.Next(e);
            }

            _RoutineBuilder.Next(d => d.AfterSetException, e, this);
        }


        [UsedImplicitly]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(_continuation);

            if (awaiter is IBreakableAwaiter breakableAwaiter)
            {
                _RoutineBuilder.Next(d => d.CurrentAwait, StackTraceHolder.New(3), this);
                _innerAwaiter = breakableAwaiter;
                TryInit();
            }
            else
                Asr.Fail("passed unbreakable awaiter");
        }


        void TryInit()
        {
            if (_cached == null)
                _cached = () => this._innerAwaiter?.Break(RoutineStoppedException.Empty);

            Task.Scope.OnDispose(_cached);
        }

        Action _cached;
        int _count;

        [SecuritySafeCritical, UsedImplicitly]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine =>
            AwaitOnCompleted(ref awaiter, ref stateMachine);

        [UsedImplicitly]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _continuation = stateMachine.MoveNext;
            _continuation.Invoke();
        }


        [UsedImplicitly]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _continuation = stateMachine.MoveNext;
        }
    }
}