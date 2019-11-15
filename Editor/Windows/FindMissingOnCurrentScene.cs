using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
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

            var list = new List<Transform>();
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if(stage != null)
            {
                ForeachTransform(stage.prefabContentsRoot.transform, list);
            }else
            {
                var count = EditorSceneManager.loadedSceneCount;
                for (var i = 0; i < count; i++)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    foreach(var go in scene.GetRootGameObjects())
                    {
                        ForeachTransform(go.transform, list);
                    }
                }
                
            }

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

        private void ForeachTransform(Transform trans, List<Transform> list)
        {
            foreach(var t in trans.gameObject.GetComponents<Component>())
            {
                if(t == null)
                {
                    list.Add(trans);
                    break;
                }
            }
            var count = trans.childCount;
            for (var i = 0; i < count; i++)
                ForeachTransform(trans.GetChild(i), list);
        }

    }
}

