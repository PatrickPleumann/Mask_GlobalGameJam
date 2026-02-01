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

        private Coroutine m_shakeCoroutine = null;

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
            yield return new WaitForSeconds(0.6f);
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