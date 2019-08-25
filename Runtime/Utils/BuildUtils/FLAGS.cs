// ----------------------------------------------------------------------------
// The MIT License
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

namespace Utility
{
#if UNITY_EDITOR && M_BUILD_FLAGS
    using Reactors;
    using Asserts;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEditor.Build;

    public static class OnLoad
    {
        [InitializeOnLoadMethod]
        public static void RemoveDefine()
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget);

            if (!ScriptingDefineUtil.TryRemoveFlag(FLAGS.NON_EDITOR, definesString, out definesString))
                return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
            AssetDatabase.SaveAssets();

            Debug.Log($"updated defines to '{PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget)}'");
        }
    }

    [UsedImplicitly]
    internal class RemoveDefine : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            OnLoad.RemoveDefine();
        }
    }

    [UsedImplicitly]
    internal class AddDefine : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var currentTarget = report.summary.platformGroup;
            if (currentTarget == BuildTargetGroup.Unknown) return;

            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget);
            if (!ScriptingDefineUtil.TryAddFlag(FLAGS.NON_EDITOR, definesString, out definesString))
            {
                Warn.Warning(FLAGS.NON_EDITOR);
                return;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
            Debug.Log($"updated defines to '{PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget)}'");
        }
    }
#endif

#if DEBUG_TRACE && !UNITY_EDITOR
    #error flag DEBUG_TRACE should be used only in Editor  
#endif


    public static class FLAGS
    {
        public const string DEBUG_TRACE = "DEBUG_TRACE";

        public const string M_DISABLE_POOLING = "M_DISABLE_POOLING";
        public const string FALSE = "false";
        public const string DEBUG = "DEBUG";
        public const string UNITY_EDITOR = "UNITY_EDITOR";
        public const string NON_EDITOR = "NON_EDITOR";
    }
}