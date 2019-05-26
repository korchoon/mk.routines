using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
    public class GenericAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        ISub _continuations;
        IScope _innerScope;
        IDisposable _dispose;

        public void BreakInner()
        {
            _dispose.Dispose();
        }

        public GenericAwaiter(ISub continuations)
        {
            _dispose = React.Scope(out _innerScope);
            
            
            var (pub, sub) = _innerScope.PubSub();
            _innerScope.OnDispose(pub.Next);

            _continuations = sub;
            continuations.OnNext(() =>
            {
                pub.Next();
                _dispose.Dispose();
            }, _innerScope);
        }

        [UsedImplicitly] public bool IsCompleted => _innerScope.Completed;

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
            
            _continuations.OnNext(continuation, _innerScope);
        }

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
    }

    public class GenericAwaiter<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        public IScope InnerTree { get; }

        ISub _onResultVoid;
        Option<T> _result;

        public GenericAwaiter(ISub<T> onResult)
        {
#if M_LATER
             var (pub, sub) = InnerTree.PubSub();
            _onResultVoid = sub;
            onResult.OnNext(msg =>
            {
                _result = msg;
                pub.Next();
            }, InnerTree);
#endif
        }

        public void BreakInner()
        {
        }

        [UsedImplicitly] public bool IsCompleted => InnerTree.Completed;

        [UsedImplicitly]
        public T GetResult()
        {
            _result.GetOrFail(out var res);
            return res;
        }

        public void OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
            _onResultVoid.OnNext(continuation, InnerTree);
        }

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
    }

    internal class _GenericAwaiter : DebugTracer<_GenericAwaiter, GenericAwaiter>
    {
    }
}