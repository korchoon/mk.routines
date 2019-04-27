using System;
using UnityEditor;

#if !ODIN_INSPECTOR


namespace Sirenix.OdinInspector.Editor
{
    public class OdinEditorWindow : EditorWindow
    {
    }
}

namespace Sirenix.OdinInspector
{
    public class ShowInInspectorAttribute : Attribute
    {
    }

    public class HideReferenceObjectPickerAttribute : Attribute
    {
    }

    public class ListDrawerSettingsAttribute : Attribute
    {
        public bool DraggableItems { get; set; }

        public bool IsReadOnly { get; set; }

        public bool ShowIndexLabels { get; set; }

        public bool HideRemoveButton { get; set; }

        public bool Expanded { get; set; }
    }

    public class ButtonAttribute : Attribute
    {
        public ButtonAttribute(ButtonSizes b)
        {
        }
        
        public ButtonAttribute(){}
    }

    public enum ButtonSizes
    {
        Large
    }

    public class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string multilineEmptyName)
        {
        }
    }

    public class InlinePropertyAttribute : Attribute
    {
    }

    internal class HorizontalGroupAttribute : Attribute
    {
        public HorizontalGroupAttribute(string s)
        {
        }
    }

    public class PropertyOrderAttribute : Attribute
    {
        public PropertyOrderAttribute(int i)
        {
        }
    }

    public class HideLabelAttribute : Attribute
    {
    }

    public class ReadOnlyAttribute : Attribute
    {
    }
}


#endif