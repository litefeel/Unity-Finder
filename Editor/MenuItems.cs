using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class MenuItems
    {
        [MenuItem("Assets/Finder/FindMaterails")]
        static void FindMaterails()
        {
            ShowWindow<FindMaterialsWindow>();
        }
        [MenuItem("Assets/Finder/FindSprites")]
        static void FindSprites()
        {
            ShowWindow<FindSpritesWindow>();
        }
        [MenuItem("Assets/Finder/FindTexts")]
        static void FindTexts()
        {
            ShowWindow<FindTextsWindow>();
        }
        [MenuItem("Assets/Finder/FindFontInPrefab")]
        static void FindFontInPrefab()
        {
            ShowWindow<FindFontInPrefabWindow>();
        }
        [MenuItem("Assets/Finder/FindScript")]
        static void FindScript()
        {
            ShowWindow<FindScriptWindow>();
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
