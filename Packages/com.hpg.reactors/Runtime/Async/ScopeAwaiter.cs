using System;
using System.Runtime.CompilerServices;
using Lib.DataFlow;
using Lib.Utility;

namespace Lib.Async
{
#if M_DISABLED
     public class ScopeAwaiter : ICriticalNotifyCompletion, IBreakableAwaiter
    {
        IDisposable _dipose;
        public IScope InnerTree { get; }

        public ScopeAwaiter(IScope scope)
        {
            _dipose = scope.Scope(out var tmp);
            InnerTree = tmp;
        }

        public bool IsCompleted => InnerTree.Completed;

        public void GetResult()
        {
        }

        public void UnsafeOnCompleted(Action moveNext) => InnerTree.OnDispose(moveNext);


        void INotifyCompletion.OnCompleted(Action continuation) => UnsafeOnCompleted(continuation);

        public void Break() => _dipose.Dispose();
    }
#endif
}