// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using Reactors.Async;
using Reactors.DataFlow;

namespace Reactors.Async.Debugger
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<(StackTraceHolder, Action)> OnDispose;
        public Action<(StackTraceHolder, Action)> Unsubscribe;
        public Action Dispose;
    }
}