// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.Async.Debugger;
using Lib.DataFlow;

namespace Lib.Async.Debugger
{
    public class _GenericAwaiter2 : DebugTracer2<_GenericAwaiter2, GenericAwaiter2>
    {
        public Action GetResult;
        public Action BreakInner;
        public Action<StackTraceHolder> OnCompletedImmediate;
        public Action<StackTraceHolder> OnCompletedLater;
    }
}