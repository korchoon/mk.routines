using System;
using System.Runtime.CompilerServices;
using System.Security;
using JetBrains.Annotations;
using Lib.DataFlow;
using Utility;
using Utility.AssertN;

namespace Lib.Async
{
    public class RoutineBuilder<T>
    {
        Action _continuation;
        IAsyncStateMachine _stateMachine;
        IBreakableAwaiter _break;

        RoutineBuilder()
        {
            Task = new Routine<T>();
            Task.Scope.OnDispose(BreakCur);
        }
        void BreakCur()
        {
            var b = _break;
            _break = null;
            b.Break();
        }

        [UsedImplicitly]
        public static RoutineBuilder<T> Create() => new RoutineBuilder<T>();

        [UsedImplicitly] public Routine<T> Task { get; }


        [UsedImplicitly]
        public void SetException(Exception e)
        {
            Task.PubErr.DisposeWith(e);
            Task.Dispose();

            if (e is RoutineStoppedException)
            {
            }
            else
            {
                Dbg.LogException(e);
                SchPub.PubError.Next(e);
            }
        }


        [UsedImplicitly]
        public void SetResult(T value)
        {
            Task.Complete(value);
            Task.MoveNext.Invoke();
            Task.Dispose();
        }

        [UsedImplicitly]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is IBreakableAwaiter braw)
            {
//                BreakCur();
//                Task.Scope.OnDispose(braw.Break);
                _break = braw;
            }
            else
                Asr.Fail("passed unbreakable awaiter");

            awaiter.OnCompleted(_continuation);
        }

        [SecuritySafeCritical, UsedImplicitly]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine =>
            AwaitOnCompleted(ref awaiter, ref stateMachine);

        [UsedImplicitly]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            _stateMachine = stateMachine;
            _continuation = _stateMachine.MoveNext;

            if (ScheduleRunner.WantsQuit) return; // todo dirty hack


            _continuation();
//            Asr.IsNotNull(_yieldOn, $"{this} has no known awaiter will not moveNext");
        }


        [UsedImplicitly]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}