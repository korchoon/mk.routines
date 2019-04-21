using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Lib.DataFlow;
using Utility;
using Utility.AssertN;

namespace Lib.Async
{
    public abstract class DebugTracer<TEvents, TTarget> where TEvents : DebugTracer<TEvents, TTarget>, new() where TTarget : class
    {
        public static Action<SubWrapper<TEvents>> OnNew;

        // ReSharper disable once StaticMemberInGenericType
        static ObjectIDGenerator Generator;

        long _id;
        static Dictionary<long, TEvents> _eventRegistry;
        static Dictionary<long, WeakReference<TTarget>> _targetRegistry;

        static DebugTracer()
        {
            _eventRegistry = new Dictionary<long, TEvents>();
            Generator = new ObjectIDGenerator();
            _targetRegistry = new Dictionary<long, WeakReference<TTarget>>();
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Register(TTarget target) // scope
        {
            var id = Generator.GetId(target, out var firstTime);
            var events = new TEvents {_id = id};
            _eventRegistry.Add(events._id, events);
            var weakReference = new WeakReference<TTarget>(target);
            _targetRegistry.Add(events._id, weakReference);

            var wrap = new SubWrapper<TEvents>(events);
            OnNew?.Invoke(wrap);
        }

        [Conditional(FLAGS.DEBUG)]
        public static void Deregister(TTarget target)
        {
            var hash = Generator.GetId(target, out _);
            _eventRegistry.Remove(hash);
        }

        static TEvents _Locate(TTarget o)
        {
//            var id = Generator.GetId(o, out _);
            var id = Generator.HasId(o, out var firstTime);
            Asr.IsFalse(firstTime);
            return _eventRegistry[id];
        }

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