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
        public long Id;

        public Action OnDeregister;

        public static Action<TEvents> OnNew;
        public static long LastId { get; private set; }

        // ReSharper disable once StaticMemberInGenericType
        static ObjectIDGenerator _generator;

        static Dictionary<long, TEvents> _eventRegistry;

        static DebugTracer()
        {
            _eventRegistry = new Dictionary<long, TEvents>();
            _generator = new ObjectIDGenerator();
            LastId = 0;
        }


        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Register(TTarget target) // scope
        {
            var id = _generator.GetId(target, out var firstTime);

            var events = new TEvents {Id = id};
            if (firstTime)
            {
                LastId = id;
                _eventRegistry.Add(id, events);
            }
            else // pooled
            {
                _eventRegistry[id] = events;
            }

            OnNew?.Invoke(events);
        }

        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Deregister(TTarget target)
        {
            var hash = _generator.GetId(target, out var firstTime);
            Asr.IsFalse(firstTime);
            if (_eventRegistry.TryGetValue(hash, out var t))
                t.OnDeregister?.Invoke();
            else
                Asr.Fail("No such key");
            
            _eventRegistry.Remove(hash);
        }

        public static bool TryLocate(TTarget key, out TEvents value)
        {
            var id = _generator.HasId(key, out var firstTime);
            Asr.IsFalse(firstTime);
            return _eventRegistry.TryGetValue(id, out value);
        }

        static TEvents Locate(TTarget o)
        {
            var id = _generator.HasId(o, out var firstTime);
            Asr.IsFalse(firstTime);
            return _eventRegistry[id];
        }

        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Next(Func<TEvents, Action> selector, TTarget o)
        {
            var target = Locate(o);
            var method = selector.Invoke(target);
            method?.Invoke();
        }

        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Next<T>(Func<TEvents, Action<T>> selector, T arg, TTarget o)
        {
            var target = Locate(o);
            var method = selector.Invoke(target);
            method?.Invoke(arg);
        }
    }
}