using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class player_controller : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CinemachineCamera playerOrbitCamera;
    
    [Header("Ground stats")]
    public float walkSpeed = 5f; // vitesse MARCHE
    public float runSpeed = 15f;// vitesse COURSE
    public float dashSpeed = 40f; // vitesse DASH
    public float dashDuration = 0.2f;// durée DASH
    public float dashCooldown = 1.5f; // TEMPS avant de POUVOIR DASH
    public float accelerationTime = 1.25f; // TEMPS acceleration COURSE
    
    
    
    [Header("Air stats")]
    [SerializeField, Tooltip("puissance SAUT")] private float jumpPower = 10f; 
    [SerializeField, Tooltip("Gravité")] private float gravity = 15f;
    [SerializeField, Tooltip("nombre MAX de Saut")] private int maxJumpCount = 2;
   
    [Header("Collider")]
    public float defaultHeight = 2f;

    [Header("Drift")]
    [SerializeField, Min(0)] private float driftDuration;
    [SerializeField, Range(1, 720)] private float driftTurnSpeed;
    [SerializeField, Range(0, 10)] private float driftSideSpeed;
    [SerializeField, Range(0, 20)] private float driftBonusSpeed;
    [Header("Crouch")]
    public float crouchHeight = 1.2f;
    public float crouchSpeed = 3f; // Vitesse ACCROUPI
    public float climbSpeed = 5f; // vitesse ESCALADE liane grimpante
   
    public float runFOV = 80f; 
    public float fovTransitionSpeed = 3f; // temps transition de la FOV
    
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

    public int driftDirection; // 0 = aucun
    private bool isSlideSpeedBoosted = false;
    private int lastDirection;


    private float currentBonusSpeed;
    private float driftingTime;
    private float currentBonusSpeedTime;
    
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
        float inputXForSlide = Input.GetAxis("Horizontal");                        // SLIDE part 1
        
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.forward, transform.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(playerCamera.transform.right, transform.up).normalized;
        
        
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Q) && !isDashSpeedBoosted)
            StartCoroutine(TemporaryDashBoost());
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
                                                // DERAPAGE part 1
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        int nextTurnDirection = 0;

        if (!leftClick && !rightClick && driftingTime >= driftDuration)
            SetBonusSpeed(2, driftBonusSpeed);
        
        if (isRunning && characterController.isGrounded)
        {
            switch (driftDirection)
            {
                case 1 when !rightClick:
                case -1 when !leftClick:
                    nextTurnDirection = 0;
                    driftingTime = 0;
                    break;
                case 0 when rightClick:
                    nextTurnDirection = 1;
                    driftingTime = 0;
                    break;
                case 0 when leftClick:
                    nextTurnDirection = -1;
                    driftingTime = 0;
                    break;
                default:
                    nextTurnDirection = driftDirection;
                    break;
            }
        }
        
        driftDirection = nextTurnDirection;
        
        if (driftDirection != 0)
        {
            float duration = driftDuration;

            driftingTime += Time.deltaTime;

            if (driftingTime < duration)
            {
                transform.rotation *= Quaternion.Euler(0, driftTurnSpeed * Time.deltaTime * driftDirection, 0);
                characterController.Move(transform.right * (driftDirection * driftSideSpeed * Time.deltaTime));
            }
        }
        else
        {
            driftingTime = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && !isSimulatingCombo && canMove && !isSliding)
        {
            if (Mathf.Abs(inputXForSlide) < 0.01f && Mathf.Abs(inputZForSlide) < 0.01f)
            {
                return;
            }

            if (!characterController.isGrounded && currentJumpCount > 0)
                slideAfterJumpDetected = true;
            else if (characterController.isGrounded && !isRunSpeedBoosted)
                StartCoroutine(TemporaryRunSpeedBoost());

            StartCoroutine(CrouchThenSlideCombo());
        }
                                                // DASH
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
                                                // COMPORTEMENT perso lors du SLIDE
        if (isSliding)
        {
            Vector3 forwardDir = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 rightDir = Vector3.Scale(playerCamera.transform.right, new Vector3(1, 0, 1)).normalized;

            float inputX = Input.GetAxis("Horizontal");
            Vector3 slideDirection = forwardDir + rightDir * inputX;
            slideDirection = slideDirection.normalized;

            Vector3 horizontalVelocity = slideDirection * dashSpeed;
            float verticalVelocity = moveDirection.y;
            if (!characterController.isGrounded)
                verticalVelocity -= gravity * Time.deltaTime;

            moveDirection = horizontalVelocity + Vector3.up * verticalVelocity;

            CollisionFlags flags = characterController.Move(moveDirection * Time.deltaTime);
            if ((flags & CollisionFlags.Below) != 0 || (flags & CollisionFlags.Sides) != 0)
                isSliding = false;

            return;
        }
                                                // COMPORTEMENT perso lors du DASH
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;

            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }
                                                // ACCROUPISSEMENT
        bool isCrouching = (Input.GetKey(KeyCode.LeftControl) || isCrouchingSimulated) && !isSliding;
        float targetSpeed = walkSpeed;
        
        if (isCrouching)
        {
            characterController.height = crouchHeight;
            targetSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }

        if (currentBonusSpeedTime > 0)
        {
            currentBonusSpeedTime -= Time.deltaTime;
            targetSpeed += currentBonusSpeed;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * (targetSpeed / accelerationTime)); // vitesse augmentation graduelle

        float inputX2 = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        float moveY = moveDirection.y;
        moveDirection = (forward * inputZ + right * inputX2).normalized * currentSpeed;
                                                // ESCALADE ou pas ESCALADE tel est la question
        if (isClimbing)
        {
            float climbInput = Input.GetAxis("Vertical");
            moveDirection.y = climbInput * climbSpeed;
            currentJumpCount = 0;
            hasJumpBoostedSinceLastGrounded = false;
        }
                                                // SAUT
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
                moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        float targetFOV = Input.GetKey(KeyCode.LeftShift) && !isCrouching && isMoving ? runFOV : normalFOV;
        playerOrbitCamera.Lens.FieldOfView = Mathf.Lerp(playerOrbitCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        
        /*
        // CAMERA movement
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            float mouseY = Input.GetAxis("Mouse X") * lookSpeed;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + mouseY, 0);
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 localPos = playerCamera.transform.localPosition;
            localPos.z += scroll * zoomSpeed;
            localPos.z = Mathf.Clamp(localPos.z, -maxCameraDistance, -minCameraDistance);
            playerCamera.transform.localPosition = localPos;
        }

        HandleCameraCollision();
        */
    }

    private void SetBonusSpeed(float time, float bonusSpeed)
    {
        currentBonusSpeedTime = time;
        currentBonusSpeed = bonusSpeed;
    }

    /*
    // CAMERA collision (toc)
    private void HandleCameraCollision()
    {
        Vector3 cameraOrigin = transform.position + Vector3.up * defaultHeight;
        Vector3 desiredCameraWorldPos = playerCamera.transform.parent.TransformPoint(playerCamera.transform.localPosition);
        Vector3 direction = desiredCameraWorldPos - cameraOrigin;
        float desiredDistance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(cameraOrigin, direction, out hit, desiredDistance))
        {
            float collisionOffset = 0.1f;
            float correctedDistance = hit.distance - collisionOffset;
            correctedDistance = Mathf.Clamp(correctedDistance, minCameraDistance, maxCameraDistance);

            Vector3 newLocalPos = playerCamera.transform.localPosition;
            newLocalPos.z = -correctedDistance;
            playerCamera.transform.localPosition = newLocalPos;
        }
        else
        {
            Vector3 newLocalPos = playerCamera.transform.localPosition;
            newLocalPos.z = Mathf.Clamp(newLocalPos.z, -maxCameraDistance, -minCameraDistance);
            playerCamera.transform.localPosition = newLocalPos;
        }
    }
    */
    // SLIDE part 2
    private IEnumerator CrouchThenSlideCombo()
    {
        isSimulatingCombo = true;
        isCrouchingSimulated = true;

        yield return new WaitForSeconds(0.2f);

        if (!isSliding && dashCooldownTimer <= 0f)
        {
            isSliding = true;
            dashCooldownTimer = dashCooldown;

            Vector3 dashDirection = playerCamera.transform.forward;
            dashDirection = Quaternion.AngleAxis(15f, playerCamera.transform.right) * dashDirection;
            dashDirection.Normalize();

            moveDirection = dashDirection * dashSpeed;
        }

        yield return new WaitForSeconds(0.7f);

        if (!Input.GetKey(KeyCode.LeftControl))
            isCrouchingSimulated = false;

        isSimulatingCombo = false;
    }
                                                // BOOST DASH (vooosh)
    private IEnumerator TemporaryDashBoost()
    {
        isDashSpeedBoosted = true;
        dashSpeed += 25f;
        yield return new WaitForSeconds(1f);
        dashSpeed -= 25f;
        isDashSpeedBoosted = false;
    }
                                                // BOOST JUMP (boingggg)
    private IEnumerator TemporaryJumpBoost()
    {
        isJumpPowerBoosted = true;
        jumpPower += 5f;
        yield return new WaitForSeconds(0.1f);
        jumpPower -= 5f;
        isJumpPowerBoosted = false;
    }
                                                // BOOST DASH combo 
    private IEnumerator TemporaryDashComboBoost()
    {
        isDashSpeedBoosted = true;
        dashSpeed += 25f;
        yield return new WaitForSeconds(0.5f);
        dashSpeed -= 25f;
        isDashSpeedBoosted = false;
    }
                                                // BOOST SPEED 
    private IEnumerator TemporaryRunSpeedBoost()
    {
        isRunSpeedBoosted = true;
        runSpeed += 10f;
        yield return new WaitForSeconds(3f);
        runSpeed -= 10f;
        isRunSpeedBoosted = false;
    }
                                                // BOOST SLIDE
    private IEnumerator TemporarySlideSpeedBoost()
    {
        if (isSlideSpeedBoosted)
            yield break;

        isSlideSpeedBoosted = true;
        runSpeed += 3.5f;
        yield return new WaitForSeconds(7f);
        runSpeed -= 3.5f;
        isSlideSpeedBoosted = false;
    }
                             
                                                // LIERRE GRIMPANT 
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
