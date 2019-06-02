using System;
using Lib.DataFlow;

namespace Lib.Async.Debugger
{
    public class _Routine : DebugTracer2<_Routine, Routine>
    {
        public Action Ctor;
        public Action<RoutineBuilder> InitBuilder;
        public Action<ISub> InitComplete;
        public Action<IScope> InitScope;
        public Action Dispose;
        public Action<GenericAwaiter2> GetAwaiter;
    }
}