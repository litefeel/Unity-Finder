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
                if (list.Count > 0)
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

        protected override void OnClickFindInScene()
        {
            //var searchFilter = $"t:{m_ScriptType.FullName}";
            //SearchableEditorWindowUtil.ForEach((win) =>
            //{
            //    win.SetSearchFilter(searchFilter, SearchableEditorWindow.SearchMode.All);
            //}, HierarchyType.GameObjects);
        }

    }
}

