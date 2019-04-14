using System;
using System.Collections.Generic;
using Lib.DataFlow;
using Utility.AssertN;

namespace Lib.Pooling
{
    public class Pool<T> : IDisposable
    {
        Func<T> _ctor;
        readonly Stack<T> _stack;

        DebugPoolCounter _debugPoolCounter = new DebugPoolCounter();
        Action<T> _reset;
        Action<T> _destroy;

        public Pool(Func<T> ctor, Action<T> reset, Action<T> destroy = null)
        {
            _destroy = destroy ?? ActionEmpty<T>.Empty;
            _ctor = ctor;
            _reset = reset;
            _stack = new Stack<T>();
        }

        public T _GetRaw()
        {
            T element;
            if (_stack.Count == 0)
            {
                _debugPoolCounter.Create();
                element = _ctor();
            }
            else
            {
                _debugPoolCounter.Pop();
                element = _stack.Pop();
            }

            return element;
        }

        public void _ReleaseRaw(T element)
        {
            Asr.IsFalse(_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element),
                "Internal error. Trying to release object that is already released to pool. ");

            _debugPoolCounter.Release();
            _reset(element);
            _stack.Push(element);
        }


        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                var t = _stack.Pop();
                _destroy.Invoke(t);
            }
        }
    }
}