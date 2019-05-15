using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;

namespace Lib.Async
{
    public struct SelfScope
    {
        [UsedImplicitly]
        public SelfScopeAwaiter GetAwaiter() => new SelfScopeAwaiter();
    }

    public class SelfScopeAwaiter : ICriticalNotifyCompletion
    {
        internal IScope Value;

        public bool IsCompleted
        {
            [UsedImplicitly]
            get;
            private set;
        }

        [UsedImplicitly]
        public IScope GetResult()
        {
//            Asr.IsNotNull(Value);
            return Value;
        }

        [UsedImplicitly]
        public void OnCompleted(Action continuation)
        {
            continuation.Invoke();
            IsCompleted = true;
        }

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action continuation)
        {
            continuation.Invoke();
            IsCompleted = true;
        }
    }
}