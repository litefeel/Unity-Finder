using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace litefeel.Finder.Editor
{
    class FindMissingOnCurrentScene : FindWindowBase<UnityObject>
    {
        protected override void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();
            bool has = false;
            var activeObject = Selection.activeObject;
            if(activeObject != null && EditorUtility.IsPersistent(activeObject))
            {
                var path = AssetDatabase.GetAssetPath(activeObject);
                var ext = System.IO.Path.GetExtension(path);
                switch(ext)
                {
                    case ".prefab":
                        has = InGameObject((GameObject)activeObject);
                        break;
                    case ".unity":
                        has = FindUtil.InScene(path, InGameObject);
                        break;
                }
                if (has)
                {
                    m_Items.Add(activeObject);
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

