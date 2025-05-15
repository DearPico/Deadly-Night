using UnityEngine;

namespace MoveTheBest.Abilities.SlideAbility
{
    [RequireComponent(typeof(PlayerMovement))]
    public class SlideAbility : MonoBehaviour
    {
        [Header("Slide Settings")]
        public float slideSpeed = 10f;
        public float slideDuration = 0.75f;

        private PlayerMovement _character;
        private bool _isSliding;
        private float _slideTimer;

        private void Awake()
        {
            _character = GetComponent<PlayerMovement>();
        }

        private void FixedUpdate()
        {
            if (!_isSliding)
                return;

            if (_slideTimer <= 0f || !_character.IsGrounded())
            {
                StopSlide();
            }
            else
            {
                Vector3 slideDirection = transform.forward * slideSpeed;
                _character.SetVelocity(new Vector3(slideDirection.x, _character.Velocity.y, slideDirection.z));
                _slideTimer -= Time.fixedDeltaTime;
            }
        }

        public void StartSlide()
        {
            if (_isSliding || !_character.IsGrounded())
                return;

            _slideTimer = slideDuration;
            _isSliding = true;
        }

        private void StopSlide()
        {
            _isSliding = false;
        }
    }
}