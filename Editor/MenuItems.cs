using UnityEditor;
using UnityEngine;

namespace litefeel.Finder.Editor
{
    static class MenuItems
    {
        [MenuItem("Window/LiteFeel/Finder/FindMaterails")]
        static void FindMaterails()
        {
            ShowWindow<FindMaterialsWindow>();
        }
        [MenuItem("Window/LiteFeel/Finder/FindSprites")]
        static void FindSprites()
        {
            ShowWindow<FindSpritesWindow>();
        }
        [MenuItem("Window/LiteFeel/Finder/FindTexts")]
        static void FindTexts()
        {
            ShowWindow<FindTextsWindow>();
        }
        [MenuItem("Window/LiteFeel/Finder/FindFontInPrefab")]
        static void FindFontInPrefab()
        {
            ShowWindow<FindFontInPrefabWindow>();
        }
        [MenuItem("Assets/LiteFeel/Finder/FindScript")]
        static void FindScript()
        {
            ShowWindow<FindScriptWindow>();
        }

        private static void ShowWindow<T>(string title = null) where T : EditorWindow
        {
            if (string.IsNullOrEmpty(title))
            {
                title = typeof(T).Name;
            }
            var window = EditorWindow.GetWindow<T>();
            window.titleContent = new UnityEngine.GUIContent(title);
            window.Show();
        }
    }
}
