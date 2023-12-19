using System;
using Mk.Debugs;
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

        void IDisposable.Dispose () {
            IsCompleted = true;
            BeforeDispose?.Invoke ();
            _rollback?.Dispose ();
        }

        void IRoutine.UpdateParent () {
            if (Utils.TrySetNull (ref _continuation, out var c)) {
                c.Invoke ();
            }
        }

        void IRoutine.Tick () {
            if (IsCompleted) {
                return;
            }

            if (DoWhile != null && !DoWhile.Invoke ()) {
                this.DisposeAndUpdateParent ();
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

        Action _continuation;

        public ActionAwaiter GetAwaiter () {
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