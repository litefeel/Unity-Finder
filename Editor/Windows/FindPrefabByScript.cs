using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByScript : FinderWindowBase<MonoScript, UnityEngine.Object>
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
            Finder.ForeachPrefabAndScene((obj, path) =>
            {
                bool has = false;
                switch (obj)
                {
                    case SceneAsset _:
                        has = InScene(path);
                        break;
                    case GameObject prefab:
                        has = prefab.GetComponentInChildren(m_ScriptType, true);
                        break;
                }
                if (has)
                {
                    m_Items.Add(obj);
                    m_ItemNames.Add(path);
                }
            }, true, GetSearchInFolders(), m_SearchType);
            m_SimpleTreeView.Reload();
        }


        private bool InScene(string scenePath)
        {
            bool has = false;
            Finder.ForeachRootGameObjectsInScene((go) =>
            {
                if (go.GetComponentInChildren(m_ScriptType, true))
                {
                    has = true;
                    return true;
                }
                return false;
            }, scenePath);
            return has;
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

