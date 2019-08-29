// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
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
            (SchPub.PubError, Sch.OnError) = scope.PubSub<Exception>(); //TODO

            var (pubUpdate, onUpdate) = scope.PubSub();

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
                        (SchPub.PubError, Sch.OnError) = editMode.PubSub<Exception>(); //TODO
                        EditMode(editMode);
                        break;
                    }

                    case PlayModeStateChange.EnteredPlayMode:
                    {
                        disposePlayMode = React.Scope(out var playMode);
                        (SchPub.PubError, Sch.OnError) = playMode.PubSub<Exception>(); //TODO
                        PlayMode(playMode);
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


        static void EditMode(IScope scope)
        {
            var g = new GameObject() {hideFlags = HideFlags.DontSaveInEditor};
            scope.Subscribe(() => Object.DestroyImmediate(g));


            var (pubGizmos, onGizmos) = scope.PubSub();
            var (pubHandles, onHandles) = scope.PubSub();

            EdSch.Gizmos = onGizmos;
            EdSch.Handles = onHandles;

            var h = g.AddComponent<GizmosHolder>();
            h.Init(pubGizmos, pubHandles);
        }

        static void PlayMode(IScope scope)
        {
        }
    }
#endif
}