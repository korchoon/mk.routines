using System;
using Lib.DataFlow;

namespace Lib.Async
{
#if M_DISABLED
     public class _Routine : DebugTracer<_Routine, Routine>
    {
        public Action<StackTraceHolder> Ctor;
        public Action Dispose;
        public Action<IScope> SubscribeToScope;
        public Action<IScope> SetScope;
        public Action<Routine.Awaiter> GetAwaiter;
    }
#endif
}