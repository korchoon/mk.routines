// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Utility;
using Utility.Asserts;

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
        IErrorScope<Exception> _onErr;

        [Obsolete("Use GetScope instead")]
        public IScope Scope(IScope breakOn) => GetScope(breakOn);

        public IScope GetScope(IScope breakOn)
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
            StopImmediately = new CatchStack(out _onErr);
            PubDispose = React.Scope(out _scope);
            _scope.OnDispose(InnerDispose);
            _onErr.OnDispose(_ => PubDispose.Dispose());
        }


        void InnerDispose()
        {
            _isCompleted = true;
            PubDispose.Dispose();
            RoutineUtils.MoveNextAndClear(ref _moveAllAwaiters);
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

        #region Static API

        public static SelfScope SelfScope() => new SelfScope();
        public static SelfDispose SelfDispose() => new SelfDispose();

        #endregion

        public class Awaiter : ICriticalNotifyCompletion, IBreakableAwaiter
        {
            Routine _awaitableTask;
            Action _continuation;
            Option<Exception> _exception;

            internal Awaiter(Routine par, IErrorScope<Exception> onErr, ref Action onMoveNext)
            {
                _Routine_Awaiter.Register(this);
                _awaitableTask = par;
                _continuation = Empty.Action();
                onErr.OnDispose(_DisposeWith);
                onMoveNext += () => RoutineUtils.MoveNextAndClear(ref _continuation);
            }

            void _DisposeWith(Exception err)
            {
                _exception = err;
                RoutineUtils.MoveNextAndClear(ref _continuation);
            }

            [UsedImplicitly] public bool IsCompleted => _awaitableTask._isCompleted;

            [UsedImplicitly]
            public void GetResult()
            {
                if (_exception.TryGet(out var err))
                {
                    if (err is RoutineStoppedException) throw err;

                    throw new Exception("See Inner", err);
                }
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
                RoutineUtils.MoveNextAndClear(ref _continuation);
            }
        }


        public class _Routine : DebugTracer<_Routine, Routine>
        {
            public Action<StackTraceHolder> Ctor;
            public Action Dispose;
            public Action<IScope> SubscribeToScope;
            public Action<IScope> SetScope;
            public Action<Awaiter> GetAwaiter;
        }

        public class _Routine_Awaiter : DebugTracer<_Routine_Awaiter, Awaiter>
        {
            public Action AfterBreak;
            public Action GetResult;
            public Action<Exception> Thrown;
            public Action<StackTraceHolder> OnCompleteImmediate;
            public Action<StackTraceHolder> OnCompleteLater;
        }
    }
}