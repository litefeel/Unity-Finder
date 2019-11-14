using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    class FindMaterialByTexture : FinderWindowBase<Texture, Material>
    {
        protected override void ConfigValues()
        {
            m_DisableFind = m_Asset == null;
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
            var so = new SerializedObject(mat);
            so.Update();
            var property = so.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference
                    && property.objectReferenceValue == tex)
                    return true;
            }
            return false;
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
        protected override bool InGameObject(GameObject prefab, Texture m_Asset)
        {
            throw new System.NotImplementedException();
        }

    }
}

