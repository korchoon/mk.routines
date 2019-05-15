using System;
using System.Diagnostics;
using Lib.Async;

namespace Lib.DataFlow
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<StackTraceHolder> Finally;
        public Action<StackTraceHolder> Unsubscribe;
        public Action Dispose;
    }
}