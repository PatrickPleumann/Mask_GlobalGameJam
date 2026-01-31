using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerSprite
    {
        public Sprite Sprite => m_sprite;
        public EPlayerType PlayerType => m_playerType;

        [SerializeField]
        private Sprite m_sprite = null;
        [SerializeField]
        private EPlayerType m_playerType = EPlayerType.NORMAL;
    }

    [DisallowMultipleComponent]
    public class PlayerRepresentation : MonoBehaviour
    {
        [SerializeField]
        private List<PlayerSprite> m_sprites = null;
        [SerializeField]
        private Sprite m_fallback = null;

        private SpriteRenderer m_spriteRenderer = null;
        private PlayerState m_playerState = null;
        private Rigidbody2D m_playerRigidbody = null;
        private Dictionary<EPlayerType, Sprite> m_spritePerType = new Dictionary<EPlayerType, Sprite>();

        private void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_playerState = GetComponentInParent<PlayerState>();
            m_playerRigidbody = GetComponentInParent<Rigidbody2D>();

            m_playerState.OnPlayerTypeChanged += UpdateRepresentation;

            foreach (PlayerSprite playerSprite in m_sprites)
            {
                m_spritePerType[playerSprite.PlayerType] = playerSprite.Sprite;
            }
        }

        private void Update()
        {
            if (Mathf.Abs(m_playerRigidbody.linearVelocity.x) > 0)
            {
                m_spriteRenderer.flipX = m_playerRigidbody.linearVelocity.x < 0f;
            }
        }

        private void UpdateRepresentation(EPlayerType _previous, EPlayerType _current)
        {
            PlayerProperties properties = m_playerState.CurrentProperties;
            m_spriteRenderer.transform.position = m_playerState.transform.position + new Vector3(0f, properties.Height / 2f, 0f);
            if (m_spritePerType.ContainsKey(_current))
            {
                m_spriteRenderer.sprite = m_spritePerType[_current];
                m_spriteRenderer.transform.localScale = Vector3.one;
            }
            else
            {
                m_spriteRenderer.sprite = m_fallback;
                m_spriteRenderer.transform.localScale = new Vector3(properties.Width, properties.Height, 1f);
            }
        }
    }
}
