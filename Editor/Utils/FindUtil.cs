using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class FindUtil
    {
        private static HashSet<Type> s_IgnoreTypes = new HashSet<Type>()
        {
            typeof(Transform),
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(CanvasRenderer),
            typeof(UnityEngine.UI.Outline),
            
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IgnoreType(Type type)
        {
            return s_IgnoreTypes.Contains(type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IgnoreType(Component obj)
        {
            return obj != null && s_IgnoreTypes.Contains(obj.GetType());
        }
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

        public static bool InGameObject(GameObject go, Material material)
        {
            return UnityUtil.AnyOneMaterialAndChildren((mat) =>
            {
                return mat == material;
            }, go);
        }
        public static bool InGameObject(GameObject go, Shader shader)
        {
            return UnityUtil.AnyOneMaterialAndChildren((mat) =>
            {
                if (mat == null) return false;
                return mat.shader == shader;
            }, go);
        }

        public static bool InGameObject(GameObject go, AudioClip clip)
        {
            return UnityUtil.AnyOneComponentAndChildren<AudioSource>((source) =>
            {
                return source.clip == clip;
            }, go.transform);
        }


        public static void FilterCurrentStageOrScene(Func<Transform, bool> func, List<Transform> results)
        {
            using (var scope = ListPoolScope<Transform>.Create())
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
                    if (comp && UnityUtil.AnyOneProperty(CheckMissingProp, comp))
                        return true;
                }
            }
            return false;
        }

        public static bool CheckMissingProp(SerializedProperty prop)
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference
                && EditorUtil.IsMissing(prop))
                return true;

            return false;
        }
        #endregion
    }
}
