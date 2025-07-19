using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class P_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float movementSmoothing = 10f; // Smoothing for movement input
    private float verticalVelocity = 0f;
    private Vector3 smoothedMovementInput;

    [Header("Bob Settings")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.2f;
    public float runBobSpeed = 14f;
    public float runBobAmount = 0.3f;
    public float bobRotationAmount = 5f;
    public float bobSmoothSpeed = 10f; // Smoothing for head bob transitions
    private Vector3 camInitialLocalPos;
    private Quaternion camInitialLocalRot;
    private float bobTimer = 0f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float mouseSmoothing = 5f; // Mouse smoothing
    private float xRotation = 0f;
    private Vector2 mouseLookVelocity;
    private Vector2 currentMouseDelta;

    [Header("FOV Settings")]
    public Camera cam;
    public float normalFOV = 60f;
    public float runFOV = 75f;
    public float fovSmoothSpeed = 5f;

    [Header("Footstep Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    public float footstepVolumeWalk = 0.5f;
    public float footstepVolumeRun = 0.7f;
    public float minFootstepInterval = 0.3f; // Minimum time between footsteps

    [Header("Debug")]
    public bool showDebugInfo = true;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.1f;

    private CharacterController controller;
    private bool isMoving = false;
    private float lastFootstepTime = 0f;
    private bool wasMoving = false; // Track previous movement state

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cam == null)
            cam = Camera.main;

        if (cam != null)
            cam.fieldOfView = normalFOV;

        // Store camera's initial local position & rotation
        if (playerCamera != null)
        {
            camInitialLocalPos = playerCamera.localPosition;
            camInitialLocalRot = playerCamera.localRotation;

            if (showDebugInfo)
                Debug.Log($"Initial camera pos: {camInitialLocalPos}, rot: {camInitialLocalRot}");
        }
        else
        {
            Debug.LogError("Player Camera not assigned!");
        }

        // Setup audio source if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure audio source for footsteps
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleFOV();
        HandleHeadBob();

        if (showDebugInfo)
            DebugInfo();
    }

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Get raw input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 targetMovementInput = new Vector3(moveX, 0f, moveZ);

        // Smooth the movement input to reduce twitchiness
        smoothedMovementInput = Vector3.Lerp(smoothedMovementInput, targetMovementInput, movementSmoothing * Time.deltaTime);

        Vector3 move = transform.right * smoothedMovementInput.x + transform.forward * smoothedMovementInput.z;

        // Apply horizontal movement
        move *= currentSpeed;

        // Apply gravity with smoother grounding
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;  // Small downward force to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // Update movement state with hysteresis to prevent rapid state changes
        float movementThreshold = 0.1f;
        bool currentlyMoving = (smoothedMovementInput.magnitude > movementThreshold);

        // Use hysteresis: harder to start moving, easier to keep moving
        if (!isMoving && currentlyMoving)
        {
            isMoving = smoothedMovementInput.magnitude > movementThreshold;
        }
        else if (isMoving && !currentlyMoving)
        {
            isMoving = smoothedMovementInput.magnitude > movementThreshold * 0.5f;
        }

        wasMoving = isMoving;
    }

    void HandleMouseLook()
    {
        if (playerCamera == null) return;

        // Get raw mouse input
        Vector2 rawMouseDelta = new Vector2(
            Input.GetAxis("Mouse X") * mouseSensitivity,
            Input.GetAxis("Mouse Y") * mouseSensitivity
        );

        // Smooth mouse input to reduce jerkiness
        currentMouseDelta = Vector2.Lerp(currentMouseDelta, rawMouseDelta, mouseSmoothing * Time.deltaTime);

        xRotation -= currentMouseDelta.y * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply horizontal rotation to player body
        transform.Rotate(Vector3.up * currentMouseDelta.x * Time.deltaTime);
    }

    void HandleFOV()
    {
        if (cam == null) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetFOV = isRunning ? runFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);
    }

    void HandleHeadBob()
    {
        if (playerCamera == null) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving)
        {
            float currentBobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float currentBobAmount = isRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * currentBobSpeed;

            // Calculate bob offsets with smoother transitions
            float bobOffsetY = Mathf.Sin(bobTimer) * currentBobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * currentBobAmount * 0.5f;

            // Apply position bob with smoothing
            Vector3 targetPos = camInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetPos, bobSmoothSpeed * Time.deltaTime);

            // Apply rotation bob combined with mouse look
            float tiltZ = Mathf.Sin(bobTimer) * bobRotationAmount;
            Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, tiltZ);
            playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, targetRotation, bobSmoothSpeed * Time.deltaTime);

            // Handle footstep sounds with improved timing
            HandleFootsteps(isRunning);
        }
        else
        {
            // When not moving, smoothly return to original position
            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                camInitialLocalPos,
                bobSmoothSpeed * Time.deltaTime
            );

            playerCamera.localRotation = Quaternion.Lerp(
                playerCamera.localRotation,
                Quaternion.Euler(xRotation, 0f, 0f),
                bobSmoothSpeed * Time.deltaTime
            );

            // Reset bob timer gradually when stopping
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * 2f);
        }
    }

    void HandleFootsteps(bool isRunning)
    {
        if (audioSource == null || footstepSounds == null || footstepSounds.Length == 0)
            return;

        // Simplified footstep timing based on movement speed and time
        float stepInterval = isRunning ? 0.35f : 0.5f;

        if (Time.time - lastFootstepTime > stepInterval)
        {
            // Additional check: only play if we've moved a minimum distance
            if (smoothedMovementInput.magnitude > 0.3f)
            {
                PlayFootstepSound(isRunning);
                lastFootstepTime = Time.time;
            }
        }
    }

    void PlayFootstepSound(bool isRunning)
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip clipToPlay = footstepSounds[Random.Range(0, footstepSounds.Length)];
            float volume = isRunning ? footstepVolumeRun : footstepVolumeWalk;

            audioSource.clip = clipToPlay;
            audioSource.volume = volume;
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
            audioSource.Play();
        }
    }

    void DebugInfo()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"isMoving: {isMoving}");
            Debug.Log($"isGrounded: {controller.isGrounded}");
            Debug.Log($"CustomGrounded: {IsGrounded()}");
            Debug.Log($"bobTimer: {bobTimer}");
            Debug.Log($"Camera local pos: {playerCamera.localPosition}");
            Debug.Log($"Camera local rot: {playerCamera.localRotation}");
            Debug.Log($"Raw Input H: {Input.GetAxis("Horizontal")}, V: {Input.GetAxis("Vertical")}");
            Debug.Log($"Smoothed Input: {smoothedMovementInput}");
        }
    }

    bool IsGrounded()
    {
        Vector3 center = transform.position + controller.center;
        Vector3 bottom = center - Vector3.up * (controller.height / 2f);

        return Physics.CheckSphere(bottom, controller.radius + groundCheckDistance, groundMask);
    }
}