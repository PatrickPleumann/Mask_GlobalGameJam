using Player;
using UnityEngine;

namespace GameLoop
{
    [DisallowMultipleComponent]
    public class StartingPoint : MonoBehaviour
    {
        private PlayerState m_activePlayer = null;

        private void Awake()
        {
            m_activePlayer = FindFirstObjectByType<PlayerState>();
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.enabled = false;
        }

        public PlayerState ResetPlayer()
        {
            m_activePlayer.transform.position = transform.position;
            m_activePlayer.transform.rotation = transform.rotation;

            return m_activePlayer;
        }
    }
}
