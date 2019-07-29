// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Game.Proto;
using Lib.Async;
using Lib.Attributes;
using Lib.Utility;
using Utility.Asserts;

namespace Lib.DataFlow
{
    internal sealed class Subscribers : IPub
    {
        struct Item
        {
            public Action Action;
            public IScope Scope;
        }

        Queue<Item> _next;
        Queue<Item> _current;
        internal bool Completed;

        public Subscribers()
        {
            _next = new Queue<Item>();
            _current = new Queue<Item>();
            Completed = false;
        }

        void Swap()
        {
            var buf = _next;
            _next = _current;
            _current = buf;
        }

        public void Next()
        {
            if (Completed) return;

#if !BUG_DOUBLE_SEND
            Asr.IsTrue(_current.Count == 0);
            Swap();
#else

            if (_current.Count == 0)
                Swap();
            else if (_current.Count > 0 && _next.Count > 0) 
                Run(_next, _current);
#endif
            // _next cleared

            Run(_current, _next);
            // _current cleared


            return;
        }


        static void Run(Queue<Item> current, Queue<Item> next)
        {
            while (current.Count > 0)
            {
                var callback = current.Dequeue();
                if (callback.Scope.Completed)
                    continue;

                callback.Action.Invoke();
                if (callback.Scope.Completed)
                    continue;

                next.Enqueue(callback);
            }
        }

        public void Sub(Action action, IScope scope)
        {
            _next.Enqueue(new Item() {Action = action, Scope = scope});
        }

        public void Dispose()
        {
            if (Completed.WasTrue()) return;

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
            Completed = false;
        }
    }


    [NonPerformant(PerfKind.GC)]
    internal sealed class Subscribers<T> : IPub<T>
    {
        struct Item
        {
            public Action<T> Action;
            public IScope Scope;
        }

        Queue<Item> _next;
        Queue<Item> _current;
        internal bool Completed;

        public Subscribers()
        {
            _next = new Queue<Item>();
            _current = new Queue<Item>();
            Completed = false;
        }

        void Swap()
        {
            var buf = _next;
            _next = _current;
            _current = buf;
        }


        public void Sub(Action<T> action, IScope scope) => _next.Enqueue(new Item() {Action = action, Scope = scope});

        public void Next(T msg)
        {
            if (Completed) return;

#if !BUG_DOUBLE_SEND
            Asr.IsTrue(_current.Count == 0);
            Swap();
#else
            if (_current.Count == 0)
                Swap();
            else if (_current.Count > 0 && _next.Count > 0)
                Run(_next, _current, msg);
#endif
            // _next cleared

            Run(_current, _next, msg);
            // _current cleared

            return;
        }


        static void Run(Queue<Item> prev, Queue<Item> next, T msg)
        {
            while (prev.Count > 0)
            {
                var callback = prev.Dequeue();
                if (callback.Scope.Completed)
                    continue;

                callback.Action.Invoke(msg);
                if (callback.Scope.Completed)
                    continue;

                next.Enqueue(callback);
            }
        }


        public void Dispose()
        {
            if (Completed.WasTrue()) return;

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
            Completed = false;
        }
    }
}