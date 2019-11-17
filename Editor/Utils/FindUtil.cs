using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class FindUtil
    {
        public static bool InScene(string scenePath, Func<GameObject, bool> InGameObject)
        {
            bool has = false;
            Finder.ForeachRootGameObjectsInScene((go) =>
            {
                if (InGameObject(go))
                {
                    has = true;
                    return true;
                }
                return false;
            }, scenePath);
            return has;
        }

        public static bool InGameObject(GameObject go)
        {
            var comps = go.GetComponentsInChildren<Component>(true);
            for (var i = 0; i < comps.Length; i++)
            {
                if (comps[i] == null)
                    return true;
            }
            return false;
        }

        public static bool InScene<T>(string scenePath, T mat, Func<GameObject, T, bool> InGameObject)
        {
            bool has = false;
            Finder.ForeachRootGameObjectsInScene((go) =>
            {
                if (InGameObject(go, mat))
                {
                    has = true;
                    return true;
                }
                return false;
            }, scenePath);
            return has;
        }

        public static bool InGameObject(GameObject go, Material material)
        {
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterials == null) continue;
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == material)
                        return true;
                }
            }
            return false;
        }
        public static bool InGameObject(GameObject go, Shader shader)
        {
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (renderer.sharedMaterials == null) continue;
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat && mat.shader == shader)
                        return true;
                }
            }
            return false;
        }

        public static bool InGameObject(GameObject go, AudioClip clip)
        {
            foreach (var source in go.GetComponentsInChildren<AudioSource>())
            {
                if (source.clip == clip)
                    return true;
            }
            return false;
        }


        public static void FilterCurrentStageOrScene(Func<Transform, bool> func, List<Transform> results)
        {
            using(var scope = ListPoolScope<Transform>.Create())
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    stage.prefabContentsRoot.transform.GetComponentsInChildren(true, scope.list);
                }
                else
                {
                    var count = EditorSceneManager.loadedSceneCount;
                    for (var i = 0; i < count; i++)
                    {
                        using (var gos = ListPoolScope<GameObject>.Create())
                        {
                            var scene = EditorSceneManager.GetSceneAt(i);
                            scene.GetRootGameObjects(gos.list);
                            foreach (var go in gos.list)
                            {
                                go.GetComponentsInChildren(true, scope.list);
                            }
                        }
                    }
                }
                for (var i = 0; i < scope.list.Count; i++)
                {
                    if (func(scope.list[i]))
                        results.Add(scope.list[i]);
                }
            }
        }

        #region Check Missing
        public static bool CheckMissingScriptOnTransfrom(Transform trans)
        {
            using (var scope = ListPoolScope<Component>.Create())
            {
                trans.GetComponents(scope.list);
                foreach (var comp in scope.list)
                {
                    if (comp == null)
                        return true;
                }
            }
            return false;
        }

        public static bool CheckMissingPropOnTransfrom(Transform trans)
        {
            using (var scope = ListPoolScope<Component>.Create())
            {
                trans.GetComponents(scope.list);
                foreach (var comp in scope.list)
                {
                    if (comp && CheckMissingProp(comp))
                        return true;
                }
            }
            return false;
        }

        public static bool CheckMissingProp(Component comp)
        {
            var so = new SerializedObject(comp);
            so.Update();
            var prop = so.GetIterator();
            return CheckMissingProp(prop, true);
        }

        public static bool CheckMissingProp(SerializedProperty prop, bool isFirst)
        {
            bool expanded = true;
            SerializedProperty end = null;
            if (!isFirst)
                end = prop.GetEndProperty();
            while (prop.NextVisible(expanded))
            {
                if (!isFirst && SerializedProperty.EqualContents(prop, end))
                    return false;
                if (prop.propertyType == SerializedPropertyType.ObjectReference
                    && EditorUtil.IsMissing(prop))
                    return true;

                if (CheckMissingProp(prop.Copy(), false))
                    return true;

                expanded = false;
            }
            return false;
        }
        #endregion
    }
}
