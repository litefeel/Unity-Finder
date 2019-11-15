using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace litefeel.Finder.Editor
{
    abstract class FindWindowBase<TObject> : EditorWindow where TObject : UnityObject
    {
        protected bool m_DisableFind;
        protected string m_Message;

        protected bool m_IgnoreSearchFolder;
        protected int m_FolderIdx;
        protected string[] m_FolderOptions = new string[] { "All Assets", "Folder" };
        protected DefaultAsset m_Folder;

        protected List<string> m_ItemNames = new List<string>();
        protected List<TObject> m_Items = new List<TObject>();

        protected TreeViewState m_TreeViewState;
        protected SimpleTreeView m_SimpleTreeView;
        protected bool m_EnabledFindInScene;
        protected SearchType m_SearchType;

        private string m_FilterStr;

        private GUIStyle m_PopupStyle;

        protected virtual void OnEnable()
        {
            m_PopupStyle = new GUIStyle(EditorStyles.popup);
            m_PopupStyle.fixedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;


            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();
            m_SimpleTreeView = new SimpleTreeView(m_TreeViewState);
            m_SimpleTreeView.onItemSelect = OnItemSelect;
            m_SimpleTreeView.onItemClick = OnItemClick;
            m_SimpleTreeView.onItemDoubleClick = OnItemDoubleClick;
        }

        protected virtual void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
                ConfigValues();

            EditorGUILayout.BeginHorizontal();
            {
                using (new EditorGUI.DisabledScope(m_DisableFind))
                {
                    GUILayoutOption[] options = null;
                    if (!m_IgnoreSearchFolder)
                        options = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth) };
                    if (GUILayout.Button("Find", options))
                        DoFind();
                }
                var width = EditorUtil.CalcLabelSize(SearchType.SceneOnly.ToString()) + 30;

                m_SearchType = (SearchType)EditorGUILayout.EnumPopup(m_SearchType, m_PopupStyle, GUILayout.Width(width));
                if (!m_IgnoreSearchFolder)
                    OnGUISearchFolder();
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(m_Message))
                EditorGUILayout.HelpBox(m_Message, MessageType.Warning, true);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Count:{0}", m_ItemNames.Count));
            using (new EditorGUI.DisabledScope(m_ItemNames.Count == 0))
            {
                EditorGUILayout.LabelField("Filter:", GUILayout.Width(EditorUtil.CalcLabelSize("Filter:")));
                var tmpStr = EditorGUILayout.TextField(m_FilterStr);
                if (tmpStr != m_FilterStr)
                {
                    m_FilterStr = tmpStr;
                    m_SimpleTreeView.SetFilter(m_FilterStr);
                }
            }
            EditorGUILayout.EndHorizontal();

            var rect = new Rect(2, 0, position.width - 4, position.height);
            rect.y = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2) * 3;
            rect.height -= rect.y + EditorGUIUtility.standardVerticalSpacing;
            GUILayout.BeginArea(rect, EditorStyles.textField);
            {
                m_SimpleTreeView.Items = m_ItemNames;
                rect.position = new Vector2(0, 0);
                m_SimpleTreeView.OnGUI(rect);
                if (m_ItemNames.Count == 0)
                    EditorGUILayout.LabelField("No References");
            }
            GUILayout.EndArea();
        }

        protected virtual void ConfigValues() { }

        protected string[] GetSearchInFolders()
        {
            if (m_FolderIdx == 0 || m_Folder == null)
                return Array.Empty<string>();

            return new string[] { AssetDatabase.GetAssetPath(m_Folder) };
        }

        protected virtual void OnGUISearchFolder()
        {
            m_FolderIdx = GUILayout.SelectionGrid(m_FolderIdx, m_FolderOptions, 2, EditorStyles.radioButton, GUILayout.ExpandWidth(false));
            using (new EditorGUI.DisabledScope(m_FolderIdx == 0))
                m_Folder = EditorGUILayout.ObjectField(m_Folder, typeof(DefaultAsset), false, GUILayout.ExpandWidth(true)) as DefaultAsset;
        }

        protected virtual void DoFind()
        {
            m_Items.Clear();
            m_ItemNames.Clear();
            Finder.ForeachPrefabAndScene((obj, path) =>
            {
                bool has = false;
                switch (obj)
                {
                    case SceneAsset _:
                        has = FindUtil.InScene(path, InGameObject);
                        break;
                    case GameObject prefab:
                        has = InGameObject(prefab);
                        break;
                }
                if (has)
                {
                    m_Items.Add((TObject)obj);
                    m_ItemNames.Add(path);
                }
            }, true, GetSearchInFolders(), m_SearchType);
            m_SimpleTreeView.Reload();
        }

        protected abstract bool InGameObject(GameObject prefab);

        protected virtual void OnItemSelect(int index)
        {
            EditorGUIUtility.PingObject(m_Items[index]);
        }

        protected virtual void OnItemClick(int index)
        {
            EditorGUIUtility.PingObject(m_Items[index]);
        }

        protected virtual void OnItemDoubleClick(int index) { }
    }

}
