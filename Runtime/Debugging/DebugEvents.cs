using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Utility;
using Utility.AssertN;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    public static class ReportExt
    {
        public static void Report(this object o, Action<Action> del)
        {
        }

        public static void Report<T>(this object o, MethodBase b) where T : Delegate
        {
        }
    }

    public abstract class DebugEvents<TEvents, TTarget> where TEvents : DebugEvents<TEvents, TTarget>, new() where TTarget : class
    {
        public static event Action<TEvents> OnNew;

        int _id;
        static Dictionary<int, TEvents> _eventRegistry;
        static Dictionary<int, WeakReference<TTarget>> _targetRegistry;

        static DebugEvents()
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
            OnNew?.Invoke(events);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Deregister(TTarget target)
        {
            var hash = target.GetHashCode();
            _eventRegistry.Remove(hash);
        }

        static TEvents Locate(TTarget o) => _eventRegistry[o.GetHashCode()];

        [Conditional(FLAGS.DEBUG)]
        public static void Report(TTarget o, Func<TEvents, Action> t)
        {
            var target = Locate(o);
            var method = t.Invoke(target);
            method?.Invoke();
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Report<T>(TTarget o, Func<TEvents, Action<T>> t, T arg)
        {
            var target = Locate(o);
            var method = t.Invoke(target);
            method?.Invoke(arg);
        }
    }
}