using System.Collections.Generic;
using Lib.DataFlow;

namespace Lib.Async.Debugger
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