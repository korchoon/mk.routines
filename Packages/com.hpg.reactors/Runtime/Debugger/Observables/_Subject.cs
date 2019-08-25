// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Reactors.DataFlow;

namespace Reactors.Async.Debugger
{
    internal class _Subject<T> : DebugTracer2<_Subject<T>, Subject<T>>
    {
        public Action<string> OnNext;
        public Action<string> Next1;
        public Action<string> Dispose;
    }

    internal class _Subject : DebugTracer2<_Subject, Subject>
    {
        public Action<string> OnNext;
        public Action<string> Next1;
        public Action<string> Dispose;
    }
}