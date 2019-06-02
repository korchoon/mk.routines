using System;
using Lib.DataFlow;

namespace Lib.Async.Debugger
{
    public class _RoutineBuilder : DebugTracer2<_RoutineBuilder, RoutineBuilder>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<Routine> SetTask;
        public Action BreakCurrent;
        public Action Start;
        public Action SetResult;
        public Action SetException;
        public Action AwaitOnCompleted;
    }
}