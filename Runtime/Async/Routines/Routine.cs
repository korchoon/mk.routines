using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
    [AsyncMethodBuilder(typeof(RoutineBuilder))]
    public sealed class Routine : IDisposable
    {
        internal IScope _scope;
        internal IDisposeWith<Exception> StopImmediately;
        Action _moveAllAwaiters;
        bool _isCompleted;
        internal IDisposable PubDispose;
        internal IScope<Exception> _onErr;

        public IScope Scope(IScope breakOn)
        {
            breakOn.OnDispose(() => StopImmediately.DisposeWith(RoutineStoppedException.Empty));
            breakOn.OnDispose(PubDispose.Dispose);
            return _scope;
        }

        internal Routine()
        {
            _Routine.Register(this);
            _Routine.Next(dr => dr.Ctor, StackTraceHolder.New(1), this);

            _moveAllAwaiters = Empty.Action();
            StopImmediately = new CatchQueue(out _onErr);
            PubDispose = React.Scope(out _scope);
            _scope.OnDispose(InnerDispose);
            _onErr.OnDispose(_ => PubDispose.Dispose());

//            DR.Next(dr => dr.SetScope, _scope, this);
        }


        void InnerDispose()
        {
            _isCompleted = true;
            PubDispose.Dispose();
            Utils.MoveNextAndClear(ref _moveAllAwaiters);
            _Routine.Next(dr => dr.Dispose, this);
        }

        void IDisposable.Dispose()
        {
            StopImmediately.DisposeWith(RoutineStoppedException.Empty);
            PubDispose.Dispose();
        }

        [UsedImplicitly]
        public Awaiter GetAwaiter()
        {
            var res = new Awaiter(this, _onErr, ref _moveAllAwaiters);
            _Routine.Next(d => d.GetAwaiter, res, this);
            return res;
        }

        public class Awaiter : ICriticalNotifyCompletion, IBreakableAwaiter
        {
            Routine _awaitableTask;
            Action _continuation;
            Option<Exception> _exception;

            public Awaiter(Routine par, IScope<Exception> onErr, ref Action onMoveNext)
            {
                _Routine_Awaiter.Register(this);
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
            public void GetResult()
            {
                if (_exception.TryGet(out var err)) throw err;
            }


            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                {
                    _Routine_Awaiter.Next(da => da.OnCompleteImmediate, StackTraceHolder.New(1), this);
                    continuation.Invoke();
                    return;
                }

                _Routine_Awaiter.Next(da => da.OnCompleteLater, StackTraceHolder.New(1), this);
                _continuation = continuation;
            }

            public void UnsafeOnCompleted(Action continuation) => ((INotifyCompletion) this).OnCompleted(continuation);

            public void Break(Exception e)
            {
                if (_exception.HasValue) return;
                _exception = e;
                _awaitableTask.StopImmediately.DisposeWith(e);
                Utils.MoveNextAndClear(ref _continuation);
            }
        }
    }
}