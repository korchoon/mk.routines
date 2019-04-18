#if ODIN_INSPECTOR
using Game.Proto;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Drawers
{
    [UsedImplicitly]
    public class TodoDrawer : OdinAttributeDrawer<TodoAttribute>
    {
        static float Alpha => EditorGUIUtility.isProSkin ? 0.9f : 0.7f;

        static Color32 GetColor()
        {
            var c = Color.red;
            c.a = Alpha;
            return c; 
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushColor(GetColor(), true);
            SirenixEditorGUI.BeginBox();
            GUIHelper.PopColor();

            this.CallNextDrawer(label);

            SirenixEditorGUI.EndBox();
        }
    }
}
#endif