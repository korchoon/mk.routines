﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using Utility;
using Utility.AssertN;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder<>))]
    public sealed class Routine<T>
    {
        internal IDisposable _dispose;
        internal IScope Scope;
        internal CatchStack PubErr;
        internal IScope<Exception> _onErr;
        Action _moveAllAwaiters;
        bool _isCompleted;

        internal Option<T> _res;

        internal Routine()
        {
            _moveAllAwaiters = Empty.Action();
            PubErr = new CatchStack(out _onErr);
            _dispose = React.Scope(out Scope);
            Scope.OnDispose(_InnerDispose);
            _onErr.OnDispose(_ => _dispose.Dispose());

            void _InnerDispose()
            {
                _isCompleted = true;
                Utils.MoveNextAndClear(ref _moveAllAwaiters);
                _dispose.Dispose();
            }
        }

        [UsedImplicitly]
        public Awaiter GetAwaiter()
        {
            return new Awaiter(this, _onErr, ref _moveAllAwaiters);
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
                onMoveNext += () => Utils.MoveNextAndClear(ref _continuation);
            }

            void _DisposeWith(Exception err)
            {
                _exception = err;
                Utils.MoveNextAndClear(ref _continuation);
            }

            [UsedImplicitly] public bool IsCompleted => _awaitableTask._isCompleted;

            [UsedImplicitly]
            public T GetResult()
            {
                if (_exception.TryGet(out var err)) throw err;
                if (_awaitableTask._res.TryGet(out var result)) return result;

                Asr.Fail("default return value");
                return default;
            }

            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                {
                    continuation.Invoke();
                    return;
                }

                _continuation = continuation;
            }

            public void UnsafeOnCompleted(Action continuation) => ((INotifyCompletion) this).OnCompleted(continuation);

            public void Break(Exception e)
            {
                if (_exception.HasValue) return;

                _exception = e;
                _awaitableTask.PubErr.DisposeWith(e);
                Utils.MoveNextAndClear(ref _continuation);
            }
        }
    }
}