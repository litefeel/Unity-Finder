using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    public static class SelectionUtil
    {
        private static Object m_Selected;

        public static void Select(Object obj)
        {
            if (obj == null) return;
            if (obj == m_Selected) return;
            m_Selected = obj;
            EditorGUIUtility.PingObject(obj);
            //Selection.activeObject = obj;
        }

        public static void Clear()
        {
            m_Selected = null;
        }
    }
}
