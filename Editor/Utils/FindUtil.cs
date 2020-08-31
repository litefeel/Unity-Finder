using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                using (var scope = ListPoolScope<Transform>.Create())
                {
                    stage.prefabContentsRoot.GetComponentsInChildren(true, scope.list);
                    FilterList(func, scope.list, results);
                }
            }
            else
            {
                var count = EditorSceneManager.loadedSceneCount;
                for (var i = 0; i < count; i++)
                    FilterScene(EditorSceneManager.GetSceneAt(i), func, results);

                if (Application.isPlaying)
                {
                    // DontDestroyOnLoad Scene
                    var go = new GameObject();
                    go.hideFlags = HideFlags.HideAndDontSave;
                    GameObject.DontDestroyOnLoad(go);
                    FilterScene(go.scene, func, results);
                    GameObject.DestroyImmediate(go);
                }
            }
        }

        public static void FilterScene(Scene scene, Func<Transform, bool> func, List<Transform> results)
        {
            var gos = ListPool<GameObject>.Get();
            var trans = ListPool<Transform>.Get();

            scene.GetRootGameObjects(gos);
            for (var i = 0; i < gos.Count; i++)
            {
                trans.Clear();
                gos[i].GetComponentsInChildren(true, trans);
                FilterList(func, trans, results);
            }
            ListPool<Transform>.Release(trans);
            ListPool<GameObject>.Release(gos);
        }

        private static void FilterList<T>(Func<T, bool> func, List<T> input, List<T> results)
        {
            for (var i = 0; i < input.Count; i++)
            {
                if (func(input[i]))
                    results.Add(input[i]);
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

        public static bool CheckMissingProp(Component obj)
        {
            if (obj == null) return false;
            var so = new SerializedObject(obj);
            so.Update();
            //Debug.Log(obj);
            var prop = so.GetIterator();
            return UnityUtil.AnyOneProperty(CheckMissingProp, prop, true);
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
