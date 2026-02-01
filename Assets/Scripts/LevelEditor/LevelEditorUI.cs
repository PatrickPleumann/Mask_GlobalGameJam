using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.UI
{
    [DisallowMultipleComponent]
    public class LevelEditorUI : MonoBehaviour
    {
        public static LevelEditorUI Instance { get; private set; }

        public event System.Action<Placeholder> OnPlaceholderTileSelected
        {
            add
            {
                m_onPlaceholderTileSelected -= value;
                m_onPlaceholderTileSelected += value;
            }
            remove
            {
                m_onPlaceholderTileSelected -= value;
            }
        }

        [SerializeField]
        private PlaceholderTile m_tilePrefab = null;
        [SerializeField]
        private Transform m_tilePanel = null;
        [SerializeField]
        private Transform m_tileContainer = null;

        private event System.Action<Placeholder> m_onPlaceholderTileSelected;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        public void Initialize(List<PlaceholderThumbnail> _availablePlaceholders)
        {
            foreach (PlaceholderThumbnail placeholderThumbnail in _availablePlaceholders)
            {
                PlaceholderTile tile = Instantiate(m_tilePrefab, m_tileContainer);
                tile.Initialize(placeholderThumbnail);
                Button button = tile.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    m_onPlaceholderTileSelected?.Invoke(placeholderThumbnail.PlaceholderPrefab);
                });
            }
        }

        public void SetContainerVisibility(bool _isVisible)
        {
            m_tilePanel.gameObject.SetActive(_isVisible);
        }
    }
}
