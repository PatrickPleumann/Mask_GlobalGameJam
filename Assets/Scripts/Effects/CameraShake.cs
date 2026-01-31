using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Effects
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve m_frequencyCurve = null;
        [SerializeField]
        private float m_shakeDuration = 1.0f;
        [SerializeField]
        private List<EPlayerType> m_enabledTypes = new List<EPlayerType>();

        private Coroutine m_shakeCoroutine = null;
        private PlayerState m_playerState = null;

        private void Awake()
        {
            m_playerState = FindFirstObjectByType<PlayerState>();
        }

        private void Start()
        {
            InputSystem.actions.FindAction("Jump").performed += OnPlayerJump;
        }

        private void OnPlayerJump(InputAction.CallbackContext _context)
        {
            if (m_enabledTypes.Contains(m_playerState.CurrentType) && Time.timeScale > 0.0f)
            {
                Shake();
            }
        }

        public void Shake()
        {
            if (m_shakeCoroutine != null)
            {
                StopCoroutine(m_shakeCoroutine);
            }
            m_shakeCoroutine = StartCoroutine(ShakeAsync());
        }

        private IEnumerator ShakeAsync()
        {
            float elapsedTime = 0.0f;
            Vector3 initialPosition = transform.localPosition;
            yield return new WaitForSeconds(0.3f);
            while (elapsedTime < m_shakeDuration)
            {
                elapsedTime += Time.deltaTime;
                float yPosition = m_frequencyCurve.Evaluate(elapsedTime);
                transform.localPosition = initialPosition + Vector3.up * yPosition;
                yield return null;
            }
            transform.localPosition = initialPosition;
        }
    }
}