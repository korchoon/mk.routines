using System;
using System.Runtime.CompilerServices;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
    public class ScopeAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        Action _continuation;
        Option<Exception> _exception;

        public ScopeAwaiter(IScope scope)
        {
            _continuation = Empty.Action();
            scope.OnDispose(Dispose);
        }

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {
            if (_exception.TryGet(out var e)) throw e;
        }

        public void UnsafeOnCompleted(Action moveNext)
        {
            if (IsCompleted)
            {
                moveNext.Invoke(); // todo
                return;
            }

            _continuation = moveNext;
        }


        void INotifyCompletion.OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

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