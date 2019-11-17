using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindMaterialByTexture : FindAssetWindowBase<Texture, Material>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
            m_IgnoreScearchAssetType = true;
        }

        protected override void DoFind()
        {
            m_Items.Clear();
            Finder.ForeachMats((mat, matPath) =>
            {
                if (InMaterial(mat, m_Asset))
                    m_Items.Add(mat);
            }, true, GetSearchInFolders());
            FillMatNames(m_Items);
            m_SimpleTreeView.Reload();
        }

        private bool InMaterial(Material mat, Texture tex)
        {
            return UnityUtil.AnyOneProperty((prop) =>
            {
                return prop.propertyType == SerializedPropertyType.ObjectReference
                    && prop.objectReferenceValue == tex;
            }, mat);
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
        protected override bool InGameObject(GameObject prefab)
        {
            throw new System.NotImplementedException();
        }

    }
}

