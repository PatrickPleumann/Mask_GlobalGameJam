using Player;
using System.Collections.Generic;
using UnityEngine;

namespace GameLoop
{
    [System.Serializable]
    public class MaskAvailability
    {
        public EPlayerType PlayerType => m_playerType;
        public int MaskCount => m_maskCount;
        [SerializeField]
        private EPlayerType m_playerType;
        [SerializeField]
        private int m_maskCount;
    }

    [CreateAssetMenu(fileName = "MaskUsage", menuName = "GameLoop/MaskUsage", order = 1)]
    public class MaskUsage : ScriptableObject
    {
        public string LevelName => m_levelName;
        public List<MaskAvailability> MaskAvailabilityList => m_maskAvailability;

        [SerializeField]
        private string m_levelName;
        [SerializeField]
        private List<MaskAvailability> m_maskAvailability;
    }
}
