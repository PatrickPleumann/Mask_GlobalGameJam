using GameLoop;
using Player;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [DisallowMultipleComponent]
    public class Spike : MonoBehaviour
    {
        [SerializeField]
        private List<EPlayerType> m_immuneTypes = new List<EPlayerType>();
        [SerializeField]
        private BoxCollider2D m_immunityCollider = null;

        private PlayerState m_playerState = null;

        private void Awake()
        {
            m_playerState = FindFirstObjectByType<PlayerState>();

            m_playerState.OnPlayerTypeChanged += AdjustImmunityCollider;
        }

        private void AdjustImmunityCollider(EPlayerType _previous, EPlayerType _current)
        {
            m_immunityCollider.enabled = m_immuneTypes.Contains(_current);
        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag("Player"))
            {
                if (!m_immuneTypes.Contains(m_playerState.CurrentType))
                {
                    GameManager.Instance.KillPlayer();
                }
            }
        }
    }
}
