#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Proto;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Drawers
{
    [UsedImplicitly]
    public class UnityInvokeDrawer<T> : OdinValueDrawer<T> where T : UnityEvent
    {
        [Obsolete]
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            this.CallNextDrawer(label);
            if (!Application.isPlaying) return;

            if (GUILayout.Button("Invoke"))
                entry.SmartValue.Invoke();
        }
    }


#if false
    public class Matrix4x4AttributeProcessor<T, T1> : OdinAttributeProcessor<T> where T : UnityEvent<T1>
    {
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return true;
        }

        public override bool CanProcessSelfAttributes(InspectorProperty property)
        {
            return true;
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            if (member is FieldInfo)
            {
                attributes.Add(new LabelWidthAttribute(30));
                attributes.Add(new HorizontalGroupAttribute(member.Name.Substring(0, 2)));
            }

            if (member.Name == "determinant")
            {
                attributes.Add(new ShowInInspectorAttribute());
            }
        }
    }
#endif
}
#endif