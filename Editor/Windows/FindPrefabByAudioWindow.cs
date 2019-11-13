using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindPrefabByAudioWindow : FinderWindowBase<AudioClip, GameObject>
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
            var list = new List<AudioSource>();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                list.Clear();
                prefab.GetComponentsInChildren(true, list);
                if (HasAsset(list))
                {
                    m_Items.Add(prefab);
                    m_ItemNames.Add(AssetDatabase.GetAssetPath(prefab));
                }
            }, true, GetSearchInFolders());
            m_SimpleTreeView.Reload();
        }

        private bool HasAsset(List<AudioSource> list)
        {
            foreach(var source in list)
            {
                if (source.clip == m_Asset)
                    return true;
            }
            return false;
        }

        protected override void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }

    }
}

