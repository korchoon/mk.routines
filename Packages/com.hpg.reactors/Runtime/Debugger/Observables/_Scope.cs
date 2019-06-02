using System;
using System.Diagnostics;
using Lib.Async;
using Lib.DataFlow;

namespace Lib.Async.Debugger
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<(StackTraceHolder, Action)> OnDispose;
        public Action<(StackTraceHolder, Action)> Unsubscribe;
        public Action Dispose;
    }
}