using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class EditorGUILayoutUtil
    {
        private static readonly HashSet<Type> s_NonObsoleteEnumTypes = new HashSet<Type>();
        private static readonly HashSet<Type> s_EnumTypes = new HashSet<Type>();
        public static Enum EnumPopup(Enum selected, params GUILayoutOption[] options)
        {
            InitEnumData(selected.GetType(), true);
            return EditorGUILayout.EnumPopup(selected, options);
        }

        public static Enum EnumPopup(Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            InitEnumData(selected.GetType(), true);
            return EditorGUILayout.EnumPopup(selected, style, options);
        }

        private static void InitEnumData(Type enumType, bool excludeObsolete = true)
        {
#if !UNITY_2019_2_OR_NEWER
            var sets = excludeObsolete ? s_NonObsoleteEnumTypes : s_EnumTypes;
            if (sets.Contains(enumType)) return;

            sets.Add(enumType);
            GetEnumData(enumType, excludeObsolete, out var values, out var displayNames);

            for (var i = 0; i < values.Length; i++)
                displayNames[i] = GetDisplayName(values[i]);
#endif
        }
#if !UNITY_2019_2_OR_NEWER
        private static void GetEnumData(Type enumType, bool excludeObsolete, out Enum[] values, out string[] displayNames)
        {
            //var EnumDataUtility = Type.GetType("UnityEditor.PopupCallbackInfo");
            var EnumDataUtility = typeof(EditorGUI);
            var GetCachedEnumData = EnumDataUtility.GetMethod("GetCachedEnumData", BindingFlags.Static | BindingFlags.NonPublic);
            var EnumData = GetCachedEnumData.Invoke(null, new object[] { enumType, excludeObsolete });
            var valuesInfo = EnumData.GetType().GetField("values");
            values = valuesInfo.GetValue(EnumData) as Enum[];
            var displayNamesInfo = EnumData.GetType().GetField("displayNames");
            displayNames = displayNamesInfo.GetValue(EnumData) as string[];
        }
#endif
        public static string GetDisplayName(Enum value)
        {
            var enumStr = value.ToString();
            var arr = value.GetType().GetMember(enumStr);
            MemberInfo memberInfo = arr.Length > 0 ? arr[0] : null;
            if (memberInfo != null)
            {
#if UNITY_2019_2_OR_NEWER
                var attribute = memberInfo.GetCustomAttribute<InspectorNameAttribute>(false);
                if (attribute != null)
                    return attribute.displayName;
#else
                var attribute = memberInfo.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>(false);
                if (attribute != null)
                    return attribute.Description;
#endif
            }
            return enumStr;
        }


    }
}
