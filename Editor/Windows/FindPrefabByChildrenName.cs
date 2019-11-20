using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByChildrenName : FindWindowBase<UnityEngine.Object>
    {
        protected string m_ChildName;
        protected bool m_EnabledFindInScene;

        protected override void ConfigValues()
        {
            m_EnabledFindInScene = !string.IsNullOrEmpty(m_ChildName);
        }

        protected override void OnGUIBody()
        {
            EditorGUILayout.BeginHorizontal();
            {
                m_ChildName = EditorGUILayout.TextField(m_ChildName);
                OnGUIFindInScene();
            }
            EditorGUILayout.EndHorizontal();

            base.OnGUIBody();
        }

        protected virtual void OnGUIFindInScene()
        {
            if (m_EnabledFindInScene)
            {
                if (GUILayout.Button("FindInScene", GUILayout.ExpandWidth(false)))
                    OnClickFindInScene();
            }
        }

        protected virtual void OnClickFindInScene()
        {
            SearchableEditorWindowUtil.ForEach((win) =>
            {
                win.SetSearchFilter(m_ChildName, SearchableEditorWindow.SearchMode.All);
            }, HierarchyType.GameObjects);
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return UnityUtil.AnyOneTransformAndChildren((trans) =>
            {
                return trans.name.Contains(m_ChildName);
            }, prefab.transform);
        }
    }
}

