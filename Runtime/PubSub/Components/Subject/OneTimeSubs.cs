using System;
using System.Collections.Generic;
using Lib.Pooling;

namespace Lib.DataFlow
{
#if M_DISABLED
     internal sealed class OneTimeSubs : IDisposable, IScope
        // promise?
    {
        Queue<Action> _pending;
        CompleteToken _d;

        public static Pool<OneTimeSubs> Pool { get; } = new Pool<OneTimeSubs>(() => new OneTimeSubs(), subs => subs.Reset());

        public OneTimeSubs()
        {
            _pending = new Queue<Action>();
            _d = new CompleteToken();
        }

        public void Dispose()
        {
            if (_d.Set()) return;

            while (_pending.Count > 0)
            {
                var uof = _pending.Dequeue();
                uof.Invoke();
            }
        }

        public void OnDispose(Action moveNext) => _pending.Enqueue(moveNext);

        void Reset()
        {
            _d = new CompleteToken();
            _pending.Clear();
        }
    }
#endif

}