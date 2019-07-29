// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

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