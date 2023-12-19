// ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

#if DEBUG
// #define MK_TRACE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Mk.Debugs;

namespace Mk.Routines {
#line hidden

    [PublicAPI]
#if MK_TRACE
    [Serializable]
#endif
    [AsyncMethodBuilder (typeof (RoutineBuilder))]
    public sealed class Routine : IRoutine, ICriticalNotifyCompletion {
        internal Action Start;
        public string __Await;
        internal Option<AttachedRoutines> AttachedRoutines;
        internal Rollback TaskRollback;
        internal IRoutine CurrentAwaiter;

        Action _continuation;

        internal Routine () { }

        public void Dispose () {
            if (IsCompleted) return;

            Start = null;
            IsCompleted = true;
            if (AttachedRoutines.TryGet (out var attachedRoutines)) {
                AttachedRoutines = default;
                for (var i = attachedRoutines.Routines.Count - 1; i >= 0; i--)
                    attachedRoutines.Routines[i].Dispose ();
            }

            if (Utils.TrySetNull (ref CurrentAwaiter, out var buf)) buf.Dispose ();
            TaskRollback?.Dispose ();
#if MK_TRACE
            __Await = $"[Interrupted at] {__Await}";
#endif
        }

        void IRoutine.UpdateParent () {
            if (AttachedRoutines.TryGet (out var attachedRoutines))
                for (var i = 0; i < attachedRoutines.Routines.Count; i++)
                    attachedRoutines.Routines[i].UpdateParent ();

            if (Utils.TrySetNull (ref _continuation, out var c)) c.Invoke ();
        }

        public void Tick () {
            if (IsCompleted) return;

            if (Utils.TrySetNull (ref Start, out var start)) {
                start.Invoke ();
                return;
            }

            if (AttachedRoutines.TryGet (out var attachedRoutines))
                for (var i = 0; i < attachedRoutines.Routines.Count; i++) {
                    if (IsCompleted) return;
                    attachedRoutines.Routines[i].Tick ();
                }

            CurrentAwaiter?.Tick ();
        }

        #region async

        [UsedImplicitly]
        public bool IsCompleted { get; private set; }

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public Routine GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public void GetResult () {
            // Asr.IsFalse(Interrupted);
        }

        public void OnCompleted (Action continuation) {
            if (IsCompleted) {
                continuation.Invoke ();
                return;
            }

            Asr.IsTrue (_continuation == null);
            _continuation = continuation;
        }

        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        #endregion

        #region static api

        static InfLoopSafety _safety = new InfLoopSafety (10000);

        public static async Routine While (Func<bool> enterIf, Action<IRollback> start = null, Action update = null, Func<IRoutine> routine = default) {
            _safety.Check ();
            if (!enterIf.Invoke ()) {
#if PREVENT_INF_LOOP
                 await Routine.Yield;
#endif
                return;
            }

            if (routine != null) {
                await routine.Invoke ().Configure (doWhile: enterIf, onStart: start, afterUpdate: update);
                await When (() => !enterIf ());
            }
            else {
                var rollback = await GetRollback ();
                start?.Invoke (rollback);
                await When (() => !enterIf ()).Configure (afterUpdate: update);
            }
        }

        static class WithResult<T> {
            public static Routine<T> Completed {
                get {
                    if (_emptyInstance != null) {
                        return _emptyInstance;
                    }

                    _emptyInstance = empty ();
                    _emptyInstance.Tick ();
                    Asr.IsTrue (_emptyInstance.IsCompleted);
                    return _emptyInstance;
#pragma warning disable 1998
                    static async Routine<T> empty () {
                        return default;
                    }
#pragma warning restore 1998
                }
            }

            static Routine<T> _emptyInstance;
        }

        public static Routine<T> CompletedWithValue<T> (T value) {
            var t = WithResult<T>.Completed;
            t._cached = value;
            return t;
        }

        public static TryAwaiter WaitSeconds (float value) => value.GetAwaiter ();

        [MustUseReturnValue]
        public static ActionAwaiter FromActions (
            Action update,
            Action dispose = default,
            Func<bool> doWhile = default,
            Action<IRollback> onStart = default)
            => new ActionAwaiter () {
                OnUpdate = update,
                DoWhile = doWhile,
                BeforeDispose = dispose,
                OnStart = onStart
            };

        [MustUseReturnValue]
        public static TryAwaiter When (Func<bool> p, Action onDispose = default) => new TryAwaiter () { TryGet = p, OnDispose = onDispose };

        [MustUseReturnValue]
        public static TryAwaiter<T> WhenGet<T> (Func<Option<T>> p) => new TryAwaiter<T> () { TryGet = p };

        [MustUseReturnValue]
        public static FuncAwaiter<T> Branch<T> (Func<T> p) where T : IOptional => new FuncAwaiter<T> () { TryGet = p };

        [MustUseReturnValue]
        public static AnyAwaiter WhenAny (params IRoutine[] args) => new AnyAwaiter () { Args = args };

        [MustUseReturnValue]
        public static AnyAwaiter<T> WhenAny_T<T> (params IRoutine<T>[] args) => new AnyAwaiter<T> () { Args = args.Select (a => (false, a)).ToArray () };

        [MustUseReturnValue]
        public static AnyAwaiter WhenAny (IReadOnlyList<IRoutine> args) => new AnyAwaiter () { Args = args };

        [MustUseReturnValue]
        public static FirstAwaiter<T> WhenFirst<T> (IReadOnlyList<IRoutine<T>> args) => new FirstAwaiter<T> () { Args = args };

        static Option<T> TryGet<T> (IRoutine<T> r) {
            if (!r.IsCompleted) {
                return default;
            }

            return r.GetResult ();
        }

        [MustUseReturnValue]
        public static FirstAwaiter<T> WhenFirst<T> (params IRoutine<T>[] args) => new FirstAwaiter<T> () { Args = args };

        [MustUseReturnValue]
        public static FirstAwaiter WhenFirst (params IRoutine[] args) {
            return new FirstAwaiter () { Args = args };
        }

        [MustUseReturnValue]
        public static AllAwaiter WhenAll (params IRoutine[] args) => new AllAwaiter () { Args = args };

        [MustUseReturnValue]
        public static AllAwaiter WhenAll (IReadOnlyList<IRoutine> args) => new AllAwaiter () { Args = args };

        [MustUseReturnValue]
        public static SelfRollbackAwaiter GetRollback () => new SelfRollbackAwaiter ();

        [MustUseReturnValue]
        public static SelfParallelAwaiter GetParallel () => new SelfParallelAwaiter ();

        public static YieldAwaiter Yield => YieldAwaiter.Pool.Pop ();

        public static ForeverAwaiter Forever { get; } = ForeverAwaiter.Instance;

        public static Routine Completed {
            get {
                if (_emptyInstance != null) {
                    return _emptyInstance;
                }

                _emptyInstance = empty ();
                _emptyInstance.Tick ();
                Asr.IsTrue (_emptyInstance.IsCompleted);
                return _emptyInstance;
#pragma warning disable 1998
                static async Routine empty () { }
#pragma warning restore 1998
            }
        }

        static Routine _emptyInstance;

        #endregion
    }
#line default
}