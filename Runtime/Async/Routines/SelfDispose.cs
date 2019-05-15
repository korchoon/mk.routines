using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Lib.Async
{
    public struct SelfDispose
    {
        [UsedImplicitly]
        public SelfDisposeAwaiter GetAwaiter() => new SelfDisposeAwaiter();
    }

    public class SelfDisposeAwaiter : ICriticalNotifyCompletion
    {
        internal IDisposable Value;

        public bool IsCompleted { [UsedImplicitly] get; private set; }

        [UsedImplicitly]
        public IDisposable GetResult()
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