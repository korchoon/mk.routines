// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Reactors.DataFlow;

namespace Reactors.Async
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