using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Mk.Debugs;

namespace Mk.Routines {
#line hidden
    public class AnyAwaiter : IRoutine<IReadOnlyList<bool>>, ICriticalNotifyCompletion {
        public IReadOnlyList<IRoutine> Args;
        bool[] _result;
        Action _continuation;

        public void Dispose () {
            if (IsCompleted) return;
            IsCompleted = true;
            foreach (var u in Args) u.Dispose ();
        }

        public void UpdateParent () {
            if (Utils.TrySetNull (ref _continuation, out var c)) c.Invoke ();
        }

        public void Tick () {
            if (IsCompleted) return;

            var any = false;
            for (var index = 0; index < Args.Count; index++) {
                var u = Args[index];
                u.Tick ();
                _result[index] = u.IsCompleted;
                any |= u.IsCompleted;
            }

            if (any) {
                this.DisposeAndUpdateParent ();
            }
        }

        #region async

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public AnyAwaiter GetAwaiter () {
            _result = new bool[this.Args.Count];
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public IReadOnlyList<bool> GetResult () {
            return _result;
        }

        [UsedImplicitly]
        public void OnCompleted (Action continuation) {
            if (IsCompleted) {
                continuation.Invoke ();
                return;
            }

            Asr.IsTrue (_continuation == null);
            _continuation = continuation;
        }

        [UsedImplicitly]
        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        [UsedImplicitly]
        public bool IsCompleted { get; private set; }

        #endregion
    }
    public class AnyAwaiter<T> : IRoutine<(bool,IRoutine<T>)[]>, ICriticalNotifyCompletion {
        public (bool, IRoutine<T>)[] Args;
        Action _continuation;

        public void Dispose () {
            if (IsCompleted) return;
            IsCompleted = true;
            foreach (var u in Args) u.Item2.Dispose ();
        }

        public void UpdateParent () {
            if (Utils.TrySetNull (ref _continuation, out var c)) c.Invoke ();
        }

        public void Tick () {
            if (IsCompleted) return;

            var any = false;
            for (var index = 0; index < Args.Length; index++) {
                ref var result = ref Args[index];
                result.Item2.Tick ();
                result.Item1 = result.Item2.IsCompleted;
                any |= result.Item2.IsCompleted;
            }

            if (any) {
                this.DisposeAndUpdateParent ();
            }
        }

        #region async

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public AnyAwaiter<T> GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public (bool,IRoutine<T>)[] GetResult () {
            return Args;
        }

        [UsedImplicitly]
        public void OnCompleted (Action continuation) {
            if (IsCompleted) {
                continuation.Invoke ();
                return;
            }

            Asr.IsTrue (_continuation == null);
            _continuation = continuation;
        }

        [UsedImplicitly]
        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        [UsedImplicitly]
        public bool IsCompleted { get; private set; }

        #endregion
    }
#line default
}