using System;
using Lib.DataFlow;

namespace Lib.Async
{
#if M_DISABLED
     public class _Routine_Awaiter : DebugTracer<_Routine_Awaiter, Routine.Awaiter>
    {
        public Action AfterBreak;
        public Action GetResult;
        public Action<Exception> Thrown;
        public Action<StackTraceHolder> OnCompleteImmediate;
        public Action<StackTraceHolder> OnCompleteLater;
    }
#endif
}