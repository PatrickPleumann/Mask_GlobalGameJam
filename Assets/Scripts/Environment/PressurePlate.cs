using Player;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [DisallowMultipleComponent]
    public class PressurePlate : MonoBehaviour
    {
        [SerializeField]
        private List<EPlayerType> m_triggerTypes = new List<EPlayerType>();
        [SerializeField]
        private List<AControllable> m_controlledObjects = new List<AControllable>();
        [SerializeField]
        private Transform m_topPart = null;
        [SerializeField]
        private Transform m_activatedState = null;

        private Vector2 m_initialPosition = Vector2.zero;

        private void Awake()
        {
            m_initialPosition = m_topPart.position;
        }

        private void AdjustPlateState(EPlayerType _previous, EPlayerType _current)
        {
            if (m_triggerTypes.Contains(_previous) && m_triggerTypes.Contains(_current))
            {
                return;
            }

            if (!m_triggerTypes.Contains(_previous) && !m_triggerTypes.Contains(_current))
            {
                return;
            }

            if (m_triggerTypes.Contains(_current))
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        private void Activate()
        {
            foreach (AControllable controllable in m_controlledObjects)
            {
                controllable.Activate();
            }
            m_topPart.position = m_activatedState.position;
        }

        private void Deactivate()
        {
            foreach (AControllable controllable in m_controlledObjects)
            {
                controllable.Deactivate();
            }
            m_topPart.position = m_initialPosition;
        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
            Debug.Log("Pressure Plate Triggered by " + _collision.name);
            PlayerState playerState = _collision.GetComponent<PlayerState>();
            if (playerState == null)
            {
                return;
            }
            playerState.OnPlayerTypeChanged += AdjustPlateState;
            if (!m_triggerTypes.Contains(playerState.CurrentType))
            {
                return;
            }

            Activate();
        }

        private void OnTriggerExit2D(Collider2D _collision)
        {
            PlayerState playerState = _collision.GetComponent<PlayerState>();
            if (playerState == null)
            {
                return;
            }
            playerState.OnPlayerTypeChanged -= AdjustPlateState;
            if (!m_triggerTypes.Contains(playerState.CurrentType))
            {
                return;
            }

            Deactivate();
        }
    }
}
