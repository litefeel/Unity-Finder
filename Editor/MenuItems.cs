using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class MenuItems
    {

        [MenuItem("Assets/Finder/Find Prefabs By Shader", priority = 100)]
        static void FindPrefabByShader()
        {
            ShowWindow<FindPrefabByShader>();
        }
        [MenuItem("Assets/Finder/Find Prefabs By Material", priority = 100)]
        static void FindPrefabByMaterial()
        {
            ShowWindow<FindPrefabByMaterial>();
        }
        [MenuItem("Assets/Finder/Find Prefabs By Script", priority = 100)]
        static void FindPrefabByScript()
        {
            ShowWindow<FindPrefabByScript>();
        }
        [MenuItem("Assets/Finder/Find Prefabs By Audio Clip", priority = 100)]
        static void FindPrefabByAudio()
        {
            ShowWindow<FindPrefabByAudio>();
        }
        [MenuItem("Assets/Finder/Find Materials By Shader", priority = 200)]
        static void FindMaterialByShader()
        {
            ShowWindow<FindMaterialByShader>();
        }
        [MenuItem("Assets/Finder/Find Materials By Texture", priority = 200)]
        static void FindMaterialByTexture()
        {
            ShowWindow<FindMaterialByTexture>();
        }
        [MenuItem("Assets/Finder/FindSprite")]
        static void FindSprite()
        {
            ShowWindow<FindSprite>();
        }
        [MenuItem("Assets/Finder/FindText")]
        static void FindText()
        {
            ShowWindow<FindText>();
        }
        [MenuItem("Assets/Finder/FindFontInPrefab")]
        static void FindFontInPrefab()
        {
            ShowWindow<FindFontInPrefab>();
        }


        private static void ShowWindow<T>(string title = null) where T : EditorWindow, IFinderWindow
        {
            if (string.IsNullOrEmpty(title))
            {
                title = typeof(T).Name;
            }
            var window = EditorWindow.GetWindow<T>();
            window.InitAsset(Selection.activeObject);
            window.titleContent = new UnityEngine.GUIContent(title);
            window.Show();
        }
    }
}
