using ECM2;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoveTheBest.Abilities.SlideAbility
{
    public class SlideAbility : MonoBehaviour
    {
        [Tooltip("Is the character able to Slide?")]
        public bool canEverSlide = true;
        
        [Tooltip("Slide initial impulse.")]
        public float slideImpulse = 20.0f;
        
        [Tooltip("Slide duration in seconds.")]
        public float slideDuration = 0.15f;
        
        [SerializeField] 
        private InputActionReference  _actionReference;
        [SerializeField]
        private int buffer = 5;
        
        private PlayerCharacter player;
        
        protected float _dashingTime;
        
        protected float _slidingTime;
        protected int lastSlideInputTime;
        

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
            if (lastSlideInputTime > 0)
                lastSlideInputTime--;
        }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            lastSlideInputTime = buffer;
        }

        public bool WantsToSlide() => canEverSlide && lastSlideInputTime > 0 && IsSlideAllowed();
        /// <summary>
        /// Determines if the Character is able to slide in its current state.
        /// Defaults to Walking or Falling while NOT crouched.
        /// </summary>

        public bool IsSlideAllowed()
        {
            if (player.IsCrouched())
                return false;
            
            return canEverSlide && (player.IsWalking() || player.IsFalling());
        }

        public virtual void DoSlide()
        {
            // Apply slide impulse along input direction (if any) or along character's forward

            Vector3 slideDirection = player.GetMovementDirection();
            if (slideDirection.isZero())
                slideDirection = player.GetForwardVector();

            Vector3 slideDirection2D = slideDirection.onlyXZ().normalized;

            player.SetVelocity(slideDirection2D * slideImpulse);
            
            // Change to sliding movement mode
            
            player.SetMovementMode(Character.MovementMode.Custom, (int)PlayerCharacter.ECustomMovementMode.Sliding);
            
            // Lock rotation towards sliding direction
            if (player.rotationMode == Character.RotationMode.OrientRotationToMovement)
                player.SetRotation(Quaternion.LookRotation(slideDirection2D));
        }
        
        /// <summary>
        /// Reset sliding state and exit sliding movement mode.
        /// </summary>

        protected virtual void ResetSlideState()
        {
            // Reset sliding state
            
            _slidingTime = 0.0f;
            lastSlideInputTime = 0;
            
            // Clear sliding impulse
            
            player.SetVelocity(Vector3.zero);
            
            // Falling is auto-manged state so its safe to use as an exit state.
            player.SetMovementMode(Character.MovementMode.Falling);
        }

        public virtual void SlidingMovementMode(float deltaTime)
        {
            // This prevents the character from rotate towards a movement direction
            
            player.SetMovementDirection(Vector3.zero);
            
            // Update slide timer...
                
            _slidingTime += deltaTime;
            if (_slidingTime >= slideDuration)
            {
                // If completed, exit slide state
                    
                ResetSlideState();
            }
        }
    }
}