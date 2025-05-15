using UnityEngine;
using UnityEngine.InputSystem;

namespace MoveTheBest.Abilities.SlideAbility
{
    [RequireComponent(typeof(PlayerMovement))]
    public class SlideAbilityInput : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference crouchAction;

        private PlayerMovement _characterMovement;
        private SlideAbility _slideAbility;

        private void Awake()
        {
            _characterMovement = GetComponent<PlayerMovement>();
            _slideAbility = GetComponent<SlideAbility>();
        }

        private void OnEnable()
        {
            if (crouchAction != null)
            {
                crouchAction.action.performed += OnCrouchPerformed;
                crouchAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (crouchAction != null)
            {
                crouchAction.action.performed -= OnCrouchPerformed;
                crouchAction.action.Disable();
            }
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            if (_characterMovement.IsGrounded() &&
                _characterMovement.Velocity.magnitude > 5f &&
                _slideAbility != null)
            {
                _slideAbility.StartSlide();
            }
        }
    }
}