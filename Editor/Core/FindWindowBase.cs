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

        // asset type for search
        private float m_FilterByTypeWith;
        protected GUIContent m_FilterByType;
        protected bool m_IgnoreScearchAssetType;
        protected SearchAssetType m_SearchAssetType;

        // asset folder for serach
        private float m_FilterByFolderWith;
        protected GUIContent m_FilterByFolder;
        protected bool m_IgnoreSearchAssetFolder;
        protected SearchAssetFolder m_SearchAssetFolder;
        protected DefaultAsset m_Folder;

        protected List<string> m_ItemNames = new List<string>();
        protected List<TObject> m_Items = new List<TObject>();


        [SerializeField]
        protected TreeViewState m_TreeViewState;
        [SerializeField]
        protected SimpleTreeView m_SimpleTreeView;

        private string m_FilterStr;

        private GUIStyle m_PopupStyle;
        private bool m_Inited;

        protected virtual void OnEnable()
        {
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();
            m_SimpleTreeView = new SimpleTreeView(m_TreeViewState);
            m_SimpleTreeView.onItemSelect = OnItemSelect;
            m_SimpleTreeView.onItemClick = OnItemClick;
            m_SimpleTreeView.onItemDoubleClick = OnItemDoubleClick;
            m_SimpleTreeView.Items = m_ItemNames;
            m_SimpleTreeView.Reload();
        }

        private void Init()
        {
            if (m_Inited) return;
            m_Inited = true;

            m_PopupStyle = new GUIStyle(EditorStyles.popup);
            m_PopupStyle.fixedHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            m_FilterByType = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Search by Type");
            m_FilterByTypeWith = EditorUtil.CalcLabelSize(EditorGUILayoutUtil.GetDisplayName(SearchAssetType.Prefab), m_PopupStyle);
            
            m_FilterByFolder = new GUIContent(EditorGUIUtility.FindTexture("Project"), "Search by Folder");
            m_FilterByFolderWith = EditorUtil.CalcLabelSize(EditorGUILayoutUtil.GetDisplayName(SearchAssetFolder.AssetsAndPackages), m_PopupStyle);
        }

        private void OnGUI()
        {
            Init();
            if (Event.current.type == EventType.Layout)
                ConfigValues();
            OnGUIBody();
        }

        protected virtual void OnGUIBody()
        {
            EditorGUILayout.BeginHorizontal();
            {
                using (new EditorGUI.DisabledScope(m_DisableFind))
                {
                    GUILayoutOption[] options = null;
                    if (!m_IgnoreSearchAssetFolder)
                        options = new GUILayoutOption[] { GUILayout.Width(EditorGUIUtility.labelWidth) };
                    if (GUILayout.Button("Find", options))
                        DoFind();
                }

                OnGUISearchAssetType();
                OnGUISearchAssetFolder();

            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(m_Message))
                EditorGUILayout.HelpBox(m_Message, MessageType.Warning, true);

            EditorGUILayout.BeginHorizontal();
            {
                var countStr = string.Format("Count:{0}", m_ItemNames.Count);
                EditorGUILayout.LabelField(countStr, GUILayout.Width(EditorUtil.CalcLabelSize(countStr)));
                GUILayout.FlexibleSpace();
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
            switch (m_SearchAssetFolder)
            {
                case SearchAssetFolder.Assets:
                    return new string[] { "Assets" };
                case SearchAssetFolder.AssetsAndPackages:
                    return Array.Empty<string>();
            }
            if (m_Folder == null)
                return Array.Empty<string>();

            return new string[] { AssetDatabase.GetAssetPath(m_Folder) };
        }

        protected virtual void OnGUISearchAssetType()
        {
            if (m_IgnoreScearchAssetType) return;

            EditorGUILayout.DropdownButton(m_FilterByType, FocusType.Passive, EditorStyles.largeLabel, GUILayout.ExpandWidth(false));
            m_SearchAssetType = (SearchAssetType)EditorGUILayoutUtil.EnumPopup(m_SearchAssetType, m_PopupStyle, GUILayout.Width(m_FilterByTypeWith));
        }

        protected virtual void OnGUISearchAssetFolder()
        {
            if (m_IgnoreSearchAssetFolder) return;

            EditorGUILayout.DropdownButton(m_FilterByFolder, FocusType.Passive, EditorStyles.largeLabel, GUILayout.ExpandWidth(false));

            m_SearchAssetFolder = (SearchAssetFolder)EditorGUILayoutUtil.EnumPopup(m_SearchAssetFolder, m_PopupStyle, GUILayout.Width(m_FilterByFolderWith));
            using (new EditorGUI.DisabledScope(m_SearchAssetFolder != SearchAssetFolder.Folder))
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
                        has = FindUtil.InScene(path, InGameObjectAndChildren);
                        break;
                    case GameObject prefab:
                        has = InGameObjectAndChildren(prefab);
                        break;
                }
                if (has)
                {
                    m_Items.Add((TObject)obj);
                    m_ItemNames.Add(path);
                }
            }, true, GetSearchInFolders(), m_SearchAssetType);
            m_SimpleTreeView.Reload();
        }

        protected abstract bool InGameObjectAndChildren(GameObject prefab);

        protected virtual void OnItemSelect(int index)
        {
            EditorGUIUtility.PingObject(m_Items[index]);
        }

        protected virtual void OnItemClick(int index)
        {
            EditorGUIUtility.PingObject(m_Items[index]);
        }

        protected virtual void OnItemDoubleClick(int index)
        {
            AssetDatabase.OpenAsset(m_Items[index]);
        }
    }

}
