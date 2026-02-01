using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.UI
{
    [DisallowMultipleComponent]
    public class LoadModal : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_content;
        [SerializeField]
        private Button m_itemPrefab;

        private Dictionary<string, Button> m_items = new Dictionary<string, Button>();
        private string m_selectedLevel = string.Empty;

        private void OnEnable()
        {
            foreach (var item in m_items)
            {
                Destroy(item.Value.gameObject);
            }
            m_items.Clear();

            List<string> levelNames = LevelEditorManager.Instance.GetSavedLevels();
            foreach (string level in levelNames)
            {
                Button newItem = Instantiate(m_itemPrefab, m_content);
                newItem.onClick.AddListener(() =>
                {
                    m_selectedLevel = level;
                });
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = level;
                m_items.Add(level, newItem);
            }
            m_content.sizeDelta = new Vector2(m_content.sizeDelta.x, levelNames.Count * 50f);
        }

        public void LoadSelected()
        {
            if (string.IsNullOrEmpty(m_selectedLevel))
            {
                return;
            }

            try
            {
                LevelEditorManager.Instance.Load(m_selectedLevel);
            }
            catch (System.Exception _ex)
            {
                Debug.LogError($"Failed to load level '{m_selectedLevel}': {_ex.Message}");
                // TODO: Inform the user
                return;
            }

            this.gameObject.SetActive(false);
        }
    }
}
