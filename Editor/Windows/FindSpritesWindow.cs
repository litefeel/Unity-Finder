using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    public class FindSpritesWindow : EditorWindow
    {

        private Sprite m_Sprite;
        private readonly List<GameObject> m_Mats = new List<GameObject>();
        private string[] m_MatNames;
        private Vector2 m_ScrollPos = Vector2.zero;
        private int m_SelectedIdx = 0;

        private void OnGUI()
        {
            m_Sprite = EditorGUILayout.ObjectField("Sprite", m_Sprite, typeof(Sprite), false) as Sprite;
            using (new EditorGUI.DisabledScope(m_Sprite == null))
            {
                if (GUILayout.Button("Find"))
                    FindSprites();
            }
            var count = m_MatNames != null ? m_MatNames.Length : 0;
            EditorGUILayout.LabelField(string.Format("Count:{0}", count));
            if (count > 0)
            {
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
                if (m_SelectedIdx >= m_MatNames.Length)
                    m_SelectedIdx = 0;
                m_SelectedIdx = GUILayout.SelectionGrid(m_SelectedIdx, m_MatNames, 1, EditorStyles.miniButton, GUILayout.ExpandHeight(false));
                EditorGUILayout.EndScrollView();

                SelectionUtil.Select(m_Mats[m_SelectedIdx]);
            }
        }


        private List<Image> m_Images = new List<Image>();
        private void FindSprites()
        {
            m_Mats.Clear();
            Finder.ForeachPrefabs((prefab, path) =>
            {
                m_Images.Clear();
                prefab.GetComponentsInChildren(true, m_Images);
                foreach (var img in m_Images)
                {
                    Debug.Log($"Sprite {path}, {GetName(img.sprite)}, {GetName(img.overrideSprite)}", img);
                    if (img.sprite == m_Sprite || img.overrideSprite == m_Sprite)
                    {
                        m_Mats.Add(prefab);
                        break;
                    }
                }
            }, true);
            FillMatNames(m_Mats, ref m_MatNames);
            Debug.Log($"XXX {m_Mats.Count}");
        }

        private string GetName(Object obj)
        {
            if (obj != null) return obj.name;
            return null;
        }

        private void FillMatNames(List<GameObject> mats, ref string[] matNames)
        {
            if (matNames == null || matNames.Length != mats.Count)
                matNames = new string[mats.Count];

            for (var i = 0; i < mats.Count; i++)
            {
                var path = AssetDatabase.GetAssetPath(mats[i]);
                //if (!path.EndsWith(".mat"))
                //    path += ".mat";
                matNames[i] = path;
            }
        }
    }
}

