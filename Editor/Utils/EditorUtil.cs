﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class EditorUtil
    {
        private static readonly GUIContent s_Text = new GUIContent();

        internal static GUIContent TempContent(string t)
        {
            s_Text.image = null;
            s_Text.text = t;
            s_Text.tooltip = null;
            return s_Text;
        }

        public static float CalcLabelSize(string label, GUIStyle style = null)
        {
            return CalcLabelSize(TempContent(label), style);
        }

        public static float CalcLabelSize(GUIContent label, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.label;
            return style.CalcSize(label).x;
        }

        public static string GetDisplayName(Enum value)
        {
            var arr = value.GetType().GetMember(value.ToString());
            MemberInfo memberInfo = arr.Length > 0 ? arr[0] : null;
            if(memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttribute<InspectorNameAttribute>(false);
                if (attribute != null)
                    return attribute.displayName;
            }
            return value.ToString();
        }

        static System.Reflection.PropertyInfo m_objectReferenceStringValue;
        public static bool IsMissing(SerializedProperty prop)
        {
            if(m_objectReferenceStringValue == null)
                m_objectReferenceStringValue = typeof(SerializedProperty).GetProperty("objectReferenceStringValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var str = m_objectReferenceStringValue.GetValue(prop) as string;
            return str.StartsWith("Missing ");
        }
    }
}
