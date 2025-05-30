using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(CharacterController))]
public class player_controller : MonoBehaviour
{
    public CharacterController Controller => characterController;
    
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineCamera playerOrbitCamera;

    [Header("Ground stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 12f;
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    public float accelerationTime = 1.25f;

    [Header("Air stats")]
    [SerializeField, Tooltip("puissance SAUT")] private float jumpPower = 7.5f;
    [SerializeField, Tooltip("Gravité")] private float gravity = 15f;
    [SerializeField, Tooltip("nombre MAX de Saut")] private int maxJumpCount = 2;

    [Header("Collider")]
    public float defaultHeight = 2f;

    [Header("Drift")]
    [SerializeField, Min(0)] private float driftDuration;
    [SerializeField, Range(1, 720)] private float driftTurnSpeed;
    [SerializeField, Range(0, 10)] private float driftSideSpeed;
    [SerializeField, Range(0, 20)] private float driftBonusSpeed;
    [SerializeField, Range(0f, 1f)] private float driftBoostMultiplier = 0.65f;

    [Header("Crouch")]
    public float crouchHeight = 1.2f;
    public float crouchSpeed = 3f;
    public float climbSpeed = 5f;

    public float runFOV = 45f;
    public float fovTransitionSpeed = 3f;

    private float dashCooldownTimer = 0f;
    private float currentSpeed = 0f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private int currentJumpCount = 0;
    private bool isClimbing = false;
    private bool isSimulatingCombo = false;
    private bool isCrouchingSimulated = false;
    private bool isDashSpeedBoosted = false;
    private bool isJumpPowerBoosted = false;
    private bool hasJumpBoostedSinceLastGrounded = false;
    private Vector3 cameraDefaultLocalPosition;
    private bool isSliding = false;
    private float normalFOV;
    private bool slideAfterJumpDetected = false;
    private bool isRunSpeedBoosted = false;
    private bool hasDoubleJumpDashBoost = false;

    public int driftDirection;
    private bool isSlideSpeedBoosted = false;
    private int lastDirection;

    private float currentBonusSpeed;
    private float driftingTime;
    private float currentBonusSpeedTime;

    private bool hasGivenDriftBonus = false;
    
    [Header("Slide"), Range(0, 1)]
    [SerializeField] private float slideSpeedMultiplier;

    private float targetSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraDefaultLocalPosition = playerCamera.transform.localPosition;
        normalFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        float inputZForSlide = Input.GetAxis("Vertical");
        float inputXForSlide = Input.GetAxis("Horizontal");

        if (!isSliding)
        {
            characterController.height = defaultHeight;
            characterController.center = new Vector3(0f, 0, 0f);
        }
        
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.forward, transform.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(playerCamera.transform.right, transform.up).normalized;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Q) && !isDashSpeedBoosted)
            StartCoroutine(TemporaryDashBoost());

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        #region Derapage
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        int nextTurnDirection = 0;

        if (isRunning && characterController.isGrounded)
        {
            switch (driftDirection)
            {
                case 1 when !rightClick:
                case -1 when !leftClick:
                    nextTurnDirection = 0;
                    driftingTime = 0;
                    hasGivenDriftBonus = false;
                    break;

                case 0 when rightClick:
                    nextTurnDirection = 1;
                    driftingTime = 0;
                    if (!hasGivenDriftBonus)
                    {
                        SetBonusSpeed(2, driftBonusSpeed * driftBoostMultiplier);
                        hasGivenDriftBonus = true;
                    }
                    break;

                case 0 when leftClick:
                    nextTurnDirection = -1;
                    driftingTime = 0;
                    if (!hasGivenDriftBonus)
                    {
                        SetBonusSpeed(2, driftBonusSpeed * driftBoostMultiplier);
                        hasGivenDriftBonus = true;
                    }
                    break;

                default:
                    nextTurnDirection = driftDirection;
                    break;
            }
        }

        driftDirection = nextTurnDirection;

