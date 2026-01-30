using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private StartingPoint m_startingPoint = null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            m_startingPoint = FindFirstObjectByType<StartingPoint>();
        }

        private void Start()
        {
            m_startingPoint.ResetPlayer();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void KillPlayer()
        {
            Debug.Log("Player has been killed.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void PlayerReachedGoal()
        {
            Debug.Log("Player has reached the goal!");
        }
    }
}
