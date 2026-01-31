using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    [DisallowMultipleComponent]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private float m_transitionDelay = 0.5f;

        private Coroutine m_activeCoroutine = null;

        public void StartGame()
        {
            if (m_activeCoroutine == null)
            {
                m_activeCoroutine = StartCoroutine(StartGameAsync());
            }
            
        }

        public void QuitGame()
        {
            if (m_activeCoroutine == null)
            {
                m_activeCoroutine = StartCoroutine(QuitGameAsync());
            }
        }

        private IEnumerator StartGameAsync()
        {
            yield return new WaitForSecondsRealtime(m_transitionDelay);
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index + 1);
        }

        private IEnumerator QuitGameAsync()
        {
            yield return new WaitForSecondsRealtime(m_transitionDelay);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
