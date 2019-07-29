// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.DataFlow;

namespace Lib.Async
{
    public class _Routine_Awaiter : DebugTracer<_Routine_Awaiter, Routine.Awaiter>
    {
        public Action AfterBreak;
        public Action GetResult;
        public Action<Exception> Thrown;
        public Action<StackTraceHolder> OnCompleteImmediate;
        public Action<StackTraceHolder> OnCompleteLater;
    }
}