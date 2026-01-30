using UnityEngine;

namespace Player
{
    [DisallowMultipleComponent]
    public class PlayerRepresentation : MonoBehaviour
    {
        private SpriteRenderer m_spriteRenderer = null;
        private PlayerState m_playerState = null;

        private void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_playerState = GetComponentInParent<PlayerState>();

            m_playerState.OnPlayerTypeChanged += UpdateRepresentation;
        }

        private void UpdateRepresentation(EPlayerType _previous, EPlayerType _current)
        {
            PlayerProperties properties = m_playerState.CurrentProperties;
            m_spriteRenderer.transform.localScale = new Vector3(properties.Width, properties.Height, 1f);
            m_spriteRenderer.transform.position = m_playerState.transform.position + new Vector3(0f, properties.Height / 2f, 0f);
            m_spriteRenderer.color = _current switch
            {
                EPlayerType.NORMAL => Color.white,
                EPlayerType.WIDE => Color.green,
                EPlayerType.HEAVY => Color.red,
                EPlayerType.NARROW => Color.blue,
                EPlayerType.LIQUID => Color.cyan,
                _ => Color.white,
            };
        }
    }
}
