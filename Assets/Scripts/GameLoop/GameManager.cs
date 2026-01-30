using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private List<MaskUsage> m_maskUsages;

        private Dictionary<string, MaskUsage> m_maskUsagePerLevel = null;
        private StartingPoint m_startingPoint = null;

        private Dictionary<EPlayerType, int> m_remainingMasks = new Dictionary<EPlayerType, int>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            m_startingPoint = FindFirstObjectByType<StartingPoint>();

            m_maskUsagePerLevel = new Dictionary<string, MaskUsage>();
            foreach (var maskUsage in m_maskUsages)
            {
                m_maskUsagePerLevel[maskUsage.LevelName] = maskUsage;
            }

            InputSystem.actions.FindAction("Restart").performed += (ctx) => KillPlayer();
        }

        private void Start()
        {
            m_startingPoint.ResetPlayer();
            ResetMaskUsage();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            InputSystem.actions.FindAction("Restart").performed -= (ctx) => KillPlayer();
        }

        private void ResetMaskUsage()
        {
            m_remainingMasks.Clear();
            string currentLevelName = SceneManager.GetActiveScene().name;
            if (m_maskUsagePerLevel.TryGetValue(currentLevelName, out MaskUsage maskUsage))
            {
                foreach (MaskAvailability availability in maskUsage.MaskAvailabilityList)
                {
                    m_remainingMasks[availability.PlayerType] = availability.MaskCount;
                }
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

        public bool CanUseMask(EPlayerType _requestedType)
        {
            if (m_remainingMasks.TryGetValue(_requestedType, out int count) && count > 0)
            {
                m_remainingMasks[_requestedType] = count - 1;
                return true;
            }
            return false;
        }

        private void OnGUI()
        {
            GUILayout.Label("Remaining Masks:");
            foreach (var kv in m_remainingMasks)
            {
                GUILayout.Label($"{kv.Key}: {kv.Value}");
            }
        }
    }
}
