using System;
using System.Runtime.CompilerServices;
using System.Security;
using JetBrains.Annotations;
using Lib.DataFlow;
using Utility.Asserts;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    public class RoutineBuilder
    {
        Action _continuation;
        IBreakableAwaiter _innerAwaiter;

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
            BreakInner();
            Task.PubDispose.Dispose();
            _RoutineBuilder.Next(d => d.AfterSetResult, this);
        }


        [UsedImplicitly]
        public void SetException(Exception e)
        {
            BreakInner();
            Task.StopImmediately.DisposeWith(e);

            if (!(e is RoutineStoppedException))
            {
                Debug.LogException(e);
                SchPub.PubError.Next(e);
            }

            _RoutineBuilder.Next(d => d.AfterSetException, e, this);
        }

        void BreakInner()
        {
            _innerAwaiter?.Break(RoutineStoppedException.Empty);
        }

        [UsedImplicitly]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            switch (awaiter)
            {
                case IBreakableAwaiter breakableAwaiter:
                    awaiter.OnCompleted(_continuation);
                    _RoutineBuilder.Next(d => d.CurrentAwait, StackTraceHolder.New(3), this);
                    _innerAwaiter = breakableAwaiter;
                    break;
                case SelfScopeAwaiter selfScopeAwaiter:
                    selfScopeAwaiter.Value = Task._scope;
                    Asr.IsNotNull(Task._scope);
                    awaiter.OnCompleted(_continuation);
                    break;
                case SelfDisposeAwaiter selfDisposeAwaiter:
                    selfDisposeAwaiter.Value = Task.PubDispose;
                    Asr.IsNotNull(Task.PubDispose);
                    awaiter.OnCompleted(_continuation);
                    break;
                default:
                    Asr.Fail("passed unbreakable awaiter");
                    break;
            }
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
}