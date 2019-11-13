using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class MenuItems
    {
        [MenuItem("Assets/Finder/FindMaterialByShader")]
        static void FindMaterialByShader()
        {
            ShowWindow<FindMaterialByShader>();
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
        [MenuItem("Assets/Finder/FindPrefabByScript")]
        static void FindPrefabByScript()
        {
            ShowWindow<FindPrefabByScript>();
        }
        [MenuItem("Assets/Finder/FindPrefabByAudio")]
        static void FindPrefabByAudio()
        {
            ShowWindow<FindPrefabByAudio>();
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
