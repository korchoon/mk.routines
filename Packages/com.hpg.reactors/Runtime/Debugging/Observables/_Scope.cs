// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using Lib.Async;

namespace Lib.DataFlow
{
    public class _Scope : DebugTracer<_Scope, IScope>
    {
        public Action<StackTraceHolder> CtorStackTrace;
        public Action<(StackTraceHolder, Action)> OnDispose;
        public Action<(StackTraceHolder, Action)> Unsubscribe;
        public Action Dispose;
        public Action<(long Parent, long Child)> AddRelation;
        public Action<(long Parent, long Child)> RemoveRelation;
    }
}