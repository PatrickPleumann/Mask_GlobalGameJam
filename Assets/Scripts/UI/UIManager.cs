using GameLoop;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Transform m_levelCompletedPanel = null;

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
        }
    }
}