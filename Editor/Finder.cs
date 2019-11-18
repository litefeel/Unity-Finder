using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    public static class Finder
    {
        private const string ASSETS_DIR = "Assets/";
        private const string PACKAGE_DIR = "Packages/";


        private static List<Material> s_TempMats = new List<Material>();

        public static void Progress<T>(Action<T> action, T[] list, string title = "Progress Materials", string content = "{0}/{count}")
        {
            var count = list.Length;
            EditorUtility.DisplayCancelableProgressBar("Progressing", $"0/{count}", 0);
            try
            {
                for (var i = 0; i < count; i++)
                {
                    if(EditorUtility.DisplayCancelableProgressBar("Progressing", list[i].ToString(), i / (float)count))
                        return;
                    action(list[i]);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }


        public static void ForeachMats(Action<Material, string> action, bool showProgress = true, string[] searchInFolders = null)
        {

            var guids = AssetDatabase.FindAssets("t:Material", searchInFolders);
            var count = guids.Length;
            if (showProgress)
                EditorUtility.DisplayCancelableProgressBar("Progress Materials", $"0/{count}", 0);
            try
            {
                for (var i = 0; i < count; i++)
                {
                    if (showProgress)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Progress Prefabs", $"{i}/{count}", i / (float)count))
                            break;
                    }
                    var matPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (s_TempMats == null)
                        s_TempMats = new List<Material>();
                    s_TempMats.Clear();
                    LoadAssetsAtPath(matPath, s_TempMats);
                    foreach (var mat in s_TempMats)
                    {
                        //Debug.Log($"matPath: {matPath}, {mat.name}, {mat.shader.name}", mat);
                        action(mat, matPath);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void ForeachPrefabs(Action<GameObject, string> action, bool showProgress, string[] searchInFolders = null)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", searchInFolders);
            var count = guids.Length;
            if (showProgress)
                EditorUtility.DisplayCancelableProgressBar("Progress Prefabs", $"0/{count}", 0);
            try
            {
                for (var i = 0; i < count; i++)
                {
                    if (showProgress)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Progress Prefabs", $"{i}/{count}", i / (float)count))
                            break;
                    }
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    action(go, path);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        public static void ForeachPrefabAndScene(Action<UnityObject, string> action, bool showProgress, string[] searchInFolders = null, SearchAssetType searchType = SearchAssetType.All)
        {
            string filter;
            switch (searchType)
            {
                case SearchAssetType.PrefabOnly: filter = "t:Prefab"; break;
                case SearchAssetType.SceneOnly: filter = "t:Scene"; break;
                default: filter = "t:Prefab t:Scene"; break;
            };
            var guids = AssetDatabase.FindAssets(filter, searchInFolders);
            var count = guids.Length;
            if (showProgress)
                EditorUtility.DisplayCancelableProgressBar("Progress Assets", $"0/{count}", 0);
            try
            {
                for (var i = 0; i < count; i++)
                {
                    if (showProgress)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Progress Assets", $"{i}/{count}", i / (float)count))
                            break;
                    }
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var go = AssetDatabase.LoadAssetAtPath<UnityObject>(path);
                    action(go, path);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void ForeachRootGameObjectsInScene(Func<GameObject, bool> action, string scenePath)
        {
            var sceneCount = EditorSceneManager.sceneCount;
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            if (scene == null) return;

            foreach (var go in scene.GetRootGameObjects())
            {
                if (action.Invoke(go)) break;
            }
            if (EditorSceneManager.sceneCount > sceneCount)
                EditorSceneManager.CloseScene(scene, true);
        }

        private static void LoadAssetsAtPath<T>(string path, List<T> list) where T : UnityEngine.Object
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is T)
                    list.Add((T)asset);
            }
        }

        internal static int FindMaterials(string shaderName, List<Material> mats, ShaderType shaderType)
        {
            var count = mats.Count;
            ForeachMats((mat, matPath) =>
            {
                if (shaderName.Equals(mat.shader.name) && GetShaderType(mat.shader).In(shaderType))
                    mats.Add(mat);
            });
            return mats.Count - count;
        }

        internal static int FindMaterials(Shader shader, List<Material> mats, string[] searchInFolders)
        {
            if (shader == null || mats == null) return 0;
            var count = mats.Count;
            ForeachMats((mat, matPath) =>
            {
                if (mat.shader == shader)
                    mats.Add(mat);
            }, true, searchInFolders);
            return mats.Count - count;
        }

        static ShaderType GetShaderType(Shader shader)
        {
            var path = AssetDatabase.GetAssetPath(shader);
            if (path.StartsWith(ASSETS_DIR))
                return ShaderType.Project;
            if (path.StartsWith(PACKAGE_DIR))
                return ShaderType.Package;
            return ShaderType.Builtin;
        }

    }
}