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
    public class RoutineBuilder2
    {
        Action _continuation;
        IAsyncStateMachine _stateMachine;
        IBreakableAwaiter _break;

        RoutineBuilder2()
        {
            Task = new Routine();
            Task._scope.OnDispose(BreakCur);
        }

        void BreakCur()
        {
            var b = _break;
            _break = null;
            b?.Break();
        }

        [UsedImplicitly]
        public static RoutineBuilder2 Create() => new RoutineBuilder2();

        [UsedImplicitly] public Routine Task { get; }


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
        public void SetResult()
        {
            Task.Dispose();
        }

        [UsedImplicitly]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is IBreakableAwaiter braw)
            {
//                Task._scope.OnDispose(braw.Break);

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

            if (ScheduleRunner.WantsQuit) return; // todo 

            _continuation();
        }


        [UsedImplicitly]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}