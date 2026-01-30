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
                foreach (AControllable controllable in m_controlledObjects)
                {
                    controllable.Activate();
                }
            }
            else
            {
                foreach (AControllable controllable in m_controlledObjects)
                {
                    controllable.Deactivate();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
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

            foreach (AControllable controllable in m_controlledObjects)
            {
                controllable.Activate();
            }
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

            foreach (AControllable controllable in m_controlledObjects)
            {
                controllable.Deactivate();
            }
        }
    }
}
