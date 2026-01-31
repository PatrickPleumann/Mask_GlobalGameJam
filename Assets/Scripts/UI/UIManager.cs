using GameLoop;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform m_levelCompletedPanel = null;
        [SerializeField]
        private Transform m_pausePanel = null;

        private void Awake()
        {
            InputSystem.actions.FindAction("Pause").performed += OnPausePressed;
        }

        private void OnDestroy()
        {
            InputSystem.actions.FindAction("Pause").performed -= OnPausePressed;
        }

        private void OnPausePressed(InputAction.CallbackContext _context)
        {
            if (m_pausePanel.gameObject.activeSelf)
            {
                ContinueGame();
            }
            else
            {
                m_pausePanel.gameObject.SetActive(true);
                GameManager.Instance.Pause();
            }
        }

        private void Start()
        {
            GameManager.Instance.OnLevelCompleted += ShowLevelCompletedPanel;
        }

        private void ShowLevelCompletedPanel()
        {
            m_levelCompletedPanel.gameObject.SetActive(true);
        }

        public void LoadNextLevel()
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }

        public void ContinueGame()
        {
            m_pausePanel.gameObject.SetActive(false);
            GameManager.Instance.Unpause();
        }

        public void RestartGame()
        {
            GameManager.Instance.RestartLevel();
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}