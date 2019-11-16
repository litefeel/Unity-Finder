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
    class FindMissingOnCurrentScene : FindWindowBase<Transform>
    {
        protected override void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();

            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                ForeachTransform(stage.prefabContentsRoot.transform, m_Items);
            }
            else
            {
                var count = EditorSceneManager.loadedSceneCount;
                for (var i = 0; i < count; i++)
                {
                    var gos = ListPool<GameObject>.Get();
                    var scene = EditorSceneManager.GetSceneAt(i);
                    scene.GetRootGameObjects(gos);
                    foreach (var go in gos)
                    {
                        ForeachTransform(go.transform, m_Items);
                    }
                    ListPool<GameObject>.Release(gos);
                }
            }

            foreach (var item in m_Items)
            {
                m_ItemNames.Add(UnityUtil.GetFullPath(item));
            }

            m_SimpleTreeView.Reload();
        }

        protected override bool InGameObject(GameObject prefab) { return false; }

        private void ForeachTransform(Transform trans, List<Transform> list)
        {
            var comps = ListPool<Component>.Get();
            trans.GetComponents(comps);
            foreach (var comp in comps)
            {
                if (comp == null)
                {
                    list.Add(trans);
                    break;
                }
            }
            ListPool<Component>.Release(comps);
            var count = trans.childCount;
            for (var i = 0; i < count; i++)
                ForeachTransform(trans.GetChild(i), list);
        }

        protected override void OnItemDoubleClick(int index)
        {
            Selection.activeTransform = m_Items[index];
        }

    }
}

