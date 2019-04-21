using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Lib.Async
{
    public class Scope
    {
        public List<Disposable> _disposables;

        public class Disposable
        {
        }
    }

    public class ScopeTracker : OdinEditorWindow
    {
        [MenuItem("Tools/Scopes")]
        private static void OpenWindow()
        {
            var window = GetWindow<ScopeTracker>();

            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }


        protected override void OnEnable()
        {
        }
    }
}