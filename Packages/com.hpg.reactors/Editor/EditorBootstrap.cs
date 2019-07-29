// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2017-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Game;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using UnityEditor;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Unknown
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class EditorBootstrap
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            var dispose = React.Scope(out var scope);
            var disposeEditMode = Empty.Disposable;
            var disposePlayMode = Empty.Disposable;
            (SchPub.PubError, Sch.OnError) = React.PubSub<Exception>(scope); //TODO

            var (pubUpdate, onUpdate) = React.PubSub(scope);

//            EditorApplication.projectChanged += () => Debug.Log("Changed");

            EditorApplication.playModeStateChanged += OnEditorApplicationOnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += BeforeReload;

            void BeforeReload()
            {
                dispose.Dispose();
            }

            Sch.Scope = scope;
            Sch.Update = onUpdate;

            EditorApplication.update += OnUpdate;

            void OnEditorApplicationOnPlayModeStateChanged(PlayModeStateChange change)
            {
                switch (change)
                {
                    case PlayModeStateChange.EnteredEditMode:
                    {
                        disposeEditMode = scope.Scope(out var editMode);
                        (SchPub.PubError, Sch.OnError) = React.PubSub<Exception>(editMode); //TODO
                        EditMode(editMode).GetScope(editMode);
                        break;
                    }

                    case PlayModeStateChange.EnteredPlayMode:
                    {
                        disposePlayMode = React.Scope(out var playMode);
                        (SchPub.PubError, Sch.OnError) = React.PubSub<Exception>(playMode); //TODO
                        PlayMode(playMode).GetScope(playMode);
                        break;
                    }

                    case PlayModeStateChange.ExitingEditMode:
                    {
                        disposeEditMode.Dispose();
                        break;
                    }

                    case PlayModeStateChange.ExitingPlayMode:
                    {
                        disposePlayMode.Dispose();
                        break;
                    }
                }
            }

            void OnUpdate()
            {
                pubUpdate.Next();
            }
        }


        static async Routine EditMode(IScope scope)
        {
            var g = new GameObject() {hideFlags = HideFlags.DontSaveInEditor};
            scope.OnDispose(() => Object.DestroyImmediate(g));


            var (pubGizmos, onGizmos) = React.PubSub(scope);
            var (pubHandles, onHandles) = React.PubSub(scope);

            EdSch.Gizmos = onGizmos;
            EdSch.Handles = onHandles;

            var h = g.AddComponent<GizmosHolder>();
            h.Init(pubGizmos, pubHandles);
        }

        static async Routine PlayMode(IScope scope)
        {
        }
    }
#endif
}