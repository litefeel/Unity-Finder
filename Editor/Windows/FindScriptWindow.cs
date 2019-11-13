using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindScriptWindow : FinderWindowBase<MonoScript, GameObject>
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

        protected override void DoFind()
        {
            base.DoFind();

            m_Items.Clear();
            m_ItemNames.Clear();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                var componets = prefab.GetComponentsInChildren(m_ScriptType, true);
                if (componets != null && componets.Length > 0)
                {
                    m_Items.Add(prefab);
                    m_ItemNames.Add(AssetDatabase.GetAssetPath(prefab));
                }
            }, true, GetSearchInFolders());
            m_SimpleTreeView.Reload();
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

