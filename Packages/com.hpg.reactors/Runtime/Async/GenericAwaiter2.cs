using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
    public class GenericAwaiter2 : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        readonly IScope _scope;
        readonly IDisposable _dispose;
        ISub _continuations;

        public void BreakInner()
        {
            if (IsCompleted)
                return;
            
            _dispose.Dispose();
        }

        public GenericAwaiter2(IScope onNext, IScope scope, IDisposable dispose)
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
            onNext.OnDispose(() =>
            {
                if (IsCompleted)
                    return;

                p.Next();
                IsCompleted = true;
                dispose.Dispose();
            });

            _continuations = s;
        }

        public GenericAwaiter2(ISub onNext, IScope scope, IDisposable dispose)
        {
            _scope = scope;
            _dispose = dispose;
            if (scope.Completed)
            {
                IsCompleted = true;
                return;
            }
            
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

    public class GenericAwaiter2<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        readonly IScope _scope;
        readonly IDisposable _dispose;
        ISub _continuations;
        Option<T> _result;

        public void BreakInner()
        {
            _dispose.Dispose();
        }

        public GenericAwaiter2(ISub<T> onNext, IScope scope, IDisposable dispose)
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
            onNext.OnNext(res =>
            {
                if (IsCompleted)
                    return;

                _result = res;
                
                p.Next();
                IsCompleted = true;
                dispose.Dispose();
            }, scope);

            _continuations = s;
        }


        [UsedImplicitly] public bool IsCompleted { get; private set; }

        [UsedImplicitly]
        public T GetResult()
        {
            _result.GetOrFail(out var res);
            return res;
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