using System.Collections.Generic;
using Lib.DataFlow;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Lib.Async
{
#if ODIN_INSPECTOR
     using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    public class ScopeTracker : OdinEditorWindow
    {
        [MenuItem("Tools/Scopes")]
        static void OpenWindow()
        {
            var window = GetWindow<ScopeTracker>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        [ShowInInspector] SortableEdList<Scope> _all;

        protected override void OnEnable()
        {
            _all = new SortableEdList<Scope>((scope, scope1) => -scope.List.Count + scope1.List.Count);
            _Scope.OnNew += OnNew;
        }

        void OnNew(_Scope t)
        {
            var sc = new Scope();
            _all.All.Add(sc);

            t.CtorStackTrace += msg => sc.Ctor = msg;
            t.Dispose += () =>
            {
                sc.Disposed = true;
                _all.All.Remove(sc);
            };
            t.Finally += msg => sc.List.Add(msg);
        }


        [InlineProperty, HideReferenceObjectPicker]
        public class Scope
        {
            public StackTraceHolder Ctor;
            public bool Disposed;

            public List<StackTraceHolder> List = new List<StackTraceHolder>();
        }
    }
#endif
}