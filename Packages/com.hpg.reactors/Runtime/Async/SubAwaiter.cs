// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Pooling;
using Lib.Utility;
using Utility;
using Utility.Asserts;

namespace Lib.Async
{
    public class SubAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        SubAwaiter()
        {
        }
        
        Action _continuation;
        Option<Exception> _exception;

        public static SubAwaiter New() => new SubAwaiter {_continuation = Empty.Action()};


        [UsedImplicitly] public bool IsCompleted { get; private set; }

        [UsedImplicitly]
        public void GetResult()
        {
            if (_exception.TryGet(out var err)) throw err;
        }

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action moveNext)
        {
            if (IsCompleted)
            {
                moveNext.Invoke(); // todo
                return;
            }

            _continuation = moveNext;
        }


        [UsedImplicitly]
        void INotifyCompletion.OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        public void Break(Exception e)
        {
            if (_exception.HasValue) return;

            _exception = e;
            _Dispose();
        }

        internal void _Dispose()
        {
            IsCompleted = true;
            RoutineUtils.MoveNextAndClear(ref _continuation);
        }
    }

    public class SubAwaiter<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        Option<T> _result;
        Action _continuation;
        Option<Exception> _exception;
        
//        public Pool<SubAwaiter<T>> Pool = new Pool<SubAwaiter<T>>(() => new SubAwaiter<T>(), );
        
        public SubAwaiter(ISub<T> ex)
        {
            ex.OnNextOnce(OneOff);

            void OneOff(T msg)
            {
                _result = msg;
                Dispose();
            }
        }

        [UsedImplicitly]
        public T GetResult()
        {
            if (_exception.TryGet(out var e)) throw e;

            if (!_result.TryGet(out var res))
                Asr.Fail("Tried to break & get result of Routine<T>");

            return res; //todo
        }

        [UsedImplicitly] public bool IsCompleted { get; private set; }

        [UsedImplicitly]
        public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action moveNext)
        {
            if (IsCompleted)
            {
                moveNext.Invoke(); // todo
                return;
            }

            _continuation = moveNext;
        }

        public void Break(Exception e)
        {
            if (_exception.HasValue) return;

            _exception = e;
            Dispose();
        }

        void Dispose()
        {
            IsCompleted = true;
            RoutineUtils.MoveNextAndClear(ref _continuation);
        }
    }
}