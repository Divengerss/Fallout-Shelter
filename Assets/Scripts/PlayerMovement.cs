using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float maxSpeed = 6f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("VFX")]
    public ParticleSystem walkParticles;
    public float walkParticleMinSpeed = 0.2f;

    [Header("Animation")]
    public Animator animator;
    public float movementThreshold = 0.1f;
    public float jumpThreshold = 0.3f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.45f;

    private float footstepTimer;


    private Rigidbody rb;
    private Vector3 currentVelocity;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
        if (!animator)
            animator = GetComponentInChildren<Animator>();
    }

    void UpdateAnimation()
    {
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > movementThreshold;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsInAir", !isGrounded);
    }

    void UpdateRotation()
    {
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void Update()
    {
        CheckGrounded();

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }

        UpdateWalkParticles();
        UpdateAnimation();
        UpdateRotation();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void UpdateWalkParticles()
    {
        if (!walkParticles) return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        bool shouldPlay =
            isGrounded &&
            horizontalVelocity.magnitude > walkParticleMinSpeed;

        if (shouldPlay && !walkParticles.isPlaying)
        {
            walkParticles.Play();
        }
        else if (!shouldPlay && walkParticles.isPlaying)
        {
            walkParticles.Stop();
        }
    }

    void PlayRandomFootstep()
    {
        int index = Random.Range(0, footstepClips.Length);
        footstepSource.PlayOneShot(footstepClips[index]);
    }

    void HandleFootsteps()
    {
        if (!isGrounded || footstepClips.Length == 0 || !footstepSource)
            return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.magnitude < movementThreshold)
            return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            PlayRandomFootstep();
            footstepTimer = footstepInterval;
        }
    }

    void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float xInput = 0f;
        float zInput = 0f;

        if (keyboard.aKey.isPressed) xInput -= 1f;
        if (keyboard.dKey.isPressed) xInput += 1f;
        if (keyboard.wKey.isPressed) zInput += 1f;
        if (keyboard.sKey.isPressed) zInput -= 1f;

        // Camera-relative directions
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * zInput + camRight * xInput;
        moveDirection = moveDirection.normalized;

        Vector3 targetVelocity = moveDirection * maxSpeed;
        float accelRate = moveDirection.magnitude > 0 ? acceleration : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            accelRate * Time.fixedDeltaTime
        );

        Vector3 velocity = rb.linearVelocity;
        velocity.x = currentVelocity.x;
        velocity.z = currentVelocity.z;
        rb.linearVelocity = velocity;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        animator.SetTrigger("Jump");
        animator.SetBool("IsInAir", true);
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance + 0.1f,
            groundLayer
        );
    }
}
