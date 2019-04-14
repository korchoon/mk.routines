using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Pooling;
using Lib.Utility;
using Utility;
using Utility.AssertN;

namespace Lib.Async
{
    public class SingleAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        Action _continuation;
        bool _stopRequested;

        public SingleAwaiter()
        {
            _continuation = Empty.Action();
        }

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {
            if (_stopRequested) throw RoutineStoppedException.Empty;
        }

        public void UnsafeOnCompleted(Action moveNext)
        {
            if (IsCompleted)
            {
                moveNext();
                return;
            }

            _continuation = moveNext;
        }


        void INotifyCompletion.OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        internal bool OneOff()
        {
            Dispose();
            return false;
        }

        void Break()
        {
            _stopRequested = true;
            Dispose();
        }

        internal void Dispose()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            var m = _continuation ?? Empty.Action();
            _continuation = Empty.Action();
            m();
        }

        public void BreakOn(IScope scope)
        {
            scope.OnDispose(Break);
        }
    }

    public class SingleAwaiter<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        bool _stopRequested;
        Option<T> _result;
        Action _continuation;

        public SingleAwaiter(ISub<T> ex)
        {
            ex.OnNext(OneOff);

            bool OneOff(T msg)
            {
                _result = msg;
                Dispose();
                return false;
            }
        }

        [UsedImplicitly]
        public T GetResult()
        {
            if (_stopRequested) throw RoutineStoppedException.Empty;

            if (!_result.TryGet(out var res))
                Asr.Fail("Tried to break & get result of Routine<T>");

            return res; //todo
        }

        [UsedImplicitly] public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        public void UnsafeOnCompleted(Action moveNext)
        {
            if (IsCompleted)
            {
                moveNext();
                return;
            }

            _continuation = moveNext;
        }

        void Break()
        {
            _stopRequested = true;
            Dispose();
        }

        void Dispose()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            var m = _continuation ?? Empty.Action();
            _continuation = Empty.Action();
            m();
        }

        public void BreakOn(IScope scope)
        {
            scope.OnDispose(Break);
        }
    }
}