using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace litefeel.Finder.Editor
{
    class FindPrefabByScript : FindAssetWindowBase<MonoScript, UnityEngine.Object>
    {
        private System.Type m_ScriptType;
        private string m_ScriptFullName;

        protected override void ConfigValues()
        {
            m_Message = null;
            if (m_Asset != null)
            {
                m_ScriptType = m_Asset.GetClass();
                if (!typeof(Component).IsAssignableFrom(m_ScriptType))
                {
                    m_ScriptFullName = null;
                    m_Message = "Script must be inherit from UnityEngine.Component";
                }
                else
                    m_ScriptFullName = m_ScriptType.FullName;
            }
            m_ScriptType = TypeCache.FindType(m_ScriptFullName);
            
            m_DisableFind = m_ScriptType == null;
            m_EnabledFindInScene = m_ScriptType != null;
        }

        protected override void OnBeforeAssetUI()
        {
            EditorGUI.BeginChangeCheck();
            m_ScriptFullName = EditorGUILayout.DelayedTextField(m_ScriptFullName);
            if(EditorGUI.EndChangeCheck())
            {
                m_ScriptType = TypeCache.FindType(m_ScriptFullName);
                m_Asset = null;
            }
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return prefab.GetComponentInChildren(m_ScriptType, true);
        }

        protected override string GetFindInSceneSearchFilter(ref SearchableEditorWindow.SearchMode searchMode)
        {
            if (m_Asset != null)
                return $"t:{m_ScriptType.FullName}";
            else
            {
                searchMode = SearchableEditorWindow.SearchMode.Type;
                return m_ScriptType.Name;
            }
        }
    }
}

