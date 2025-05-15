using System;
using ECM2;
using ECM2.Walkthrough.Ex92;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoveTheBest.Abilities.DashingAbility
{
    public class DashAbility : MonoBehaviour
    {
        [Tooltip("Is the character able to Dash?")]
        [SerializeField] private bool canEverDash = true;
        
        [Tooltip("Dash initial impulse.")]
        [SerializeField] private float dashImpulse = 20.0f;
        
        [Tooltip("Dash duration in seconds.")]
        [SerializeField] 
        private float dashDuration = 0.15f;

        [SerializeField] 
        private InputActionReference  _actionReference;
        [SerializeField]
        private int buffer = 5;
        
        private PlayerCharacter player;
        
        protected int lastDashInputTime;
        protected float _dashingTime;

        private void Awake()
        {
            player = GetComponent<PlayerCharacter>();
        }

        private void OnEnable()
        {
            _actionReference.action.performed += OnInputPerformed;
        }

        private void OnDisable()
        {
            _actionReference.action.performed -= OnInputPerformed;
        }

        private void LateUpdate()
        {
            if (lastDashInputTime > 0)
                lastDashInputTime--;
        }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            lastDashInputTime = buffer;
        }
        

        public bool WantsToDash() => canEverDash && lastDashInputTime > 0 && IsDashAllowed();
        /// <summary>
        /// Determines if the Character is able to dash in its current state.
        /// Defaults to Walking or Falling while NOT crouched.
        /// </summary>

        public bool IsDashAllowed()
        {
            if (player.IsCrouched())
                return false;
            
            return canEverDash && (player.IsWalking() || player.IsFalling());
        }

        public virtual void DoDash()
        {
            // Apply dash impulse along input direction (if any) or along character's forward

            Vector3 dashDirection = player.GetMovementDirection();
            if (dashDirection.isZero())
                dashDirection = player.GetForwardVector();

            Vector3 dashDirection2D = dashDirection.onlyXZ().normalized;

            player.SetVelocity(dashDirection2D * dashImpulse);
            
            // Change to dashing movement mode
            
            player.SetMovementMode(Character.MovementMode.Custom, (int)PlayerCharacter.ECustomMovementMode.Dashing);
            
            // Lock rotation towards dashing direction
            if (player.rotationMode == Character.RotationMode.OrientRotationToMovement)
                player.SetRotation(Quaternion.LookRotation(dashDirection2D));
        }
        
        /// <summary>
        /// Reset dashing state and exit dashing movement mode.
        /// </summary>

        protected virtual void ResetDashState()
        {
            // Reset dashing state
            
            _dashingTime = 0.0f;
            lastDashInputTime = 0;
            
            // Clear dashing impulse
            
            player.SetVelocity(Vector3.zero);
            
            // Falling is auto-manged state so its safe to use as an exit state.
            player.SetMovementMode(Character.MovementMode.Falling);
        }

        public virtual void DashingMovementMode(float deltaTime)
        {
            // This prevents the character from rotate towards a movement direction
            
            player.SetMovementDirection(Vector3.zero);
            
            // Update dash timer...
                
            _dashingTime += deltaTime;
            if (_dashingTime >= dashDuration)
            {
                // If completed, exit dash state
                    
                ResetDashState();
            }
        }
    }
}
