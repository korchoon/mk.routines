using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib.Async
{
    public class GenericAwaiter2 : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        readonly IScope _scope;
        readonly IDisposable _dispose;
        ISub _continuations;

        public void BreakInner()
        {
            _dispose.Dispose();
        }

        static ISub ToSub(IScope scope)
        {
            var (p, s) = scope.PubSub();
            scope.OnDispose(p.Next);
            return s;
        }

        ISub ToSub(ISub<(bool, bool)> branchOf)
        {
            var (p, s) = _scope.PubSub();
            branchOf.OnNext(_ => p.Next(), _scope);
            return s;
        }

        public GenericAwaiter2(ISub onNext, IScope scope, IDisposable dispose)
        {
            _scope = scope;
            _dispose = dispose;
            
            scope.OnDispose(() => IsCompleted = true);

            var (p, s) = scope.PubSub();
            scope.OnDispose(() =>
            {
                if (IsCompleted)
                    return;

                p.Next();
                IsCompleted = true;
            });
            onNext.OnNext(() =>
            {
                if (IsCompleted)
                    return;
                
                p.Next();
                IsCompleted = true;
                dispose.Dispose();
            }, scope);

            _continuations = s;
        }


        [UsedImplicitly] public bool IsCompleted { get; private set; }

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
}