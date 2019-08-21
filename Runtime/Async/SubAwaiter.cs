// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Lib.DataFlow;
using Lib.Pooling;
using Lib.Utility;
using Utility;
using Utility.Asserts;

namespace Lib.Async
{
#if M_DISABLED
     public class SubAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        IDisposable _dispose;
        public IScope InnerTree { get; }

        SubAwaiter()
        {
            _dispose = React.Scope(out var tmp);
            InnerTree = tmp;
        }

        public static SubAwaiter New() => new SubAwaiter();


        [UsedImplicitly] public bool IsCompleted => InnerTree.Completed;

        [UsedImplicitly]
        public void GetResult()
        {
//            Break();
        }

        public void Break() => _dispose.Dispose();

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action moveNext) => InnerTree.OnDispose(moveNext);


        [UsedImplicitly]
        void INotifyCompletion.OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);
    }

    public class SubAwaiter<T> : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        Option<T> _result;
        public IScope InnerTree { get; }
        IDisposable _dispose;

        public SubAwaiter(ISub<T> s)
        {
            _dispose = React.Scope(out var tmp);
            InnerTree = tmp;
            s.OnNext(msg =>
            {
                _result = msg;
                Break();
            }, InnerTree);
        }

        [UsedImplicitly]
        public T GetResult()
        {
//            Break();

            if (!_result.TryGet(out var res))
            {
//                Asr.Fail("Tried to break & get result of Routine<T>");
            }

            return res; //todo
        }

        [UsedImplicitly] public bool IsCompleted => InnerTree.Completed;

        [UsedImplicitly]
        public void OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        [UsedImplicitly]
        public void UnsafeOnCompleted(Action moveNext) => InnerTree.OnDispose(moveNext);


        public void Break()
        {
            _dispose.Dispose();
        }
    }
#endif
}