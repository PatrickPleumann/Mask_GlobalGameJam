using Effects;
using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameLoop
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static int NextSceneIndex { get; set; } = 1;

        public event System.Action OnLevelCompleted
        {
            add
            {
                m_onLevelCompleted -= value;
                m_onLevelCompleted += value;
            }
            remove
            {
                m_onLevelCompleted -= value;
            }
        }
        public event System.Action OnPlayerDied
        {
            add
            {
                m_onPlayerDied -= value;
                m_onPlayerDied += value;
            }
            remove
            {
                m_onPlayerDied -= value;
            }
        }
        public event System.Action<EPlayerType, int> OnMaskUsageChanged
        {
            add
            {
                m_onMaskUsageChanged -= value;
                m_onMaskUsageChanged += value;
            }
            remove
            {
                m_onMaskUsageChanged -= value;
            }
        }

        public MaskUsage CurrentMaskUsage
        {
            get
            {
                return m_maskUsagePerLevel[SceneManager.GetActiveScene().name];
            }
        }

        [SerializeField]
        private List<MaskUsage> m_maskUsages;

        private event System.Action m_onLevelCompleted;
        private event System.Action m_onPlayerDied;
        private event System.Action<EPlayerType, int> m_onMaskUsageChanged;

        private StartingPoint m_startingPoint = null;
        private Dictionary<string, MaskUsage> m_maskUsagePerLevel = null;
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

            InputSystem.actions.FindAction("Restart").performed += OnRestartLevel;
        }

        private void Start()
        {
            Time.timeScale = 1.0f;
            LevelTransition.Instance.FadeIn();
            m_startingPoint.ResetPlayer();
            ResetMaskUsage();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            InputSystem.actions.FindAction("Restart").performed -= OnRestartLevel;
        }

        private void OnRestartLevel(InputAction.CallbackContext _context)
        {
            RestartLevel();
        }

        public void RestartLevel()
        {
            GameManager.NextSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene("Reload");
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
                    m_onMaskUsageChanged?.Invoke(availability.PlayerType, availability.MaskCount);
                }
            }
        }

        public void KillPlayer()
        {
            LevelTransition.Instance.OnTransitionCompleted += ResetAfterFade;
            LevelTransition.Instance.FadeOut();
            m_onPlayerDied?.Invoke();
        }

        private void ResetAfterFade()
        {
            LevelTransition.Instance.OnTransitionCompleted -= ResetAfterFade;
            RestartLevel();
        }

        public void PlayerReachedGoal()
        {
            LevelTransition.Instance.OnTransitionCompleted += ProceedAfterFade;
            LevelTransition.Instance.FadeOut();
        }

        public void Pause()
        {
            Time.timeScale = 0.0f;
        }

        public void Unpause()
        {
            Time.timeScale = 1.0f;
        }

        private void ProceedAfterFade()
        {
            LevelTransition.Instance.OnTransitionCompleted -= ProceedAfterFade;
            m_onLevelCompleted?.Invoke();
        }

        public bool CanUseMask(EPlayerType _requestedType)
        {
            if (m_remainingMasks.TryGetValue(_requestedType, out int count) && count > 0)
            {
                m_remainingMasks[_requestedType] = count - 1;
                m_onMaskUsageChanged?.Invoke(_requestedType, m_remainingMasks[_requestedType]);
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
