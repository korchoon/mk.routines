using Mk.Routines;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer (typeof (Routine), true)]
class RoutineDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI (SerializedProperty property) {
        var root = new VisualElement ();
        root.Add (new PropertyField (property.FindPropertyRelative ("__Await")));
        root.Add (new Button (getValue ().Tick) { text = "Tick" });
        root.Add (new Button (getValue ().Dispose) { text = "Dispose" });
        return root;

        Routine getValue () {
            var res = fieldInfo.GetValue (property.serializedObject.targetObject);
            return (Routine)res;
        }
    }
}