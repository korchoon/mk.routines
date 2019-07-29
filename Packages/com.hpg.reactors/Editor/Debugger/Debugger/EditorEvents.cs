// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

using System;
using Lib;
using Lib.DataFlow;
using UnityEditor;
using UnityEngine.Events;

namespace MyNamespace
{
    public static class EditorEvents
    {
        public static void EditorApp(IScope scope)
        {
        }

        public class EditorAppSubs
        {
            public EditorAppSubs(IScope scope)
            {
            }
#if name
             internal static UnityAction projectWasLoaded;
            internal static UnityAction editorApplicationQuit;
            public static EditorApplication.ProjectWindowItemCallback projectWindowItemOnGUI;
            public static EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI;
            internal static EditorApplication.CallbackFunction refreshHierarchy;
            internal static EditorApplication.CallbackFunction dirtyHierarchySorting;
            public static EditorApplication.CallbackFunction update;
            public static EditorApplication.CallbackFunction delayCall;
            public static EditorApplication.CallbackFunction searchChanged;
            internal static EditorApplication.CallbackFunction assetLabelsChanged;
            internal static EditorApplication.CallbackFunction assetBundleNameChanged;
            public static EditorApplication.CallbackFunction modifierKeysChanged;
            internal static EditorApplication.CallbackFunction globalEventHandler;
            internal static Func<bool> doPressedKeysTriggerAnyShortcut;
            internal static EditorApplication.CallbackFunction windowsReordered;
            public static EditorApplication.SerializedPropertyCallbackFunction contextualPropertyMenu;
            private static EditorApplication.CallbackFunction delayedCallback;
#endif

            public class Pubs
            {
            }
        }

        public static SceneViewCtx SceneView(IScope scope)
        {
            var res = new SceneViewCtx(out var sceneViewPubs, scope);


            UnityEditor.SceneView.beforeSceneGui += OnSceneViewOnBeforeSceneGui;
            UnityEditor.SceneView.duringSceneGui += OnSceneViewOnDuringSceneGui;

            scope.OnDispose(() =>
            {
                UnityEditor.SceneView.beforeSceneGui -= OnSceneViewOnBeforeSceneGui;
                UnityEditor.SceneView.duringSceneGui -= OnSceneViewOnDuringSceneGui;
            });

            void OnSceneViewOnBeforeSceneGui(SceneView sv) => sceneViewPubs.BeforeSceneGui.Next(sv);
            void OnSceneViewOnDuringSceneGui(SceneView sv) => sceneViewPubs.DuringSceneGui.Next(sv);

            return res;
        }

        public class SceneViewCtx
        {
            public ISub<SceneView> BeforeSceneGui { get; }
            public ISub<SceneView> DuringSceneGui { get; }

            public SceneViewCtx(out Pubs pubs, IScope scope)
            {
                pubs = new Pubs();

                (pubs.BeforeSceneGui, BeforeSceneGui) = scope.PubSub<SceneView>();
                (pubs.DuringSceneGui, DuringSceneGui) = scope.PubSub<SceneView>();
            }

            public class Pubs
            {
                public IPub<SceneView> BeforeSceneGui;
                public IPub<SceneView> DuringSceneGui;
            }
        }
    }
}