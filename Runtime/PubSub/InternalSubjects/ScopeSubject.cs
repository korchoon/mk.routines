using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lib.Async;
using Lib.Pooling;
using Sirenix.Utilities;
using UnityEngine.Assertions;
using Utility.AssertN;

namespace Lib.DataFlow
{
    internal class ScopeSubject : IDisposable, IScope
    {
        Stack<Action> _stack;
        bool _disposed;
        public static Pool<ScopeSubject> Pool { get; } = new Pool<ScopeSubject>(() => new ScopeSubject(), subs => subs._SetNew());

        public ScopeSubject()
        {
            _stack = new Stack<Action>();
            _SetNew();
        }

        void _SetNew()
        {
            TraceScope.Register(this);
            
            _disposed = false;
            Asr.IsTrue(_stack.Count == 0);
            _stack.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            while (_stack.Count > 0)
            {
                var dispose = _stack.Pop();
                dispose.Invoke();
            }
            
            TraceScope.Pub(this, t => t.AfterDispose);
        }

        public void OnDispose(Action dispose)
        {
            TraceScope.Pub(this, t => t.AfterDispose);
            
            if (_disposed)
            {
//                dispose();
                return;
            }

//            Assert.IsTrue(_stack.Count < 300);

            _stack.Push(dispose);
        }
    }

    public class TraceScope : DebugTracer<TraceScope, IScope>
    {
        internal Action<DisposeAction> OnDispose;
        internal Action AfterDispose;
        internal Action ImmediateOnDispose;
    }

    public class DisposeAction
    {
        string _toString;
        string _trace;
        public override string ToString() => _trace;

        public DisposeAction(Action a)
        {
            _toString = a.Method.GetNiceName();
            var frame = new StackTrace(true).GetFrame(3);
            _trace = $"{Path.GetFileNameWithoutExtension(frame.GetFileName())}: {frame.GetMethod().GetNiceName()} : {frame.GetFileLineNumber()}";
        }
    }
}