using UnityEngine;

namespace MoveTheBest
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        public float walkSpeed = 3.5f;
        public float runSpeed = 7f;
        public float gravity = -9.81f;
        public float jumpHeight = 1.5f;
        public int maxJumps = 2;

        private CharacterController controller;
        private Vector3 velocity;
        private int jumpCount;
        private bool isGrounded;
    
        public Vector3 Velocity => velocity;
        public bool IsGrounded() => isGrounded;
        public void SetVelocity(Vector3 newVelocity) => velocity = newVelocity;



        private PlayerInputHandler inputHandler;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
        }

        void Update()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                jumpCount = 0;
            }

            Vector2 moveInput = inputHandler.GetMoveInput();
            Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

            // Make movement relative to the camera
            if (Camera.main != null)
            {
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = Camera.main.transform.right;

                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                move = camForward * moveInput.y + camRight * moveInput.x;
            }

            float speed = inputHandler.IsSprinting() ? runSpeed : walkSpeed;
            controller.Move(move * speed * Time.deltaTime);

            // Jump
            if (inputHandler.IsJumping() && jumpCount < maxJumps)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCount++;
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}