        if (driftDirection != 0)
        {
            driftingTime += Time.deltaTime;
            if (driftingTime < driftDuration)
            {
                transform.rotation *= Quaternion.Euler(0, driftTurnSpeed * Time.deltaTime * driftDirection, 0);
                characterController.Move(transform.right * (driftDirection * driftSideSpeed * Time.deltaTime));
            }
        }
        else
        {
            driftingTime = 0;
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Q) && canMove && !isSliding)
        {
            if (Mathf.Abs(inputXForSlide) < 0.01f && Mathf.Abs(inputZForSlide) < 0.01f)
                return;

            if (!characterController.isGrounded && currentJumpCount > 0)
                slideAfterJumpDetected = true;
            else if (characterController.isGrounded && !isRunSpeedBoosted)
                StartCoroutine(TemporaryRunSpeedBoost());

            StartCoroutine(CrouchThenSlideCombo());
        }

        #region Dash
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && canMove && dashCooldownTimer <= 0f)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            Vector3 dashDir = playerCamera.transform.forward;
            dashDir.y = 0f;
            dashDir.Normalize();

            if (Input.GetKey(KeyCode.LeftShift) && !isDashSpeedBoosted)
                StartCoroutine(TemporaryDashComboBoost());

            moveDirection = dashDir * dashSpeed;
            moveDirection.y = 0;

            if (!characterController.isGrounded && currentJumpCount == 1)
                hasDoubleJumpDashBoost = true;
        }
        #endregion

        #region Slide
        if (isSliding)
        {
            // Taille plus petite pour le slide
            characterController.height = 0.5f;
            characterController.center = new Vector3(0, -0.55f, 0);

            // Direction de slide calculée à partir de la caméra
            Vector3 forwardDir = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 rightDir = Vector3.Scale(playerCamera.transform.right, new Vector3(1, 0, 1)).normalized;

            float inputX = Input.GetAxis("Horizontal");
            Vector3 slideDirection = (forwardDir + rightDir * inputX).normalized;

            // Calcul du mouvement horizontal
            float currentSlideSpeed = dashSpeed * slideSpeedMultiplier * targetSpeed;
    
            // Clamp pour éviter les valeurs trop hautes (anti-bug)
            currentSlideSpeed = Mathf.Min(currentSlideSpeed, 20f); // ← tu peux ajuster ce max

            Vector3 horizontalVelocity = slideDirection * currentSlideSpeed;

            // Gestion de la gravité
            float verticalVelocity = moveDirection.y;
            if (!characterController.isGrounded)
                verticalVelocity -= gravity * Time.deltaTime;
            else
                verticalVelocity = -1f; // évite de rebondir en restant grounded

            // Applique le mouvement final
            moveDirection = horizontalVelocity + Vector3.up * verticalVelocity;
            characterController.Move(moveDirection * Time.deltaTime);

            // Optionnel : sortir du slide si plus de vitesse ou collision
            if (horizontalVelocity.magnitude < 1f || (characterController.collisionFlags & CollisionFlags.Sides) != 0)
                isSliding = false;

            return;
        }
        #endregion

        #region Dash
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;

            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }
        #endregion

        #region Crouch
        bool isCrouching = (Input.GetKey(KeyCode.LeftControl) || isCrouchingSimulated) && !isSliding;
        targetSpeed = walkSpeed;

        if (isCrouching)
        {
            //targetSpeed = crouchSpeed;
            //transform.localScale = new Vector3(1f, 0.55f, 1f);
        }
        else
        {
            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        #endregion

        if (currentBonusSpeedTime > 0)
        {
            currentBonusSpeedTime -= Time.deltaTime;
            targetSpeed += currentBonusSpeed;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * (targetSpeed / accelerationTime));

        float inputX2 = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        float moveY = moveDirection.y;
        moveDirection = (forward * inputZ + right * inputX2).normalized * currentSpeed;

        #region Climb
        if (isClimbing)
        {
            float climbInput = Input.GetAxis("Vertical");
            moveDirection.y = climbInput * climbSpeed;
            currentJumpCount = 0;
            hasJumpBoostedSinceLastGrounded = false;
        }
        #endregion

        #region Jump
        else
        {
            if (characterController.isGrounded)
            {
                if (slideAfterJumpDetected && !isRunSpeedBoosted)
                    StartCoroutine(TemporaryRunSpeedBoost());

                slideAfterJumpDetected = false;
                currentJumpCount = 0;
                hasJumpBoostedSinceLastGrounded = false;
                hasDoubleJumpDashBoost = false;
            }

            if (Input.GetButtonDown("Jump") && canMove && currentJumpCount < maxJumpCount)
            {
                if (Input.GetKey(KeyCode.LeftShift) && !isJumpPowerBoosted && !hasJumpBoostedSinceLastGrounded)
                {
                    StartCoroutine(TemporaryJumpBoost());
                    hasJumpBoostedSinceLastGrounded = true;
                }

                moveDirection.y = hasDoubleJumpDashBoost && currentJumpCount == 1 ? jumpPower + 5f : jumpPower;
                currentJumpCount++;
            }
            else
            {
                moveDirection.y = moveY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
               
        }
        #endregion

        characterController.Move(moveDirection * Time.deltaTime);

        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        float targetFOV = Input.GetKey(KeyCode.LeftShift) && !isCrouching && isMoving ? runFOV : normalFOV;
        playerOrbitCamera.Lens.FieldOfView = Mathf.Lerp(playerOrbitCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
    }

    private void SetBonusSpeed(float time, float bonusSpeed)
    {
        currentBonusSpeedTime = time;
        currentBonusSpeed = bonusSpeed;
    }

    private IEnumerator CrouchThenSlideCombo()
    {
        isSimulatingCombo = true;
        isCrouchingSimulated = true;
        
        yield return new WaitForSeconds(0.2f);

        if (!isSliding && dashCooldownTimer <= 0f)
        {
            isSliding = true;
            dashCooldownTimer = dashCooldown;

            Vector3 dashDirection;
            
            if (!characterController.isGrounded)
            {
                Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
                dashDirection = forward;

                // Ajoute une légère inclinaison vers le sol pour forcer l’atterrissage, mais sans excès
                dashDirection = Quaternion.AngleAxis(15f, playerCamera.transform.right) * dashDirection;
            }

            else
            {
                dashDirection = playerCamera.transform.forward;
                dashDirection = Quaternion.AngleAxis(15f, playerCamera.transform.right) * dashDirection;
                dashDirection.Normalize();
            }

            moveDirection = dashDirection * dashSpeed;

            if (isSliding)
            {
                yield return new WaitForSeconds(1.5f);
                isSliding = false;
            }
        }

        yield return new WaitForSeconds(0.7f);

        if (!Input.GetKey(KeyCode.LeftControl))
            isCrouchingSimulated = false;

        // Remettre la taille normale après le slide
        transform.localScale = new Vector3(1f, 1f, 1f);

        isSimulatingCombo = false;
    }

    private IEnumerator TemporaryDashBoost()
    {
        isDashSpeedBoosted = true;
        dashSpeed += 7f;
        yield return new WaitForSeconds(1f);
        dashSpeed -= 7f;
        isDashSpeedBoosted = false;
    }

    private IEnumerator TemporaryJumpBoost()
    {
        isJumpPowerBoosted = true;
        jumpPower += 5f;
        yield return new WaitForSeconds(0.1f);
        jumpPower -= 5f;
        isJumpPowerBoosted = false;
    }

    private IEnumerator TemporaryDashComboBoost()
    {
        isDashSpeedBoosted = true;
        dashSpeed += 15f;
        yield return new WaitForSeconds(0.5f);
        dashSpeed -= 15f;
        isDashSpeedBoosted = false;
    }

    private IEnumerator TemporaryRunSpeedBoost()
    {
        isRunSpeedBoosted = true;
        runSpeed += 5.5f;
        yield return new WaitForSeconds(5f);
        runSpeed -= 5.5f;
        isRunSpeedBoosted = false;
    }

    private IEnumerator TemporarySlideSpeedBoost()
    {
        if (isSlideSpeedBoosted)
            yield break;

        isSlideSpeedBoosted = true;
        runSpeed += 1f;
        yield return new WaitForSeconds(3.5f);
        runSpeed -= 1f;
        isSlideSpeedBoosted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("liere_grimpant"))
        {
            isClimbing = true;
            moveDirection = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("liere_grimpant"))
        {
            isClimbing = false;
        }
    }
}
