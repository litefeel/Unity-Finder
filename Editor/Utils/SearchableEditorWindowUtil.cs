using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using static UnityEditor.SearchableEditorWindow;

namespace litefeel.Finder.Editor
{
    static class SearchableEditorWindowUtil
    {
        static FieldInfo m_SearchableWindows;
        static FieldInfo m_HierarchyType;
        static MethodInfo m_SetSearchFilter;

        public static void ForEach(Action<SearchableEditorWindow> action, HierarchyType type)
        {
            foreach (var window in GetSearchableWindows())
            {
                if (GetHierarchyType(window) == type)
                    action(window);
            }
        }

        public static void SetSearchFilter(this SearchableEditorWindow win, string searchFilter, SearchMode mode)
        {
            if (m_SetSearchFilter == null)
                m_SetSearchFilter = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            m_SetSearchFilter.Invoke(win, new object[] { searchFilter, mode, false, false });
        }
        private static List<SearchableEditorWindow> GetSearchableWindows()
        {
            if (m_SearchableWindows == null)
                m_SearchableWindows = typeof(SearchableEditorWindow).GetField("searchableWindows", BindingFlags.Static | BindingFlags.NonPublic);
            return m_SearchableWindows.GetValue(null) as List<SearchableEditorWindow>;
        }

        private static HierarchyType GetHierarchyType(SearchableEditorWindow win)
        {
            if (m_HierarchyType == null)
                m_HierarchyType = typeof(SearchableEditorWindow).GetField("m_HierarchyType", BindingFlags.Instance | BindingFlags.NonPublic);
            return (HierarchyType)m_HierarchyType.GetValue(win);
        }
    }
}
