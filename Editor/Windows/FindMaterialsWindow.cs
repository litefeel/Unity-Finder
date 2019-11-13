using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindMaterialsWindow : FinderWindowBase<Shader, Material>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
        }

        protected override void DoFind()
        {
            base.DoFind();
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
    }
}

