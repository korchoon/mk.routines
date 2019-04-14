using Lib;
using Utility.AssertN;

namespace Utility
{
#if UNITY_EDITOR
    using System;
    using System.Linq;
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

    public static class FLAGS
    {
        public const string FALSE = "false";
        public const string DEBUG = "DEBUG";
        public const string DEBUG_TRACE_LOG = "DEBUG_TRACE_LOG";
        public const string UNITY_EDITOR = "UNITY_EDITOR";
        public const string NON_EDITOR = "NON_EDITOR";
    }
}