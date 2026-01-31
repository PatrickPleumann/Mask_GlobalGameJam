using GameLoop;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PropertiesPerType
    {
        public EPlayerType PlayerType => m_playerType;
        public PlayerProperties Properties => m_properties;

        [SerializeField]
        private EPlayerType m_playerType;
        [SerializeField]
        private PlayerProperties m_properties;

        override public string ToString()
        {
            return $"{m_playerType} : {m_properties.name}";
        }
    }

    [DisallowMultipleComponent]
    public class PlayerState : MonoBehaviour
    {
        public PlayerProperties CurrentProperties => m_propertiesPerType[m_currentType];
        public EPlayerType CurrentType
        {
            get => m_currentType;
            set
            {
                if (m_currentType == value)
                {
                    return;
                }

                EPlayerType previous = m_currentType;
                m_currentType = value;

                m_onPlayerTypeChanged(previous, m_currentType);
            }
        }
        public event System.Action<EPlayerType, EPlayerType> OnPlayerTypeChanged
        {
            add
            {
                m_onPlayerTypeChanged -= value;
                m_onPlayerTypeChanged += value;
            }
            remove => m_onPlayerTypeChanged -= value;
        }


        [SerializeField]
        private List<PropertiesPerType> m_propertiesPerTypeList = new List<PropertiesPerType>();

        private event System.Action<EPlayerType, EPlayerType> m_onPlayerTypeChanged;
        private Dictionary<EPlayerType, PlayerProperties> m_propertiesPerType = null;
        private EPlayerType m_currentType = EPlayerType.NORMAL;

        private void Awake()
        {
            m_propertiesPerType = new Dictionary<EPlayerType, PlayerProperties>();
            foreach (PropertiesPerType kv in m_propertiesPerTypeList)
            {
                m_propertiesPerType[kv.PlayerType] = kv.Properties;
            }
        }

        private void Start()
        {
            m_currentType = GameManager.Instance.CurrentMaskUsage.InitialType;
            m_onPlayerTypeChanged?.Invoke(m_currentType, m_currentType);
        }
    }
}
