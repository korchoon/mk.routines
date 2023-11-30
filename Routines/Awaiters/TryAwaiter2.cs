using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mk.Routines {
#line hidden
    public class TryAwaiter2<T> : IRoutine<T>, ICriticalNotifyCompletion where T : IOptional {
        public Func<T> TryGet;
        public Action OnDispose;
        Action _continuation;
        T _cached;

        public void Break () {
            if (IsCompleted) return;
            IsCompleted = true;
            OnDispose?.Invoke ();
        }

        void IRoutine.UpdateParent () {
            if (Utils.TrySetNull (ref _continuation, out var c)) c.Invoke ();
        }

        public void Update () {
            if (IsCompleted) return;

            _cached = TryGet.Invoke ();
            if (_cached.HasValue) {
                this.BreakAndUpdateParent ();
            }
        }

        #region async

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public TryAwaiter2<T> GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public bool IsCompleted { get; private set; }

        [UsedImplicitly]
        public T GetResult () => _cached;

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

        #endregion
    }
#line default
}