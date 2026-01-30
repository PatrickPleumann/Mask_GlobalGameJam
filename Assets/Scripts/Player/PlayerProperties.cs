using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerProperties", menuName = "Player/PlayerProperties", order = 1)]
    public class PlayerProperties : ScriptableObject
    {
        public float MovementSpeed => m_movementSpeed;
        public float JumpForce => m_jumpForce;
        public float Weight => m_weight;
        public float Height => m_height;
        public float Width => m_width;
        public float AirSpeed => m_airSpeed;

        [SerializeField]
        private float m_movementSpeed = 1.0f;
        [SerializeField]
        private float m_airSpeed = 0.0f;
        [SerializeField]
        private float m_jumpForce = 5.0f;
        [SerializeField]
        private float m_weight = 1.0f;
        [SerializeField]
        private float m_height = 2.0f;
        [SerializeField]
        private float m_width = 0.5f;
    }
}
