using System;
using System.Collections.Generic;
using Game.Proto;
using Lib.Attributes;
using Lib.Utility;
using Utility.AssertN;

namespace Lib.DataFlow
{
    [NonPerformant(PerfKind.GC)]
    public sealed class Subscribers : IPub
    {
        Queue<Func<bool>> _next;
        Queue<Func<bool>> _current;
        CompleteToken _d;

        public Subscribers()
        {
            _next = new Queue<Func<bool>>();
            _current = new Queue<Func<bool>>();
            _d = new CompleteToken();
        }

        void Swap()
        {
            var buf = _next;
            _next = _current;
            _current = buf;
        }

        public bool Next()
        {
            if (_d) return false;

#if !BUG_DOUBLE_SEND
            Asr.IsTrue(_current.Count == 0); 
            Swap();
#else
            if (_current.Count == 0)
                Swap();
            else if (_next.Count > 0)
                Run(_next, _current);
#endif
            // _next cleared

            Run(_current, _next);
            // _current cleared

            return true;
        }

        static void Run(Queue<Func<bool>> current, Queue<Func<bool>> next)
        {
            while (current.Count > 0)
            {
                var callback = current.Dequeue();
                var moveNext = callback.Invoke();
                if (moveNext)
                    next.Enqueue(callback);
            }
        }

        public void Sub(Func<bool> moveNext) => _next.Enqueue(moveNext);

        public void Dispose()
        {
            if (_d.Set()) return;

            Clear();
        }

        void Clear()
        {
            _next.Clear();
            _current.Clear();
        }

        public void Reset()
        {
            Clear();
            _d = new CompleteToken();
        }
    }


    [NonPerformant(PerfKind.GC)]
    public sealed class Subscribers<T> : IPub<T>
    {
        Queue<Func<T, bool>> _next;
        Queue<Func<T, bool>> _current;
        CompleteToken _d;

        public Subscribers()
        {
            _next = new Queue<Func<T, bool>>();
            _current = new Queue<Func<T, bool>>();
            _d = new CompleteToken();
        }

        void Swap()
        {
            var buf = _next;
            _next = _current;
            _current = buf;
        }

        public bool Next(T msg)
        {
            if (_d) return false;

#if !BUG_DOUBLE_SEND
            Asr.IsTrue(_current.Count == 0);
            Swap();
#else
            if (_current.Count == 0)
                Swap();
            else if (_next.Count > 0)
                Run(_next, _current, msg);
#endif
            // _next cleared

            Run(_current, _next, msg);
            // _current cleared

            return true;
        }

        static void Run(Queue<Func<T, bool>> current, Queue<Func<T, bool>> next, T msg)
        {
            while (current.Count > 0)
            {
                var callback = current.Dequeue();
                var moveNext = callback.Invoke(msg);
                if (moveNext)
                    next.Enqueue(callback);
            }
        }


        public void Sub(Func<T, bool> moveNext) => _next.Enqueue(moveNext);

        public void Dispose()
        {
            if (_d.Set()) return;

            Clear();
        }

        void Clear()
        {
            _next.Clear();
            _current.Clear();
        }

        public void Reset()
        {
            Clear();
            _d = new CompleteToken();
        }
    }
}