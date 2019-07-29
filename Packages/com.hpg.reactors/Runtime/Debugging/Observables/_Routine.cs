// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib.DataFlow;

namespace Lib.Async
{
    public class _Routine : DebugTracer<_Routine, Routine>
    {
        public Action<StackTraceHolder> Ctor;
        public Action Dispose;
        public Action<IScope> SubscribeToScope;
        public Action<IScope> SetScope;
        public Action<Routine.Awaiter> GetAwaiter;
    }
}