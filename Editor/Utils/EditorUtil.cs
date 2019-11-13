using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.SearchableEditorWindow;

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
    }
}
