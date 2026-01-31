using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    public class LevelSelectionManager : MonoBehaviour
    {
        [SerializeField]
        private List<LevelInfo> m_levelInfos = null;
        [SerializeField]
        private LevelSelectionTile m_tilePrefab = null;
        [SerializeField]
        private Transform m_tileParent = null;
        [SerializeField]
        private Button m_playSelectedButton = null;
        [SerializeField]
        private TextMeshProUGUI m_playSelectedText = null;
        [SerializeField]
        private string m_playTextFormat = "Play {0}";

        private List<LevelSelectionTile> m_tiles = new List<LevelSelectionTile>();
        private LevelSelectionTile m_selectedTile = null;

        private void Start()
        {
            foreach (LevelInfo info in m_levelInfos)
            {
                LevelSelectionTile tile = Instantiate(m_tilePrefab, m_tileParent);
                tile.Initialize(info);
                m_tiles.Add(tile);
                tile.OnClick += SelectTile;
            }

            m_playSelectedButton.interactable = false;
            m_playSelectedText.text = "Select a Level";
        }

        private void OnEnable()
        {
            m_playSelectedButton.interactable = false;
            m_playSelectedText.text = "Select a Level";
            m_selectedTile = null;
        }

        private void OnDestroy()
        {
            foreach (LevelSelectionTile tile in m_tiles)
            {
                if (tile == null)
                {
                    continue;
                }

                tile.OnClick -= SelectTile;
            }
        }

        public void PlaySelected()
        {
            if (m_selectedTile == null)
            {
                return;
            }
            SceneManager.LoadScene(m_selectedTile.AssociatedLevelInfo.SceneName);
        }

        private void SelectTile(LevelSelectionTile _tile)
        {
            m_selectedTile = _tile;
            m_playSelectedText.text = string.Format(m_playTextFormat, _tile.AssociatedLevelInfo.LevelName);
            m_playSelectedButton.interactable = true;
        }
    }
}
