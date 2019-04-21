//#define M_DISABLE_POOLING

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
#if !M_DISABLE_POOLING
            _destroy = destroy ?? ActionEmpty<T>.Empty;
            _ctor = ctor;
            _reset = reset;
            _stack = new Stack<T>();
#endif
        }

        public T _GetRaw()
        {
#if M_DISABLE_POOLING
            return _ctor.Invoke();
#else
            T element;
            if (_stack.Count == 0)
            {
                _debugPoolCounter.New();
                element = _ctor();
            }
            else
            {
                _debugPoolCounter.Get();
                element = _stack.Pop();
            }

            return element;
#endif
        }

        public void _ReleaseRaw(T element)
        {
#if !M_DISABLE_POOLING
            Asr.IsFalse(_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element),
                "Internal error. Trying to release object that is already released to pool. ");

            _debugPoolCounter.Release();
            _reset(element);
            _stack.Push(element);
#endif
        }


        public void Dispose()
        {
#if M_DISABLE_POOLING
            while (_stack.Count > 0)
            {
                var t = _stack.Pop();
                _destroy.Invoke(t);
            }
#endif
        }
    }
}