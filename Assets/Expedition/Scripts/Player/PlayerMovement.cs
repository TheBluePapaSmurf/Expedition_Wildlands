using System.Collections;
using UnityEngine;
using Cinemachine;

namespace BDE.Expedition.PlayerControls
{
    [RequireComponent(typeof(CharacterController), typeof(InputHandler))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float originalSpeed = 7.5f;
        public float jumpSpeed = 10.0f;
        public float maxJumpHoldTime = 0.3f;
        public float gravity = -20.0f;
        public float rotationSpeed = 10.0f;
        public float accelerationTime = 0.1f;
        public float decelerationTime = 0.1f;
        public float airMovementMultiplier = 0.8f;

        [Header("Ground Detection")]
        public Transform groundCheck;
        public float groundDistance = 0.1f;
        public LayerMask groundMask;
        public float coyoteTime = 0.2f;

        [Header("Camera")]
        public Camera mainCamera;
        public CinemachineFreeLook freeLookCamera;

        [Header("Effects")]
        public ParticleSystem[] walkingParticleEffect;
        public GameObject jumpEffectPrefab;

        private CharacterController characterController;
        private InputHandler inputHandler;
        private Animator currentAnimator;
        private LedgeDetection ledgeDetection;

        private Vector3 velocity;
        private Vector3 horizontalVelocity;
        private float verticalVelocity;

        private bool isGrounded;
        private bool wasGrounded;
        private float coyoteTimeCounter;
        private int jumpCount = 0;
        private const int maxJumps = 2;

        private bool isHangingOnLedge = false;
        private bool hasLeftLedge = false;
        private bool areParticlesPlaying = false;

        private float jumpInputTimer = 0f;

        public bool IsGrounded => isGrounded;
        public bool IsMoving => horizontalVelocity.magnitude > 0.1f;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputHandler = GetComponent<InputHandler>();
            ledgeDetection = GetComponent<LedgeDetection>();

            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        void Start()
        {
            SetCurrentAnimator();
            UpdateMovementProperties();
            coyoteTimeCounter = coyoteTime;
        }

        void Update()
        {
            UpdateGroundState();
            HandleLedgeHanging();

            if (!isHangingOnLedge)
            {
                HandleJumpInput();
                HandleMovement();
                ApplyGravity();
                ApplyMovement();
                UpdateAnimations();
                HandleParticleEffects();
            }

            // Reset input flags
            inputHandler.ResetActionInputs();
        }

        void UpdateGroundState()
        {
            wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && !wasGrounded)
            {
                OnLanded();
            }
            else if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                hasLeftLedge = false;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }

        void HandleJumpInput()
        {
            if (inputHandler.JumpInput)
            {
                if (CanJump())
                {
                    PerformJump();
                    inputHandler.ConsumeJumpInput();
                }
            }

            // Handle variable jump height
            if (inputHandler.JumpInputHeld && verticalVelocity > 0 && jumpInputTimer < maxJumpHoldTime)
            {
                jumpInputTimer += Time.deltaTime;
                verticalVelocity += (jumpSpeed * 0.5f) * Time.deltaTime;
            }
            else if (!inputHandler.JumpInputHeld && verticalVelocity > 0)
            {
                verticalVelocity *= 0.5f; // Cut jump short
            }
        }

        bool CanJump()
        {
            return (isGrounded || coyoteTimeCounter > 0 || jumpCount < maxJumps);
        }

        void PerformJump()
        {
            verticalVelocity = jumpSpeed;
            jumpCount++;
            jumpInputTimer = 0f;

            if (jumpCount == 1)
            {
                currentAnimator?.SetTrigger("Jump");
            }
            else if (jumpCount == 2)
            {
                currentAnimator?.SetTrigger("DoubleJump");
            }

            // Spawn jump effect
            if (jumpEffectPrefab != null && groundCheck != null)
            {
                Instantiate(jumpEffectPrefab, groundCheck.position, Quaternion.identity);
            }
        }

        void HandleMovement()
        {
            Vector3 inputVector = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y);

