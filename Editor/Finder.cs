using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    public static class Finder
    {
        private const string ASSETS_DIR = "Assets/";
        private const string PACKAGE_DIR = "Packages/";
        private const string BUILDIN_EXTRA = "Resources/unity_builtin_extra";
        private const string DEFAULT_RES = "Library/unity default resources";
        private static MethodInfo s_FindBuiltin;

        [MenuItem("Tools/ReplaceBuiltinShader")]
        public static void FindShader()
        {
            var guids = AssetDatabase.FindAssets("t:Material");
            Debug.Log("========================================");
            foreach (var guid in guids)
            {
                var matPath = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                var shaderPath = AssetDatabase.GetAssetPath(mat.shader);
                if (!shaderPath.StartsWith("Assets/") && !shaderPath.StartsWith("Packages/"))
                {
                    var shaderName = mat.shader.name;
                    if (!shaderPath.Equals(BUILDIN_EXTRA))
                        Debug.LogError($"unkonw shader path:{shaderPath} name:{shaderName}");
                    else
                    {
                        var newshader = Shader.Find(shaderName);
                        if (newshader == mat.shader)
                        {
                            Debug.Log($"cannot find custom shader: {shaderName}, cannot replace buildin shader", mat);
                        }
                        else
                        {
                            var oldQueue = mat.renderQueue;
                            mat.shader = newshader;
                            mat.renderQueue = oldQueue;
                            Debug.Log($"replace shader:{shaderName}:{matPath}", mat);
                        }
                    }

                }
            }


            //Debug.Log("===========================================");
            //foreach (var entry in map)
            //{
            //    Debug.Log($"{entry.Value}", entry.Key);
            //}
        }

        [MenuItem("Tools/Replace Shader/List Standard")]
        public static void ListStandard()
        {
            ForeachMats((mat, matPath) =>
            {
                if (mat.shader.name.Equals("Standard (Specular setup)"))
                    Debug.Log(mat, mat);
            });
        }

        [MenuItem("Tools/Replace Shader/Replace Standard to buildin")]
        public static void ReplaceStandard()
        {
            const string shaderName = "Standard";
            var projectShader = Shader.Find(shaderName);
            if (!projectShader)
            {
                Debug.LogError($"can not fond project shader: {shaderName}");
                return;
            }

            //var buildinShader = FindBuiltinShader("Legacy Shaders/Transparent/Diffuse");
            var buildinShader = FindStandardShader();
            if (!buildinShader)
            {
                Debug.LogError($"can not fond buildin shader: {shaderName}");
                return;
            }
            if (buildinShader == projectShader)
            {
                Debug.LogError($"only one shader: {shaderName}");
                return;
            }

            Debug.Log("=====================================");

            ForeachMats((mat, matPath) =>
            {
                if (mat.shader == projectShader)
                {
                    var oldRenderQueue = mat.renderQueue;
                    mat.shader = buildinShader;
                    mat.renderQueue = oldRenderQueue;
                    Debug.Log($"Replace {matPath}", mat);
                }
            });
        }

        private static List<Material> s_TempMats = new List<Material>();
        private static void ForeachMats(Action<Material, string> action)
        {
            var guids = AssetDatabase.FindAssets("t:Material");
            foreach (var guid in guids)
            {
                var matPath = AssetDatabase.GUIDToAssetPath(guid);
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

        private static void LoadAssetsAtPath<T>(string path, List<T> list) where T : UnityEngine.Object
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is T)
                    list.Add((T)asset);
            }
        }


        private static Shader FindStandardShader()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var shader = cube.GetComponent<Renderer>().sharedMaterial.shader;
            GameObject.DestroyImmediate(cube);
            Debug.Log($"shader {shader.name}, {AssetDatabase.GetAssetPath(shader)}");
            return shader;
        }

        private static Shader FindBuiltinShader(string shaderName)
        {
            if (s_FindBuiltin == null)
                s_FindBuiltin = typeof(Shader).GetMethod("FindBuiltin", BindingFlags.NonPublic | BindingFlags.Static);

            var shader = s_FindBuiltin.Invoke(null, new string[] { shaderName });
            return shader as Shader;
        }

        [MenuItem("Window/LiteFeel/ShaderTools/FindStandard")]
        private static void FindShaders()
        {
            var map = new Dictionary<Shader, Material>();
            var shaderName = "Standard";
            var count = 0;
            ForeachMats((mat, matPath) =>
            {
                if (mat.shader.name == shaderName)
                {
                    map[mat.shader] = mat;
                    count++;
                }
            });
            Debug.Log("=================================");
            Debug.Log(count);
            foreach (var entry in map)
                Debug.Log(entry.Value.name, entry.Value);
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

        internal static int FindMaterials(Shader shader, List<Material> mats)
        {
            if (shader == null || mats == null) return 0;
            var count = mats.Count;
            ForeachMats((mat, matPath) =>
            {
                if (mat.shader == shader)
                    mats.Add(mat);
            });
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