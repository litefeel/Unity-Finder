using System;
using UnityEditor;
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
            for(var i = 0; i < comps.Length; i++)
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
    }
}
