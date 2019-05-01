using System;
using System.Runtime.CompilerServices;
using Lib.DataFlow;

namespace Lib.Async
{
    public struct SelfScope
    {
        public SelfScopeAwaiter GetAwaiter() => new SelfScopeAwaiter();
    }

    public class SelfScopeAwaiter : ICriticalNotifyCompletion
    {
        internal IScope Value;
        bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            private set => _isCompleted = value;
        }

        public IScope GetResult()
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