using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mk.Routines {
#line hidden
    public class ProxyAwaiter : IRoutine, ICriticalNotifyCompletion {
        public Func<bool> DoWhile;
        public Action<IRollback> OnStart;
        public Action OnBreak;
        public Action BeforeUpdate;
        public Action AfterUpdate;
        public Action BeforeDispose;
        internal IRoutine Main;
        bool _started;
        Rollback _rollback;

        void IRoutine.Break () {
            IsCompleted = true;
            BeforeDispose?.Invoke ();
            Main.Break ();
            _rollback?.Dispose ();
        }

        void IRoutine.UpdateParent () {
            Main.UpdateParent ();
            if (Utils.TrySetNull (ref _continuation, out var c)) {
                c.Invoke ();
            }
        }

        void IRoutine.Update () {
            if (IsCompleted) {
                return;
            }

            if (DoWhile != null && !DoWhile.Invoke ()) {
                OnBreak?.Invoke ();
                this.BreakAndUpdateParent ();
                return;
            }

            if (!_started) {
                _started = true;
                if (OnStart != null) {
                    Dbg.LineGame ();
                    _rollback = new Rollback ();
                    _rollback.Defer (Main.Break);
                    OnStart.Invoke (_rollback);
                }
            }


            BeforeUpdate?.Invoke ();
            Main.Update ();
            if (Main.IsCompleted) {
                this.BreakAndUpdateParent ();
                return;
            }

            // todo bug: afterUpdate got called after disposal
            AfterUpdate?.Invoke ();
        }

        #region async

        CalledOnceGuard _guard;
        Action _continuation;

        public ProxyAwaiter GetAwaiter () {
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
            // Main.OnCompleted(continuation);
        }

        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        public bool IsCompleted { get; private set; }

        #endregion
    }

    public class ProxyAwaiter<T> : IRoutine<T>, ICriticalNotifyCompletion {
        public Action BeforeUpdateAction;
        public Action BeforeDisposeAction;
        public Action AfterUpdateAction;
        public IRoutine<T> MainRoutine;

        Action _continuation;

        public void Break () {
            IsCompleted = true;
            BeforeDisposeAction?.Invoke ();
            MainRoutine.Break ();
        }

        public void UpdateParent () {
            MainRoutine.UpdateParent ();
            if (Utils.TrySetNull (ref _continuation, out var c)) c.Invoke ();
        }

        public void Update () {
            if (IsCompleted) return;

            BeforeUpdateAction?.Invoke ();
            MainRoutine.Update ();
            if (MainRoutine.IsCompleted) {
                this.BreakAndUpdateParent ();
                return;
            }

            AfterUpdateAction?.Invoke ();
        }

        #region async

        public ProxyAwaiter<T> GetAwaiter () => this;

        public T GetResult () {
            return MainRoutine.GetResult ();
        }

        public void OnCompleted (Action continuation) {
            if (IsCompleted) {
                continuation.Invoke ();
                return;
            }

            Asr.IsTrue (_continuation == null);
            _continuation = continuation;

            // MainRoutine.OnCompleted(continuation);
        }

        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        public bool IsCompleted { get; private set; }

        #endregion
    }
#line default
}