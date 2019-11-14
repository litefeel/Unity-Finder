using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    using static FindUtil;
    class FindPrefabByMaterial : FinderWindowBase<Material, UnityEngine.Object>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_EnabledFindInScene = m_Asset != null;
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
                        has = InScene(path, m_Asset, InGameObject);
                        break;
                    case GameObject prefab:
                        has = InGameObject(prefab, m_Asset);
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

        
        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }
    }
}

