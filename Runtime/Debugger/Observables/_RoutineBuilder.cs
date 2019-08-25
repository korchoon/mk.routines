// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors.DataFlow;

namespace Reactors.Async.Debugger
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