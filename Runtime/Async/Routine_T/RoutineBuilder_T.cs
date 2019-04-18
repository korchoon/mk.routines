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
        IAsyncStateMachine _stateMachine;

        RoutineBuilder()
        {
            Task = new Routine<T>();
        }

        [UsedImplicitly]
        public static RoutineBuilder<T> Create() => new RoutineBuilder<T>();

        [UsedImplicitly] public Routine<T> Task { get; }


        [UsedImplicitly]
        public void SetException(Exception e)
        {
            Task.IsCompleted = true;
            Task.PubErr.DisposeWith(e);
            Task.MoveNext = Empty.Action();
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
            Task.IsCompleted = true;
            Task.Complete(value);
            Task.MoveNext.Invoke();
            Task.MoveNext = Empty.Action();
            Task.Dispose();
        }

        [UsedImplicitly]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is IBreakableAwaiter braw)
                braw.BreakOn(Task.Scope);
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