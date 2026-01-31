using GameLoop;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    public class MaskTile : MonoBehaviour
    {
        [SerializeField]
        private EPlayerType m_matchingPlayerType = EPlayerType.NORMAL;
        [SerializeField]
        private Sprite m_maskSprite = null;
        [SerializeField]
        private TextMeshProUGUI m_amountText = null;
        [SerializeField]
        private Image m_maskImage = null;

        private void Awake()
        {
            GameManager.Instance.OnMaskUsageChanged += UpdateMaskText;
            m_maskImage.sprite = m_maskSprite;
        }

        private void OnValidate()
        {
            m_maskImage.sprite = m_maskSprite;
        }

        private void UpdateMaskText(EPlayerType _type, int _amount)
        {
            if (m_matchingPlayerType != _type)
            {
                return;
            }
            m_amountText.text = _amount.ToString();
        }
    }
}
