using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using Utility;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder2))]
    public sealed class Routine : IAwait, IDisposable
    {
        IDisposable _dispose;
        internal IScope _scope;
        internal IDisposeWith<Exception> PubErr;
        IScope<Exception> _onErr;

        public Action MoveNext;

        // to not to forget DisposeOn
        public IScope Scope(IScope breakOn)
        {
            breakOn.OnDispose(Dispose);
            return _scope;
        }

        internal Routine()
        {
            MoveNext = Empty.Action();
            _dispose = React.Scope(out _scope);
            PubErr = ReactExperimental.ErrScope(out _onErr);
        }

        public void Dispose() => _dispose.Dispose();

        [UsedImplicitly]
        public Awaiter GetAwaiter() => new Awaiter(this, _onErr, ref MoveNext);

        public bool IsCompleted { get; internal set; }

        public class Awaiter : ICriticalNotifyCompletion, IBreakableAwaiter
        {
            internal Routine AwaitableTask;
            Action _continuation;
            Option<Exception> _exception;

            public Awaiter(Routine par, IScope<Exception> onErr, ref Action onMoveNext)
            {
                AwaitableTask = par;
                _continuation = Empty.Action();
                onErr.OnDispose(_DisposeWith);
                onMoveNext += Step;
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

            [UsedImplicitly] public bool IsCompleted => AwaitableTask.IsCompleted;

            [UsedImplicitly]
            public void GetResult()
            {
                if (_exception.TryGet(out var err))
                    throw err;
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

            public void BreakOn(IScope scope) => scope.OnDispose(Break);
            public void Break()
            {
                AwaitableTask.Dispose();
            }
        }
    }
}