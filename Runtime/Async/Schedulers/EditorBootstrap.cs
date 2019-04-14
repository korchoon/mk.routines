using System;
using Lib;
using Lib.Async;
using Lib.DataFlow;
using UnityEditor;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PingPong
{
#if UNITY_EDITOR
    using UnityEditor;

    public static class EditorBootstrap
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            var disposeEditMode = Empty.Disposable;
            var disposePlayMode = Empty.Disposable;
            (SchPub.PubError, Sch.OnError) = React.Channel<Exception>(Empty.Scope()); //TODO
 
            var (pubUpdate, onUpdate) = React.Channel(Empty.Scope());

//            EditorApplication.projectChanged += () => Debug.Log("Changed");

            var dispose = React.Scope(out var scope);
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

                        disposeEditMode = React.Scope(out var editMode);
                        (SchPub.PubError, Sch.OnError) = React.Channel<Exception>(editMode); //TODO
                        EditMode(editMode).Scope(editMode);
                        break;
                    }

                    case PlayModeStateChange.EnteredPlayMode:
                    {
                        disposePlayMode = React.Scope(out var playMode);
                        (SchPub.PubError, Sch.OnError) = React.Channel<Exception>(playMode); //TODO
                        PlayMode(playMode).Scope(playMode);
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


            var (pubGizmos, onGizmos) = React.Channel(scope);
            var (pubHandles, onHandles) = React.Channel(scope);

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