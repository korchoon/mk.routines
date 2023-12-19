using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mk.Routines {
    public class ForeverAwaiter : IRoutine, ICriticalNotifyCompletion {
        public static ForeverAwaiter Instance { get; } = new ForeverAwaiter();

        ForeverAwaiter() { }

        public void UpdateParent() { }
        public void Tick() { }
        public void Dispose() { }

        #region async

        [UsedImplicitly]
        public bool IsCompleted { get; } = false;

        [UsedImplicitly]
        public void GetResult() { }

        [UsedImplicitly]
        public void OnCompleted(Action _) { }

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action _) { }

        [UsedImplicitly]
        public ForeverAwaiter GetAwaiter() => this;

        #endregion
    }
}