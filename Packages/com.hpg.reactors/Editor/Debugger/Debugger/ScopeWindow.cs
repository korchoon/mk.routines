// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using JetBrains.Annotations;
using Lib;
using Lib.Async;
using Lib.Async.Debugger;
using Lib.DataFlow;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Utility;

namespace MyNamespace
{
    public class ScopeWindow : ReactiveWindow
    {
        [MenuItem("Debug Tools/Scopes")]
        static void OpenWindow()
        {
            var w = GetWindow<ScopeWindow>();
            w.Show();
        }

        [InitializeOnLoadMethod]
        static void RemoveFlag()
        {
//            SetFlag(false);
        }

        [ShowInInspector] SortableEdList<_Scope> _all;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SetFlag(false);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        protected override async Routine Flow()
        {
            var scope = await Routine.SelfScope();

            var i = 0;
            Context.OnGui.OnNext(() =>
            {
                if (i++ % 10 == 0) _all.Sort();
            }, scope);

            Context.EventCtx.KeyDown.OnNext(key =>
            {
                if (key == KeyCode.R)
                    Debug.Log("RRR");
            }, scope);


            _all = new SortableEdList<_Scope>((s0, s1) => -s0.Count + s1.Count);
            Lib.Async.Debugger._Scope.OnNew += OnNew;

            await EnsureDebugFlag();

            await scope;
        }

        async Routine EnsureDebugFlag()
        {
            if (!HasFlag())
                await Context.Button("Set DEBUG_SCOPE flag");

            SetFlag(true);
            // todo postpone unset flag  
        }

        static bool HasFlag() => ScriptingDefineUtil.Split(ScriptingDefineUtil.Current()).Contains(FLAGS.DEBUG_TRACE);

        static void SetFlag(bool active)
        {
            string res;
            if (active)
            {
                if (!ScriptingDefineUtil.TryAddFlag(FLAGS.DEBUG_TRACE, ScriptingDefineUtil.Current(), out res))
                    return;
            }
            else
            {
                if (!ScriptingDefineUtil.TryRemoveFlag(FLAGS.DEBUG_TRACE, ScriptingDefineUtil.Current(), out res))
                    return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, res);
            AssetDatabase.SaveAssets();
        }


        void OnNew(Lib.Async.Debugger._Scope t)
        {
            var sc = new _Scope();
            _all.All.Add(sc);

            t.CtorStackTrace += msg => sc.Created = msg;
            t.OnDeregister += () => _all.All.Remove(sc);
            t.OnDispose += msg =>
            {
                sc.Subscribers.Add(msg.Item1);
                var act = msg.Item2;
                t.Unsubscribe += tuple =>
                {
                    if (tuple.Item2 != act)
                        return;
                    sc.Subscribers.Remove(msg.Item1);
                };
            };
        }


        [InlineProperty, HideReferenceObjectPicker]
        public class _Scope
        {
            [UsedImplicitly, HideLabel] public StackTraceHolder Created;
            public int Count => Subscribers.Count;
            public List<StackTraceHolder> Subscribers = new List<StackTraceHolder>();
        }
    }
}