using System;
using System.Diagnostics;
using Lib.Async;

namespace Lib.DataFlow
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<StackTraceHolder> OnDispose;
        public Action AfterDispose;
        public Action<StackTraceHolder> SubscribeAfterDispose;
    }
}