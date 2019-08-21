// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lib.DataFlow;
using Utility;
using Utility.Asserts;
using Debug = UnityEngine.Debug;

namespace Lib.Async.Debugger
{
    public abstract class DebugTracer2<TEvents, TTarget> where TEvents : DebugTracer2<TEvents, TTarget>, new() where TTarget : class
    {
        public long Id;
        string _toString;

        public override string ToString()
        {
            return _toString;
        }

        public Action OnDeregister;
        public static Action<TEvents> OnNew;

        public static long LastId { get; private set; }

        // ReSharper disable once StaticMemberInGenericType
        static ObjectIDGenerator _generator;


        static DebugTracer2()
        {
            _generator = new ObjectIDGenerator();
            LastId = 0;
            _Registry = new Dictionary<long, TargetEntry>();
        }


        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Register(TTarget target, string toString) // scope
        {
            var id = _generator.GetId(target, out var firstTime);

            var events = new TEvents {Id = id, _toString = toString};
            var targetEntry = new TargetEntry(events);
            if (firstTime)
            {
                LastId = id;
                _Registry.Add(id, targetEntry);
            }
            else // pooled
            {
                _Registry[id] = targetEntry;
            }
        }

        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Deregister(TTarget target)
        {
            var hash = _generator.GetId(target, out var firstTime);
            Asr.IsFalse(firstTime);
            if (_Registry.TryGetValue(hash, out var t))
                _Registry[hash].Deregistered = true;
            else
                Asr.Fail("No such key");
        }

        static TargetEntry Locate2(TTarget o)
        {
            var id = _generator.HasId(o, out var firstTime);
            Asr.IsFalse(firstTime);
            return _Registry[id];
        }


        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Next(Func<TEvents, Action> selector, TTarget o)
        {
            var message = StackTraceHolder.New(2).GetName(false);
            var targetEntry = Locate2(o);
            Debug.Log($"{targetEntry.Events._toString}: {message}");
            targetEntry.CallVoid(selector);
        }

        [Conditional(FLAGS.DEBUG_TRACE)]
        public static void Next<T>(Func<TEvents, Action<T>> selector, T arg, TTarget o)
        {
            Locate2(o).CallT(selector, arg);
        }

        public static Dictionary<long, TargetEntry> _Registry;

        public TargetEntry[] All => _Registry.Values.ToArray();

        public class TargetEntry
        {
            public TEvents Events;
            public bool Deregistered { get; internal set; }

            public Dictionary<Delegate, MethodEntry> CallTrace;

            public void CallVoid<TDel>(TDel del, int skip = 1) where TDel : Delegate
            {
                if (!CallTrace.TryGetValue(del, out var entry))
                {
                    entry = new MethodEntry();
                    CallTrace.Add(del, entry);
                }

                entry.Inc(skip + 1);
            }

            public void CallT<TDel, TValue>(TDel del, TValue value, int skip = 1) where TDel : Delegate
            {
                if (!CallTrace.TryGetValue(del, out var entry))
                {
                    entry = new MethodEntry();
                    CallTrace.Add(del, entry);
                }

                entry.Inc(skip + 1);
            }

            public TargetEntry(TEvents events)
            {
                Events = events;
                CallTrace = new Dictionary<Delegate, MethodEntry>();
            }
        }
    }
}