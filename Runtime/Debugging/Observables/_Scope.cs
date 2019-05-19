using System;
using System.Diagnostics;
using Lib.Async;

namespace Lib.DataFlow
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<(StackTraceHolder, Action)> OnDispose;
        public Action<(StackTraceHolder, Action)> Unsubscribe;
        public Action Dispose;
        public Action<(long Parent, long Child)> AddRelation;
        public Action<(long Parent, long Child)> RemoveRelation;
    }
}