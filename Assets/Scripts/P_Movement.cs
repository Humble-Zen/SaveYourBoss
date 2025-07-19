using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class P_Movement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    //public CharacterController controller;
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;

    [Header("Bob Settings")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.2f; // Increased for more visible effect
    public float runBobSpeed = 14f;
    public float runBobAmount = 0.3f; // Increased for more visible effect
    public float bobRotationAmount = 5f; // Increased for more visible effect
    private Vector3 camInitialLocalPos;
    private Quaternion camInitialLocalRot;
    private float bobTimer = 0f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

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

    [Header("Debug")]
    public bool showDebugInfo = true;

    [Header("Ground Check")]
    public LayerMask groundMask; // Default layer
    public float groundCheckDistance = 0.1f;

    private CharacterController controller;
    private bool isMoving = false;
    private float lastFootstepTime = 0f;

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

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply horizontal movement
        move *= currentSpeed;

        // Apply gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;  // small downward force to keep grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // Update movement state
        isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);
    }


    void HandleMouseLook()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply horizontal rotation to player body
        transform.Rotate(Vector3.up * mouseX);
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

        // Use isMoving instead of requiring isGrounded - more reliable
        if (isMoving)
        {
            float currentBobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float currentBobAmount = isRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * currentBobSpeed;

            // Calculate bob offsets
            float bobOffsetY = Mathf.Sin(bobTimer) * currentBobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * currentBobAmount * 0.5f;

            // Apply position bob
            Vector3 targetPos = camInitialLocalPos + new Vector3(bobOffsetX, bobOffsetY, 0f);
            playerCamera.localPosition = targetPos;

            // Apply rotation bob combined with mouse look
            float tiltZ = Mathf.Sin(bobTimer) * bobRotationAmount;
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, tiltZ);

            // Handle footstep sounds - play when the bob hits the lowest point
            HandleFootsteps(isRunning);
        }
        else
        {
            // When not moving, smoothly return to original position
            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                camInitialLocalPos,
                Time.deltaTime * walkBobSpeed
            );

            playerCamera.localRotation = Quaternion.Lerp(
                playerCamera.localRotation,
                Quaternion.Euler(xRotation, 0f, 0f),
                Time.deltaTime * walkBobSpeed
            );
        }
    }

    void HandleFootsteps(bool isRunning)
    {
        if (audioSource == null || footstepSounds == null || footstepSounds.Length == 0)
            return;

        // Calculate when to play footsteps based on the bob cycle
        float stepInterval = isRunning ? (2f * Mathf.PI) / runBobSpeed : (2f * Mathf.PI) / walkBobSpeed;

        // Play footstep when the sin wave crosses zero going down (foot hits ground)
        float previousBobPhase = (bobTimer - Time.deltaTime * (isRunning ? runBobSpeed : walkBobSpeed)) % (2f * Mathf.PI);
        float currentBobPhase = bobTimer % (2f * Mathf.PI);

        // Check if we crossed the footstep trigger points (0 or ?)
        bool shouldPlayFootstep = false;

        if (previousBobPhase > currentBobPhase) // Wrapped around
        {
            shouldPlayFootstep = true;
        }
        else if ((previousBobPhase < Mathf.PI && currentBobPhase >= Mathf.PI) ||
                 (previousBobPhase < 2f * Mathf.PI && currentBobPhase >= 2f * Mathf.PI))
        {
            shouldPlayFootstep = true;
        }

        if (shouldPlayFootstep && Time.time - lastFootstepTime > stepInterval * 0.8f)
        {
            PlayFootstepSound(isRunning);
            lastFootstepTime = Time.time;
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
            Debug.Log($"Input H: {Input.GetAxis("Horizontal")}, V: {Input.GetAxis("Vertical")}");
        }
    }

    bool IsGrounded()
    {
        Vector3 center = transform.position + controller.center;
        Vector3 bottom = center - Vector3.up * (controller.height / 2f);

        return Physics.CheckSphere(bottom, controller.radius + groundCheckDistance, groundMask);
    }
}