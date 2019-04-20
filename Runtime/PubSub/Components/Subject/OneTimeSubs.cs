using System;
using System.Collections.Generic;
using Lib.Async;
using Lib.Pooling;
using UnityEngine.Assertions;

namespace Lib.DataFlow
{
    internal sealed class OneTimeSubs : IDisposable, IScope
        // promise?
    {
        Stack<Action> _pending;
        CompleteToken _d;

        public static Pool<OneTimeSubs> Pool { get; } = new Pool<OneTimeSubs>(() => new OneTimeSubs(), subs => subs.Reset());

        public OneTimeSubs()
        {
            _pending = new Stack<Action>();
            _d = new CompleteToken();
        }

        public void Dispose()
        {
            if (_d.Set()) return;

            while (_pending.Count > 0)
            {
                var dispose = _pending.Pop();
                dispose.Invoke();
            }
            Assert.IsTrue(_pending.Count < 300);
        }

        public void OnDispose(Action moveNext) => _pending.Push(moveNext);

        void Reset()
        {
            _d = new CompleteToken();
            _pending.Clear();
        }
    }

}