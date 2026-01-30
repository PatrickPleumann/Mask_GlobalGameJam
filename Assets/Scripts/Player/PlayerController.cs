using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_groundMask;
        private Rigidbody2D m_rigidbody = null;
        private BoxCollider2D m_collider = null;
        private Vector2 m_movementInput = Vector2.zero;
        private bool m_wasJumpPressed = false;
        private PlayerState m_playerState = null;

        private void Awake()
        {
            m_playerState = GetComponent<PlayerState>();
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_collider = GetComponent<BoxCollider2D>();
            SubscribeToInput();

            m_playerState.OnPlayerTypeChanged += UpdateRigibody;
            m_playerState.OnPlayerTypeChanged += UpdateColliderShape;
        }

        private void OnDestroy()
        {
            m_playerState.OnPlayerTypeChanged -= UpdateRigibody;
            m_playerState.OnPlayerTypeChanged -= UpdateColliderShape;
        }

        private void SubscribeToInput()
        {
            InputSystem.actions.FindAction("Move").started += OnPlayerMoved;
            InputSystem.actions.FindAction("Move").canceled += OnPlayerMoved;
            InputSystem.actions.FindAction("Jump").performed += OnPlayerJump;
        }

        private void OnPlayerJump(InputAction.CallbackContext _context)
        {
            m_wasJumpPressed = true;
        }

        private void OnPlayerMoved(InputAction.CallbackContext _context)
        {
            m_movementInput = _context.ReadValue<Vector2>();
        }

        private void UpdateColliderShape(EPlayerType _previous, EPlayerType _current)
        {
            PlayerProperties properties = m_playerState.CurrentProperties;
            m_collider.size = new Vector2(properties.Width, properties.Height);
            m_collider.offset = new Vector2(0f, properties.Height / 2f);
        }

        private void UpdateRigibody(EPlayerType _previous, EPlayerType _current)
        {
            PlayerProperties properties = m_playerState.CurrentProperties;
            m_rigidbody.mass = properties.Weight;
        }

        private bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.up * 0.05f, Vector2.down, 0.1f, m_groundMask);
            return hit.collider != null;
        }

        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                m_playerState.CurrentType = EPlayerType.NORMAL;
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                m_playerState.CurrentType = EPlayerType.HEAVY;
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                m_playerState.CurrentType = EPlayerType.LIQUID;
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                m_playerState.CurrentType = EPlayerType.WIDE;
            }
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                m_playerState.CurrentType = EPlayerType.NARROW;
            }

            bool isGrounded = IsGrounded();
            if (m_wasJumpPressed)
            {
                if (isGrounded)
                {
                    m_rigidbody.AddForce(Vector2.up * m_playerState.CurrentProperties.JumpForce, ForceMode2D.Impulse);
                }
                m_wasJumpPressed = false;
            }
            Vector2 velocity = m_rigidbody.linearVelocity;
            velocity.x = m_movementInput.x * (isGrounded ? m_playerState.CurrentProperties.MovementSpeed : m_playerState.CurrentProperties.AirSpeed);
            m_rigidbody.linearVelocity = velocity;
        }


        private void OnGUI()
        {
            GUILayout.Label($"Current Player Type: {m_playerState.CurrentType}");
            GUILayout.Label($"Current MovementSpeed: {m_movementInput}");
        }
    }
}
