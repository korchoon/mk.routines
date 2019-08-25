// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using Reactors.DataFlow;

namespace Reactors.Async.Debugger
{
    public class MethodEntry<T> : MethodEntry
    {
        public T Value;

        public void Trace(T value)
        {
            Inc();
            Value = value;
        }
    }

    public class MethodEntry
    {
        public int Count { get; private set; }
        public List<StackTraceHolder> StackTraceHolder;

        public MethodEntry()
        {
            StackTraceHolder = new List<StackTraceHolder>();
        }

        public void Inc(int skipFrames = 1)
        {
            StackTraceHolder.Add(DataFlow.StackTraceHolder.New(skipFrames + 1));
            ++Count;
        }
    }
}