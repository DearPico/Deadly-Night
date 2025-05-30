using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    private Transform capsuleTransform;

    private bool isJumping = false;
    private bool hasDoubleJumped = false;
    private Vector3 velocity;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private float airTime = 0f; // Timer d'air
    public float airThreshold = 0.08f; // Temps avant d’activer l'anim "falling"

    void Start()
    {
        animator = GetComponent<Animator>();
        capsuleTransform = transform.parent; // Capsule = parent de perso_urbain_rig
        controller = capsuleTransform.GetComponent<CharacterController>();
    }

    void Update()
    {
        animator.SetBool("IsGrounded", controller.isGrounded);
        HandleMovement();
        HandleFalling();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            animator.SetBool("Running", Input.GetKey(KeyCode.LeftShift));
            animator.SetBool("Walking", !Input.GetKey(KeyCode.LeftShift));
            /*if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);
            }
            else
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
            }*/
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("Dash");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("Slide");
        }
    }

    void HandleFalling()
    {
        // Si le joueur n'est pas au sol (en l'air, qu’il monte ou qu’il tombe)
        if (!controller.isGrounded)
        {
            airTime += Time.deltaTime;

            if (airTime >= airThreshold)
            {
                animator.SetBool("IsFalling", true);
            }
        }
        else
        {
            airTime = 0f;
            animator.SetBool("IsFalling", false);
        }

        // Atterrissage
        if (controller.isGrounded && isJumping)
        {
            animator.SetTrigger("Land");
        }
    }
}
