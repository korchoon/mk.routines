using System;
using System.Runtime.CompilerServices;

namespace Mk.Routines {
#line hidden
    public class ActionAwaiter : IRoutine, ICriticalNotifyCompletion {
        public Func<bool> DoWhile;
        public Action OnUpdate;
        public Action BeforeDispose;
        public Action<IRollback> OnStart;
        Rollback _rollback;
        bool _started;

        public ActionAwaiter () { }

        void IRoutine.Break () {
            IsCompleted = true;
            BeforeDispose?.Invoke ();
            _rollback?.Dispose ();
        }

        void IRoutine.UpdateParent () {
            if (Utils.TrySetNull (ref _continuation, out var c)) {
                c.Invoke ();
            }
        }

        void IRoutine.Update () {
            if (IsCompleted) {
                return;
            }

            if (DoWhile != null && !DoWhile.Invoke ()) {
                this.BreakAndUpdateParent ();
                return;
            }

            if (!_started) {
                _started = true;
                if (OnStart != null) {
                    this._rollback = new Rollback ();
                    OnStart.Invoke (_rollback);
                }
            }

            OnUpdate?.Invoke ();
        }

        #region async

        CalledOnceGuard _guard;
        Action _continuation;

        public ActionAwaiter GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        public void GetResult () { }

        public void OnCompleted (Action continuation) {
            if (IsCompleted) {
                continuation.Invoke ();
                return;
            }

            Asr.IsTrue (_continuation == null);
            _continuation = continuation;
        }

        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        public bool IsCompleted { get; private set; }

        #endregion
    }
#line default
}