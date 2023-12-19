using System;
using System.Threading;
using JetBrains.Annotations;
using Mk.Debugs;
using Mk.Routines;
using UnityEngine;


namespace Mk {
    public interface IRollback {
        bool IsDisposed { get; }
        void Defer (Action action);
        void RemoveDeferred (Action action);
    }

    /// <summary>
    /// Container to defer actions to be executed on Dispose (resource cleanup, cancel side effects)
    /// </summary>
    public class Rollback : IRollback, IDisposable {
        public bool IsDisposed { get; private set; }
        readonly object lockObject;
        Action deferredActions;

        public Rollback () {
            lockObject = new object ();
            IsDisposed = false;
        }

        public static Rollback Root () {
            return new Rollback ();
        }

        /// <summary>
        /// Defer action to be executed upon Dispose
        /// </summary>
        /// <param name="action"></param>
        public void Defer (Action action) {
            lock (lockObject) {
                Asr.IsFalse (IsDisposed, "This rollback is disposed. Cannot defer action.");
                deferredActions = action + deferredActions; // stack-like order
            }
        }

        /// <summary>
        /// Remove deferred action so it won't be executed
        /// </summary>
        /// <param name="action"></param>
        public void RemoveDeferred (Action action) {
            lock (lockObject) {
                Asr.IsFalse (IsDisposed, "This rollback is disposed. Cannot remove deferred action.");
#if UNITY_EDITOR
                var countPrev = deferredActions.GetInvocationList ().Length;
                deferredActions -= action;
                if (deferredActions != null)
                    if (deferredActions.GetInvocationList ().Length == countPrev) {
                        Asr.Fail ("Trying to remove action which wasn't deferred");
                    }
#else
			deferredActions -= action;
#endif
            }
        }

        /// <summary>
        /// Executes deferred actions in reverse order
        /// </summary>
        public void Dispose () {
            lock (lockObject) {
                if (IsDisposed) {
                    return;
                }

                IsDisposed = true;
                if (deferredActions != null) {
                    foreach (var d in deferredActions.GetInvocationList ()) {
                        var action = (Action)d;
                        try {
                            action ();
                        }
                        catch (Exception e) {
                            Debug.LogException (e);
                        }
                    }

                    deferredActions = null;
                }
            }
        }
    }

    public static class RollbackExtensions {
        /// <summary>
        /// Child CancellationTokenSource. Disposing cts won't dispose Rollback
        /// </summary>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public static CancellationTokenSource GetCancellationTokenSource (this IRollback rollback) {
            // todo check
            var source = new CancellationTokenSource ();
            rollback.Defer (source.Dispose);
            rollback.Defer (source.Cancel);
            return source;
        }

        /// <summary>
        /// Child cancellation token
        /// </summary>
        /// <param name="rollback"></param>
        /// <returns></returns>
        public static CancellationToken GetCancellationToken (this IRollback rollback) {
            // todo check
            var source = new CancellationTokenSource ();
            rollback.Defer (source.Dispose);
            rollback.Defer (source.Cancel);
            return source.Token;
        }

        /// <summary>
        /// Opens a child rollback which will be disposed with current one. Allows cascade disposals
        /// </summary>
        /// <returns></returns>
        [MustUseReturnValue]
        public static Rollback OpenRollback (this IRollback parentRollback) {
            Asr.IsFalse (parentRollback.IsDisposed, "This rollback is disposed. Cannot open rollback from it.");
            var result = new Rollback ();
            parentRollback.Defer (result.Dispose);
            result.Defer (cancelDisposingChild);
            return result;

            void cancelDisposingChild () {
                if (!parentRollback.IsDisposed) {
                    parentRollback.RemoveDeferred (result.Dispose);
                }
            }
        }
    }
}