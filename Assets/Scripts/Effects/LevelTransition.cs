using System.Collections;
using UnityEngine;

namespace Effects
{
    [DisallowMultipleComponent]
    public class LevelTransition : MonoBehaviour
    {
        public static LevelTransition Instance { get; private set; }

        public event System.Action OnTransitionCompleted
        {
            add
            {
                m_onTransitionCompleted -= value;
                m_onTransitionCompleted += value;
            }
            remove
            {
                m_onTransitionCompleted -= value;
            }
        }

        [SerializeField]
        private float m_maxSize = 30.0f;
        [SerializeField]
        private float m_minSize = 0.0f;
        [SerializeField]
        private float m_transitionTime = 2.0f;
        [SerializeField]
        private Transform m_blackout = null;

        private event System.Action m_onTransitionCompleted;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void FadeIn()
        {
            StartCoroutine(FadeAsync(m_minSize, m_maxSize));
        }

        public void FadeOut()
        {
            StartCoroutine(FadeAsync(m_maxSize, m_minSize));
        }

        private IEnumerator FadeAsync(float _start, float _target)
        {
            float elapsedTime = 0.0f;
            Vector3 initialScale = new Vector3(_start, _start, _start);
            transform.localScale = initialScale;
            m_blackout.gameObject.SetActive(true);

            while (elapsedTime < 1.0f)
            {
                elapsedTime += Time.deltaTime / m_transitionTime;
                float scaleValue = Mathf.Lerp(_start, _target, elapsedTime);
                transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                yield return null;
            }

            transform.localScale = new Vector3(_target, _target, _target);
            m_blackout.gameObject.SetActive(false);
            m_onTransitionCompleted?.Invoke();
        }
    }
}
