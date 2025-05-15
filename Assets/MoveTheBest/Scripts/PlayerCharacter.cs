using ECM2;
using ECM2.Examples.ThirdPerson;
using MoveTheBest.Abilities.DashingAbility;
using MoveTheBest.Abilities.SlideAbility;
using UnityEngine;

namespace MoveTheBest
{
    
    public class PlayerCharacter : ThirdPersonCharacter
    {
        #region ENUMS

        public enum ECustomMovementMode
        {
            None,
            Dashing,
            Sliding,
        }

        #endregion

        private DashAbility dashAbility;
        private SlideAbility slideAbility;
        
        #region METHODS

        protected override void Awake()
        {
            dashAbility = GetComponent<DashAbility>();
            slideAbility = GetComponent<SlideAbility>();
            base.Awake();
        }


        protected override bool DoJump()
        {
            if(!IsSliding())
            {
                var doJump = base.DoJump();
                return doJump;
            }
            else
            {
                //TODO combo
                return false;
            }
        }

        /// <summary>
        /// Is the character currently dashing?
        /// </summary>
        
        public bool IsDashing() => 
            movementMode == MovementMode.Custom && customMovementMode == (int)ECustomMovementMode.Dashing;

        public bool IsSliding() =>
            movementMode == MovementMode.Custom && customMovementMode == (int)ECustomMovementMode.Sliding;


        protected override void OnBeforeSimulationUpdate(float deltaTime)
        {
            // Call base method implementation
            
            base.OnBeforeSimulationUpdate(deltaTime);
            
            // Attempts to start a requested dash

            if (!IsDashing() && dashAbility.WantsToDash())
                dashAbility.DoDash();
            
            if(!IsSliding() && slideAbility.IsSlideAllowed())
                slideAbility.DoSlide();
        }

        protected override void CustomMovementMode(float deltaTime)
        {
            // Call base method implementation
            
            base.CustomMovementMode(deltaTime);
            
            // Update dashing movement mode

            if (customMovementMode == (int)ECustomMovementMode.Dashing)
                dashAbility.DashingMovementMode(deltaTime);
            if(customMovementMode == (int)ECustomMovementMode.Sliding)
                slideAbility.SlidingMovementMode(deltaTime);
        }

        #endregion
    }
}