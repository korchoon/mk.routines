using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lib.DataFlow;
using Sirenix.OdinInspector;
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

        public List<Scope> All;

        protected override void OnEnable()
        {
            All = new List<Scope>();
            TraceScope.OnNew += OnNew;

            void OnNew(TraceScope t)
            {
                var sc = new Scope();
                All.Add(sc);

                t.NameOnCreate += msg => sc.Name = msg;
                t.AfterDispose += () => All.Remove(sc);
                t.OnDispose += i => sc.List.Add(i);
            }
        }


        [InlineProperty, HideReferenceObjectPicker, HideLabel]
        public class Scope
        {
            [HideLabel] public string Name;

            public List<DisposeActionInfo> List = new List<DisposeActionInfo>();

            public bool Disposed;
        }
    }
}