using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.Async.Debugger;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
    public class GenericAwaiter2 : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        readonly IScope _scope;
        readonly Action _break;
        ISub _continuations;
        Action _continuation;
        Action _unsub;

        public void BreakInner()
        {
            _break.Invoke();
        }

        public void Unsub()
        {
            _unsub.Invoke();
        }

        public GenericAwaiter2(IScope scope, Action onBreakInner)
        {
            _scope = scope;
            if (_scope.Completed)
            {
                _unsub = Empty.Action();
                return;
            }

            _break = onBreakInner;

            var (p, s) = _scope.PubSub();
            _scope.OnDispose(p.Next);
            _continuations = s;
            _unsub = () => _scope.Unsubscribe(p.Next);
        }

        [UsedImplicitly] public bool IsCompleted => _scope.Completed;

        [UsedImplicitly]
        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                continuation.Invoke();
                return;
            }

            _continuations.OnNext(continuation, _scope);
        }

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
    }

    public class GenericAwaiter2<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        readonly GenericAwaiter2 _aw;
        Func<T> _getResult;

        public GenericAwaiter2(GenericAwaiter2 aw, Func<T> getResult)
        {
            _aw = aw;
            _getResult = getResult;
        }

        [UsedImplicitly]
        public T GetResult() => _getResult.Invoke();

        [UsedImplicitly] public bool IsCompleted => _aw.IsCompleted;

        public void OnCompleted(Action continuation) => _aw.OnCompleted(continuation);
        public void UnsafeOnCompleted(Action continuation) => _aw.UnsafeOnCompleted(continuation);
        public void BreakInner() => _aw.BreakInner();

        public void Unsub()
        {
            _aw.Unsub();
        }
    }
}