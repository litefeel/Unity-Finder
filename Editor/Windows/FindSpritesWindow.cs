using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    class FindSpritesWindow : FinderWindowBase<Sprite, GameObject>
    {
        protected override void OnGUI()
        {
            base.OnGUI();
            m_DisableFind = m_Asset == null;
        }


        private List<Image> m_Images = new List<Image>();
        protected override void DoFind()
        {
            m_Items.Clear();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                m_Images.Clear();
                prefab.GetComponentsInChildren(true, m_Images);
                foreach (var img in m_Images)
                {
                    Debug.Log($"Sprite {path}, {GetName(img.sprite)}, {GetName(img.overrideSprite)}", img);
                    if (img.sprite == m_Asset || img.overrideSprite == m_Asset)
                    {
                        m_Items.Add(prefab);
                        break;
                    }
                }
            }, true, GetSearchInFolders());
            FillMatNames(m_Items);
        }

        private string GetName(Object obj)
        {
            if (obj != null) return obj.name;
            return null;
        }

        private void FillMatNames(List<GameObject> mats)
        {
            m_ItemNames.Clear();
            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                //if (!path.EndsWith(".mat"))
                //    path += ".mat";
                m_ItemNames.Add(path);
            }
        }
    }
}

