//#define M_DISABLE_POOLING

using System;
using System.Collections.Generic;
using Lib.DataFlow;
using Utility.Asserts;

namespace Lib.Pooling
{
    public class Pool<T> : IDisposable where T : class
    {
        Func<T> _ctor;
        readonly Stack<T> _stack;

        // todo place asserts on app quit
        DebugPoolCounter _debugPoolCounter = new DebugPoolCounter();
        Action<T> _reset;
        Action<T> _destroy;

        public Pool(Func<T> ctor, Action<T> reset, Action<T> destroy = null)
        {
            _ctor = ctor;
#if !M_DISABLE_POOLING
            _destroy = destroy ?? ActionEmpty<T>.Empty;
            _reset = reset;
            _stack = new Stack<T>();
#endif
        }

        public T Get()
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

        public void Release(ref T element)
        {
#if !M_DISABLE_POOLING
            Asr.IsFalse(_stack.Count > 0 && ReferenceEquals(_stack.Peek(), element),
                "Internal error. Trying to release object that is already released to pool. ");

            _debugPoolCounter.Release();
            _reset.Invoke(element);
            _stack.Push(element);
#endif

            element = null;
        }


        public void Dispose()
        {
#if !M_DISABLE_POOLING
            while (_stack.Count > 0)
            {
                var t = _stack.Pop();
                _destroy.Invoke(t);
            }
#endif
        }

        public _Scope GetScoped(out T tmp)
        {
            tmp = Get();
            return new _Scope(this, ref tmp);
        }

        public struct _Scope : IDisposable
        {
            Pool<T> _pool;
            T _val;

            internal _Scope(Pool<T> pool, ref T val)
            {
                _pool = pool;
                _val = val;
            }

            public void Dispose()
            {
                _pool.Release(ref _val);
            }
        }
    }
}