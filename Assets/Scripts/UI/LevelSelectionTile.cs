using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    public class LevelSelectionTile : MonoBehaviour
    {
        public event System.Action<LevelSelectionTile> OnClick
        {
            add
            {
                m_onClick -= value;
                m_onClick += value;
            }
            remove
            {
                m_onClick -= value;
            }
        }

        public LevelInfo AssociatedLevelInfo => m_associatedInfo;

        [SerializeField]
        private Image m_thumbnail = null;
        [SerializeField]
        private TextMeshProUGUI m_levelName = null;
        [SerializeField]
        private Button m_button = null;

        private event System.Action<LevelSelectionTile> m_onClick;
        private LevelInfo m_associatedInfo = null;

        private void Awake()
        {
            m_button.onClick.AddListener(HandleOnClick);
        }

        private void OnDestroy()
        {
            m_button.onClick.RemoveListener(HandleOnClick);
        }

        public void Initialize(LevelInfo _levelInfo)
        {
            m_associatedInfo = _levelInfo;
            m_thumbnail.sprite = _levelInfo.LevelImage;
            m_levelName.text = _levelInfo.LevelName;
        }

        private void HandleOnClick()
        {
            m_onClick?.Invoke(this);
        }
    }
}
