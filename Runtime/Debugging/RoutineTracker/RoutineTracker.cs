using System.Collections.Generic;
using System.ComponentModel;
using Lib.Async;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Lib.DataFlow
{
    public static class DictUtils
    {
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
        {
            if (d.ContainsKey(key))
                return false;

            d.Add(key, value);
            return true;
        }
    }

    public class RoutineTracker : OdinEditorWindow
    {
        [MenuItem("Tools/Routine")]
        static void OpenWindow()
        {
            var window = GetWindow<RoutineTracker>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        Dictionary<long, RoutineEd> _routines;
        Dictionary<long, Awaiter> _awaiters;
        Dictionary<long, Scope> _scopes;

        [ShowInInspector] SortableEdList<Builder> _builders;

        protected override void OnEnable()
        {
            Debug.Log("Enable");
            _routines = new Dictionary<long, RoutineEd>();
            _awaiters = new Dictionary<long, Awaiter>();
            _scopes = new Dictionary<long, Scope>();
            _builders = new SortableEdList<Builder>((a, b) => b.CtorTrace.Name.Length - a.CtorTrace.Name.Length);

            Routine.DR.OnNew += OnRoutine;
            _RoutineBuilder.OnNew += OnBuilder;
            Routine.DA.OnNew += OnAwaiter;
            _Scope.OnNew += OnScope;
        }

        void OnScope(_Scope t)
        {
            var sc = new Scope() {RefId = t.Id};
            _scopes.TryAdd(sc.RefId, sc);
            t.CtorStackTrace += msg => sc.Ctor = msg;
            t.AfterDispose += () => sc.Disposed = true;
            t.OnDispose += i => sc.List.Add(i);
        }

        void OnRoutine(Routine.DR evt)
        {
            var r = new RoutineEd {RefId = evt.Id};
            _routines.TryAdd(r.RefId, r);

            InitializeRoutine(t: r, evt: evt);
        }

        void InitializeRoutine(RoutineEd t, Routine.DR evt)
        {
            t.RefId = evt.Id;
            evt.GetAwaiter += awaiter =>
            {
                if (!Routine.DA.TryLocate(awaiter, out var located))
                    return;

                _awaiters.TryGetValue(located.Id, out var ed);
                t.SelfAwaiters.Add(ed);
            };
            evt.Dispose += () => { t.Disposed = true; };

            evt.Ctor += msg => t.Ctor = msg;

            evt.SetScope += scope =>
            {
                if (!_Scope.TryLocate(scope, out var loc))
                    return;

                _scopes.TryGetValue(loc.Id, out var ed);
                t.Scope = ed;
            };

            evt.SubscribeToScope += scope =>
            {
                if (!_Scope.TryLocate(scope, out var loc))
                    return;
                
                _scopes.TryGetValue(loc.Id, out var ed);
                t.DisposeOnScope = ed;
            };
        }

        void OnBuilder(_RoutineBuilder t)
        {
            var b = new Builder();
            _builders.All.Add(b);

            t.AfterSetException += e => _builders.All.Remove(b);
            t.AfterSetResult += () => _builders.All.Remove(b);
#if !M_DISABLED
            t.CtorTrace += r => { b.CtorTrace = r; };
            t.CurrentAwait += r => { b.Await = r.FrameFileInfo(); };
#endif
        }

        void OnAwaiter(Routine.DA evt)
        {
            var aw = new Awaiter() {RefId = evt.Id};
            _awaiters.TryAdd(aw.RefId, aw);
            evt.Thrown += msg => aw.Thrown = msg.ToString();
            evt.AfterBreak += () => aw.AfterBreak = true;
            evt.GetResult += () => aw.GotResult = true;
            evt.OnCompleteImmediate += holder => aw.CompletedImmediate.Add(holder);
            evt.OnCompleteLater += holder => aw.Later.Add(holder);
        }


        [HideReferenceObjectPicker, HideLabel, InlineProperty]
        class RoutineEd
        {
            public long RefId;
            public StackTraceHolder Ctor;
            public Scope Scope;
            public Scope DisposeOnScope;
            public List<Awaiter> SelfAwaiters = new List<Awaiter>();
            public List<Awaiter> InnerBlockAwaiters = new List<Awaiter>();
            public bool Disposed;
        }

        class Awaiter
        {
            public long RefId;
            public string Thrown;
            public bool AfterBreak;
            public bool GotResult;
            public List<StackTraceHolder> Later = new List<StackTraceHolder>();
            public List<StackTraceHolder> CompletedImmediate = new List<StackTraceHolder>();
        }

        [HideReferenceObjectPicker, HideLabel]
        class Builder
        {
//            public RoutineEd Routine;
            public StackTraceHolder CtorTrace;
            [Sirenix.OdinInspector.ReadOnly] public string Await;
        }

        [InlineProperty, HideReferenceObjectPicker]
        public class Scope
        {
            public long RefId;
            public StackTraceHolder Ctor;
            public bool Disposed;

            public List<StackTraceHolder> List = new List<StackTraceHolder>();
        }
    }
}