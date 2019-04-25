using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Lib.DataFlow;
using Utility;
using Utility.AssertN;
using Debug = UnityEngine.Debug;

namespace Lib.Async
{
    public abstract class DebugTracer3<TEvents, TTarget> where TEvents : DebugTracer3<TEvents, TTarget>, new() where TTarget : class
    {
        public static ISub<SubWrapper<TEvents>> OnNew;
        static IPub<SubWrapper<TEvents>> _pubNew;

        int _id;
        static Dictionary<int, TEvents> _eventRegistry;
        static Dictionary<int, WeakReference<TTarget>> _targetRegistry;

        static DebugTracer3()
        {
            OnNew = new Subject<SubWrapper<TEvents>>(Empty.Scope());
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
            _pubNew.Next(wrap);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Deregister(TTarget target)
        {
            var hash = target.GetHashCode();
            _eventRegistry.Remove(hash);
        }

        static TEvents _Locate(TTarget o) => _eventRegistry[o.GetHashCode()];

        [Conditional(FLAGS.DEBUG)]
        static void Report(TTarget o, Func<TEvents, Action> t)
        {
            var target = _Locate(o);
            var method = t.Invoke(target);
            method?.Invoke();
        }

        [Conditional(FLAGS.DEBUG)]
        static void Report<T>(TTarget o, Func<TEvents, Action<T>> t, T arg)
        {
            var target = _Locate(o);
            var method = t.Invoke(target);
            method?.Invoke(arg);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Pub(TTarget o, Func<TEvents, IPub> selector)
        {
            var target = _Locate(o);
            var method = selector.Invoke(target);
            method?.Next();
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Pub<T>(TTarget o, Func<TEvents, IPub<T>> selector, T arg)
        {
            var target = _Locate(o);
            var method = selector.Invoke(target);
            method?.Next(arg);
        }
    }

    public class SubWrapper<T>
    {
        T _t;

        public SubWrapper(T t)
        {
            _t = t;
        }

        public ISub Sub(Func<T, ISub> selector)
        {
            var res = selector.Invoke(_t);
            return res;
        }

        public ISub<TArg> Sub<TArg>(Func<T, ISub<TArg>> selector) => selector.Invoke(_t);
    }
}