            if (inputVector.magnitude < 0.1f)
            {
                // Decelerate when no input
                horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero,
                    originalSpeed / decelerationTime * Time.deltaTime);
            }
            else
            {
                // Transform input relative to camera
                Vector3 cameraForward = mainCamera.transform.forward;
                Vector3 cameraRight = mainCamera.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();

                Vector3 desiredDirection = (cameraForward * inputVector.z + cameraRight * inputVector.x).normalized;
                Vector3 targetVelocity = desiredDirection * originalSpeed;

                // Apply air movement penalty
                if (!isGrounded)
                    targetVelocity *= airMovementMultiplier;

                // Accelerate towards target velocity
                horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity,
                    originalSpeed / accelerationTime * Time.deltaTime);

                // Rotate towards movement direction
                if (horizontalVelocity.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                        rotationSpeed * Time.deltaTime);
                }
            }
        }

        void ApplyGravity()
        {
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Small downward force to keep grounded
            }
            else
            {
                float gravityMultiplier = verticalVelocity < 0 ? 1.5f : 1f; // Fall faster than rise
                verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;
            }
        }

        void ApplyMovement()
        {
            velocity = horizontalVelocity + Vector3.up * verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }

        void UpdateAnimations()
        {
            if (currentAnimator == null) return;

            float speed = horizontalVelocity.magnitude / originalSpeed;
            currentAnimator.SetFloat("Speed", speed);
            currentAnimator.SetBool("IsGrounded", isGrounded);
            currentAnimator.SetBool("IsInAir", !isGrounded);
            currentAnimator.SetFloat("VerticalVelocity", verticalVelocity);
        }

        void HandleParticleEffects()
        {
            bool shouldPlayParticles = isGrounded && IsMoving;

            if (shouldPlayParticles && !areParticlesPlaying)
            {
                foreach (var particle in walkingParticleEffect)
                    particle?.Play();
                areParticlesPlaying = true;
            }
            else if (!shouldPlayParticles && areParticlesPlaying)
            {
                foreach (var particle in walkingParticleEffect)
                    particle?.Stop();
                areParticlesPlaying = false;
            }
        }

        void OnLanded()
        {
            jumpCount = 0;
            verticalVelocity = 0f;
            jumpInputTimer = 0f;
            currentAnimator?.SetTrigger("Land");
        }

        void HandleLedgeHanging()
        {
            if (!hasLeftLedge && ledgeDetection != null && ledgeDetection.IsLedgeDetected() &&
                !isGrounded && !isHangingOnLedge)
            {
                StartHanging();
            }

            if (isHangingOnLedge)
            {
                if (inputHandler.JumpInput)
                {
                    StopHanging();
                    PerformJump();
                    inputHandler.ConsumeJumpInput();
                }
                else if (inputHandler.StopHangingInput)
                {
                    StopHanging();
                }
            }
        }

        void StartHanging()
        {
            isHangingOnLedge = true;
            verticalVelocity = 0;
            horizontalVelocity = Vector3.zero;
            characterController.enabled = false;
            currentAnimator?.SetBool("IsGrabbingLedge", true);
            jumpCount = 1;
        }

        void StopHanging()
        {
            isHangingOnLedge = false;
            characterController.enabled = true;
            hasLeftLedge = true;
            currentAnimator?.SetBool("IsGrabbingLedge", false);
        }

        public void SetCurrentAnimator()
        {
            currentAnimator = CharacterManager.Instance?.GetCurrentAnimator();
        }

        public void UpdateMovementProperties()
        {
            var movementProperties = CharacterManager.Instance?.GetCurrentMovementProperties();
            if (movementProperties != null)
            {
                originalSpeed = movementProperties.originalSpeed;
                jumpSpeed = movementProperties.jumpSpeed;
                maxJumpHoldTime = movementProperties.maxJumpHoldTime;
                gravity = movementProperties.gravity;
                rotationSpeed = movementProperties.rotationSpeed;
            }
        }

        public Vector3 GetForwardDirection() => transform.forward;
        public Vector3 GetVelocity() => velocity;
    }
}
