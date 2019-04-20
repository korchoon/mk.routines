using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using UnityEngine.Assertions;
using Utility;
using Utility.AssertN;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder<>))]
    public sealed class Routine<T> : IAwait
    {
#if DEBUG_TRACE
        internal RoutineDebug _debug;
        public override string ToString() => _builder.ToString();

        [ShowInInspector, HideLabel, PropertyOrder(1)]
        internal string Name => _builder.ToString();
#endif
        IDisposable _dispose;
        internal IScope Scope;
        internal IDisposeWith<Exception> PubErr;
        IScope<Exception> _onErr;
        public Action MoveNext;

        Option<T> _res;

        internal Routine()
        {
            MoveNext = Empty.Action();
            _dispose = React.Scope(out Scope);
            PubErr = ReactExperimental.ErrScope(out _onErr);
        }


        internal void Dispose()
        {
            if (IsCompleted) return;
            Assert.IsFalse(IsCompleted);
            MoveNext.Invoke();
            IsCompleted = true;
            MoveNext = Empty.Action();
            _dispose.Dispose();
        }

        [UsedImplicitly]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this, _onErr, ref MoveNext);
        }

        Option<T> GetResult() => _res;

        public bool IsCompleted { get; internal set; }

        static async Routine<Option<T>> _Internal(Routine<T> routine) => await routine;

        public void Complete(T value)
        {
            _res = value;
        }


        public class Awaiter : ICriticalNotifyCompletion, IBreakableAwaiter
        {
            Routine<T> _awaitableTask;
            Action _continuation;
            Option<Exception> _exception;

            public Awaiter(Routine<T> par, IScope<Exception> onErr, ref Action onMoveNext)
            {
                _awaitableTask = par;
                _continuation = Empty.Action();
                onErr.OnDispose(_DisposeWith);
                onMoveNext = Step;
            }

            void Step()
            {
                _continuation();
            }

            void _DisposeWith(Exception err)
            {
                _exception = err;
                var m = _continuation;
                _continuation = Empty.Action();
                m.Invoke();
            }

            [UsedImplicitly] public bool IsCompleted => _awaitableTask.IsCompleted;

            [UsedImplicitly]
            public T GetResult()
            {
                if (_exception.TryGet(out var err))
                    throw err;

                if (_awaitableTask.GetResult().TryGet(out var result)) return result;

                Asr.Fail("default return value");
                return default;
            }

            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                {
                    continuation();
                    return;
                }

                _continuation = continuation;
            }

            public void UnsafeOnCompleted(Action continuation) => ((INotifyCompletion) this).OnCompleted(continuation);

            public void Break()
            {
                _awaitableTask._dispose.Dispose();
            }
        }
    }
}