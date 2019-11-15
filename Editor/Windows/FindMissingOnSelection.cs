using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingOnSelection : FindWindowBase<UnityObject>
    {
        protected override void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();
            bool has = false;
            foreach (var obj in Selection.objects)
            {
                if (!EditorUtility.IsPersistent(obj)) continue;

                var path = AssetDatabase.GetAssetPath(obj);
                var ext = System.IO.Path.GetExtension(path);
                switch (ext)
                {
                    case ".prefab":
                        has = InGameObject((GameObject)obj);
                        break;
                    case ".unity":
                        has = FindUtil.InScene(path, InGameObject);
                        break;
                }
                if (has)
                {
                    m_Items.Add(obj);
                    m_ItemNames.Add(path);
                }
            }
            
            m_SimpleTreeView.Reload();
        }
        protected override bool InGameObject(GameObject prefab)
        {
            var comps = prefab.GetComponentsInChildren<Component>(true);
            for(var i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null)
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

