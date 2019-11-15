using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace litefeel.Finder.Editor
{
    class FindFontInPrefab : FindAssetWindowBase<Font, Text>
    {
        protected override void OnGUI()
        {
            m_IgnoreSearchFolder = true;
            base.OnGUI();
            m_DisableFind = m_Asset == null;
        }

        protected override void OnGUISearchFolder()
        {
        }

        protected override void DoFind()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null) return;

            var texts = new List<Text>();
            prefabStage.prefabContentsRoot.GetComponentsInChildren(true, texts);

            m_Items.Clear();
            foreach(var txt in texts)
            {
                if (txt.font == m_Asset)
                    m_Items.Add(txt);
            }
            
            FillMatNames(m_Items);
        }

        private void FillMatNames(List<Text> mats)
        {
            m_ItemNames.Clear();

            for (var i = 0; i < mats.Count; i++)
            {
                m_ItemNames.Add(mats[i].name);
            }
        }

        protected override bool InGameObject(GameObject prefab)
        {
            throw new System.NotImplementedException();
        }
    }
}

