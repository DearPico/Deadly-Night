using ECM2;
using ECM2.Walkthrough.Ex92;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineThirdPersonCharacter))]
public class DashAbility : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.5f;

    [Header("Input")]
    public InputActionReference dashAction;

    private CinemachineThirdPersonCharacter character;
    private CharacterMovement movementScript;

    private bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private Vector3 dashDirection;

    private void Awake()
    {
        character = GetComponent<CinemachineThirdPersonCharacter>();
        movementScript = GetComponent<CharacterMovement>();
    }

    private void OnEnable()
    {
        if (dashAction != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
        {
            dashAction.action.performed -= OnDashPerformed;
            dashAction.action.Disable();
        }
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;

                // Libère le contrôle normal après dash
                if (movementScript != null)
                    movementScript.enabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // On récupère la vélocité actuelle
            Vector3 currentVelocity = character.velocity;

            // On conserve la gravité (Y)
            Vector3 dashVelocity = dashDirection * dashSpeed;
            dashVelocity.y = currentVelocity.y;

            character.SetVelocity(dashVelocity);
        }
    }



    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        TriggerDash();
    }

    public void TriggerDash()
    {
        if (isDashing || cooldownTimer > 0f)
            return;

        Debug.Log("DASH TRIGGERED");

        dashDirection = transform.forward;
        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        // Optionnel : désactive les autres mouvements pendant le dash
        if (movementScript != null)
            movementScript.enabled = false;
    }
}
