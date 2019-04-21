using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lib.DataFlow;
using Utility;

namespace Lib.Async
{
    public abstract class DebugTracer<TEvents, TTarget> where TEvents : DebugTracer<TEvents, TTarget>, new() where TTarget : class
    {
        public static Action<SubWrapper<TEvents>> OnNew;

        int _id;
        static Dictionary<int, TEvents> _eventRegistry;
        static Dictionary<int, WeakReference<TTarget>> _targetRegistry;

        static DebugTracer()
        {
            _eventRegistry = new Dictionary<int, TEvents>();
            _targetRegistry = new Dictionary<int, WeakReference<TTarget>>();
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Register(TTarget target) // scope
        {
            var events = new TEvents {_id = target.GetHashCode()};
            _eventRegistry.Add(events._id, events);
            var weakReference = new WeakReference<TTarget>(target);
            _targetRegistry.Add(events._id, weakReference);

            var wrap = new SubWrapper<TEvents>(events);
            OnNew?.Invoke(wrap);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Deregister(TTarget target)
        {
            var hash = target.GetHashCode();
            _eventRegistry.Remove(hash);
        }

        static TEvents _Locate(TTarget o) => _eventRegistry[o.GetHashCode()];

        [Conditional(FLAGS.DEBUG)]
        public static void Pub(TTarget o, Func<TEvents, Action> selector)
        {
            var target = _Locate(o);
            var method = selector.Invoke(target);
            method?.Invoke();
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Pub<T>(TTarget o, Func<TEvents, Action<T>> selector, T arg)
        {
            var target = _Locate(o);
            var method = selector.Invoke(target);
            method?.Invoke(arg);
        }
    }
}