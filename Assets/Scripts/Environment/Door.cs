using System.Collections;
using UnityEngine;

namespace Environment
{
    public class Door : AControllable
    {
        [SerializeField]
        private float m_closeDelay = 1.0f;
        [SerializeField]
        private float m_openSpeed = 2.0f;
        [SerializeField]
        private Transform m_openPosition = null;

        private Vector3 m_initialPosition;
        private Vector3 m_absoluteOpenPosition;

        private Coroutine m_closeCoroutine = null;
        private Coroutine m_openCoroutine = null;

        private void Awake()
        {
            m_initialPosition = transform.position;
            m_absoluteOpenPosition = m_openPosition.position;
        }

        public override void Activate()
        {
            if (m_closeCoroutine != null)
            {
                StopCoroutine(m_closeCoroutine);
                m_closeCoroutine = null;
            }
            if (m_openCoroutine == null)
            {
                m_openCoroutine = StartCoroutine(OpenDoor());
            }
        }

        public override void Deactivate()
        {
            if (m_openCoroutine != null)
            {
                StopCoroutine(m_openCoroutine);
                m_openCoroutine = null;
            }
            if (m_closeCoroutine == null)
            {
                m_closeCoroutine = StartCoroutine(CloseAfterDelay());
            }
        }

        private IEnumerator OpenDoor()
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                transform.position = Vector3.Lerp(startPosition, m_absoluteOpenPosition, elapsedTime);
                elapsedTime += Time.deltaTime * m_openSpeed;
                yield return null;
            }
            transform.position = m_absoluteOpenPosition;
        }

        private IEnumerator CloseAfterDelay()
        {
            yield return new WaitForSeconds(m_closeDelay);
            Vector3 startPosition = transform.position;
            float elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                transform.position = Vector3.Lerp(startPosition, m_initialPosition, elapsedTime);
                elapsedTime += Time.deltaTime * m_openSpeed;
                yield return null;
            }
            transform.position = m_initialPosition;
        }
    }
}
