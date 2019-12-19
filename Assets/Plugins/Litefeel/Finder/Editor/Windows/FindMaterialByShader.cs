using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindMaterialByShader : FindAssetWindowBase<Shader, Material>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_IgnoreScearchAssetType = true;
        }

        protected override void DoFind()
        {
            m_Items.Clear();
            Finder.FindMaterials(m_Asset, m_Items, GetSearchInFolders());
            FillMatNames(m_Items);
            m_SimpleTreeView.Reload();
        }

        private void FillMatNames(List<Material> mats)
        {
            m_ItemNames.Clear();

            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                if (!path.EndsWith(".mat"))
                    path += $"/{mats[i].name}.mat";
                m_ItemNames.Add(path);
            }
        }
        protected override bool InGameObjectAndChildren(GameObject prefab)
        {
            throw new System.NotImplementedException();
        }
        protected override void OnItemDoubleClick(int index)
        {
            Selection.activeObject = m_Items[index];
        }

    }
}

