using System;
using Lib.DataFlow;

namespace Lib.Async
{
    public class _RoutineBuilder : DebugTracer<_RoutineBuilder, object>
    {
        public Action<StackTraceHolder> CtorTrace;
        public Action<StackTraceHolder> CurrentAwait;
        public Action AfterSetResult;
        public Action<Exception> AfterSetException;
        public Action<IBreakableAwaiter> AwaitOnCompleted;
    }
}