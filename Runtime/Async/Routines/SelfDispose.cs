using System;
using System.Runtime.CompilerServices;

namespace Lib.Async
{
    public struct SelfDispose
    {
        public SelfDisposeAwaiter GetAwaiter() => new SelfDisposeAwaiter();
    }

    public class SelfDisposeAwaiter : ICriticalNotifyCompletion
    {
        internal IDisposable Value;
        bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            private set => _isCompleted = value;
        }

        public IDisposable GetResult()
        {
//            Asr.IsNotNull(Value);
            return Value;
        }

        public void OnCompleted(Action continuation)
        {
            continuation.Invoke();
            _isCompleted = true;
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            continuation.Invoke();
            _isCompleted = true;
        }
    }
}