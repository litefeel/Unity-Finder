using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByScript : FindAssetWindowBase<MonoScript, UnityEngine.Object>
    {
        private System.Type m_ScriptType;

        protected override void ConfigValues()
        {
            m_Message = null;
            m_ScriptType = null;
            if (m_Asset != null)
            {
                m_ScriptType = m_Asset.GetClass();
                if (!typeof(Component).IsAssignableFrom(m_ScriptType))
                {
                    m_ScriptType = null;
                    m_Message = "Script must be inherit from UnityEngine.Component";
                }
            }
            m_DisableFind = m_ScriptType == null;
            m_EnabledFindInScene = m_ScriptType != null;
        }

        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            return prefab.GetComponentInChildren(m_ScriptType, true);
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

        protected override string GetFindInSceneSearchFilter()
        {
            return $"t:{m_ScriptType.FullName}";
        }

    }
}

