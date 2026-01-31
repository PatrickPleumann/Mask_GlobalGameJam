using Effects;
using GameLoop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        public event System.Action<float> OnJumpChargeChanged
        {
            add
            {
                m_onJumpChargeChanged -= value;
                m_onJumpChargeChanged += value;
            }
            remove
            {
                m_onJumpChargeChanged -= value;
            }
        }

        [SerializeField]
        private LayerMask m_groundMask;
        [SerializeField]
        private List<EPlayerType> m_typesWithChargeableJump = new List<EPlayerType>();
        [SerializeField]
        private float m_maxJumpChargeTime = 1.0f;
        [SerializeField]
        private List<EPlayerType> m_typesWithShake = new List<EPlayerType>();

        private event System.Action<float> m_onJumpChargeChanged;

        private Rigidbody2D m_rigidbody = null;
        private BoxCollider2D m_collider = null;
        private Vector2 m_movementInput = Vector2.zero;
        private bool m_wasJumpPressed = false;
        private PlayerState m_playerState = null;
        private float m_currentJumpCharge = 0.0f;
        private CameraShake m_cameraShake = null;

        private void Awake()
        {
            m_playerState = GetComponent<PlayerState>();
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_collider = GetComponent<BoxCollider2D>();
            SubscribeToInput();

            m_playerState.OnPlayerTypeChanged += UpdateRigibody;
            m_playerState.OnPlayerTypeChanged += UpdateColliderShape;

            m_cameraShake = FindFirstObjectByType<CameraShake>();
        }

        private void Start()
        {
            GameManager.Instance.OnPlayerDied += DisableSelf;
        }

        private void DisableSelf()
        {
            GameManager.Instance.OnPlayerDied -= DisableSelf;
            UnsubscribeFromInput();
            m_movementInput = Vector2.zero;
            m_rigidbody.linearVelocity = Vector2.zero;
        }

        private void OnDestroy()
        {
            m_playerState.OnPlayerTypeChanged -= UpdateRigibody;
            m_playerState.OnPlayerTypeChanged -= UpdateColliderShape;

            UnsubscribeFromInput();
        }

        private void SubscribeToInput()
        {
            InputSystem.actions.FindAction("Move").started += OnPlayerMoved;
            InputSystem.actions.FindAction("Move").canceled += OnPlayerMoved;
            InputSystem.actions.FindAction("Jump").performed += OnPlayerJump;
            InputSystem.actions.FindAction("Jump").canceled += OnPlayerJumpCanceled;

            InputSystem.actions.FindAction("UseHeavyMask").performed += ChangeToHeavyMask;
            InputSystem.actions.FindAction("UseNormalMask").performed += ChangeToNormalMask;
            InputSystem.actions.FindAction("UseLiquidMask").performed += ChangeToLiquidMask;
            InputSystem.actions.FindAction("UseWideMask").performed += ChangeToWideMask;
            InputSystem.actions.FindAction("UseNarrowMask").performed += ChangeToNarrowMask;
        }

        private void UnsubscribeFromInput()
        {
            InputSystem.actions.FindAction("Move").started -= OnPlayerMoved;
            InputSystem.actions.FindAction("Move").canceled -= OnPlayerMoved;
            InputSystem.actions.FindAction("Jump").performed -= OnPlayerJump;

            InputSystem.actions.FindAction("UseHeavyMask").performed -= ChangeToHeavyMask;
            InputSystem.actions.FindAction("UseNormalMask").performed -= ChangeToNormalMask;
            InputSystem.actions.FindAction("UseLiquidMask").performed -= ChangeToLiquidMask;
            InputSystem.actions.FindAction("UseWideMask").performed -= ChangeToWideMask;
            InputSystem.actions.FindAction("UseNarrowMask").performed -= ChangeToNarrowMask;
        }

        private void ChangeMask(EPlayerType _target)
        {
            if (GameManager.Instance.CanUseMask(_target))
            {
                m_playerState.CurrentType = _target;
            }
        }

        private void ChangeToHeavyMask(InputAction.CallbackContext _context)
        {
            ChangeMask(EPlayerType.HEAVY);
        }

        private void ChangeToWideMask(InputAction.CallbackContext _context)
        {
            ChangeMask(EPlayerType.WIDE);
        }

        private void ChangeToLiquidMask(InputAction.CallbackContext _context)
        {
            ChangeMask(EPlayerType.LIQUID);
        }

        private void ChangeToNormalMask(InputAction.CallbackContext _context)
        {
            ChangeMask(EPlayerType.NORMAL);
        }

        private void ChangeToNarrowMask(InputAction.CallbackContext _context)
        {
            ChangeMask(EPlayerType.NARROW);
        }


        private void OnPlayerJump(InputAction.CallbackContext _context)
        {
            m_wasJumpPressed = true;
            if (m_typesWithChargeableJump.Contains(m_playerState.CurrentType))
            {
                m_currentJumpCharge = 0.0f;
                m_onJumpChargeChanged?.Invoke(m_currentJumpCharge / m_maxJumpChargeTime);
            }
        }

        private void OnPlayerJumpCanceled(InputAction.CallbackContext _context)
        {
            m_wasJumpPressed = false;
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
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + Vector2.up * 0.1f,
                new Vector2(m_playerState.CurrentProperties.Width * 1.1f, m_playerState.CurrentProperties.Height),
                0, Vector2.down, 0.2f, m_groundMask);

            return hit.collider != null;
        }

        private void Jump(float _chargeTime)
        {
            m_rigidbody.AddForce(Vector2.up * m_playerState.CurrentProperties.JumpForce * (_chargeTime / m_maxJumpChargeTime), ForceMode2D.Impulse);
            
            if (m_typesWithShake.Contains(m_playerState.CurrentType))
            {
                m_cameraShake.Shake();
            }
        }

        private void Update()
        {
            bool isGrounded = IsGrounded();
            if (m_wasJumpPressed)
            {
                if (m_typesWithChargeableJump.Contains(m_playerState.CurrentType))
                {
                    m_currentJumpCharge = Mathf.Min(m_maxJumpChargeTime, m_currentJumpCharge + Time.deltaTime);
                    m_onJumpChargeChanged?.Invoke(m_currentJumpCharge / m_maxJumpChargeTime);
                }
                else
                {
                    if (isGrounded)
                    {
                        Jump(m_maxJumpChargeTime);
                    }
                    m_wasJumpPressed = false;
                }
            }
            else if (m_currentJumpCharge > 0.0f)
            {
                if (isGrounded)
                {
                    Jump(m_currentJumpCharge);
                }
                m_currentJumpCharge = -1.0f;
                m_onJumpChargeChanged?.Invoke(-1.0f);
            }
            Vector2 velocity = m_rigidbody.linearVelocity;
            velocity.x = m_movementInput.x * (isGrounded ? m_playerState.CurrentProperties.MovementSpeed : m_playerState.CurrentProperties.AirSpeed);
            m_rigidbody.linearVelocity = velocity;
        }
    }
}
