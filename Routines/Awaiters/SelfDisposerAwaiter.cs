// ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Mk.Routines {
    [Serializable]
    public class AttachedRoutines {
        [SerializeReference] internal List<IRoutine> Routines = new List<IRoutine> ();

        public void Attach (Action action) {
            Attach(Routine.FromActions (action));
        }
        
        public void Attach (IRoutine r) {
            Routines.Add (r);
            r.Update (); // todo ?
        }

        public void AttachScoped (IRoutine r, IRollback rollback) {
            Attach (r);
            rollback.Defer (() => {
                r.Break ();
                Detach (r);
            });
        }

        public void Detach (IRoutine r) {
            Routines.Remove (r);
        }
    }
    
    public class SelfParallelAwaiter : IRoutine, ICriticalNotifyCompletion {
        internal AttachedRoutines Value;
        public void UpdateParent () { }
        public void Update () { }
        public void Break () { }

        #region async

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public SelfParallelAwaiter GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public bool IsCompleted { [UsedImplicitly] get; private set; }

        [UsedImplicitly]
        public AttachedRoutines GetResult () => Value;

        [UsedImplicitly]
        public void OnCompleted (Action continuation) => IsCompleted = true;

        [UsedImplicitly]
        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        #endregion
    }

    public class SelfDisposerAwaiter : IRoutine, ICriticalNotifyCompletion {
        internal Rollback Value;
        public void UpdateParent () { }
        public void Update () { }
        public void Break () { }

        #region async

        CalledOnceGuard _guard;

        [UsedImplicitly]
        public SelfDisposerAwaiter GetAwaiter () {
            _guard.Assert ();
            return this;
        }

        [UsedImplicitly]
        public bool IsCompleted { [UsedImplicitly] get; private set; }

        [UsedImplicitly]
        public Rollback GetResult () => Value;

        [UsedImplicitly]
        public void OnCompleted (Action continuation) => IsCompleted = true;

        [UsedImplicitly]
        public void UnsafeOnCompleted (Action continuation) => OnCompleted (continuation);

        #endregion
    }
}