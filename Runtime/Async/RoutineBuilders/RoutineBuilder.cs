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
    public class RoutineBuilder
    {
        Action _continuation;
        IBreakableAwaiter _innerAwaiter;
        Action _cached;

        RoutineBuilder()
        {
            _RoutineBuilder.Register(this);

            Task = new Routine();
            _RoutineBuilder.Next(trc => trc.CtorTrace, StackTraceHolder.New(3), this);
        }

        [UsedImplicitly]
        public static RoutineBuilder Create()
        {
            return new RoutineBuilder();
        }

        [UsedImplicitly] public Routine Task { get; private set; }

        [UsedImplicitly]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _continuation = stateMachine.MoveNext;
            _continuation.Invoke();
        }


        [UsedImplicitly]
        public void SetResult()
        {
            Task.PubDispose.Dispose();
            _RoutineBuilder.Next(d => d.AfterSetResult, this);
        }


        [UsedImplicitly]
        public void SetException(Exception e)
        {
            Task.StopImmediately.DisposeWith(e);

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

            Task._scope.OnDispose(_cached);
        }


        [SecuritySafeCritical, UsedImplicitly]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine =>
            AwaitOnCompleted(ref awaiter, ref stateMachine);


        [UsedImplicitly]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _continuation = stateMachine.MoveNext;
        }
    }

    public class _RoutineBuilder : DebugTracer<_RoutineBuilder, object>
    {
        public Action<StackTraceHolder> CtorTrace;
        public Action<StackTraceHolder> CurrentAwait;
        public Action AfterSetResult;
        public Action<Exception> AfterSetException;
        public Action<IBreakableAwaiter> AwaitOnCompleted;
    }
}