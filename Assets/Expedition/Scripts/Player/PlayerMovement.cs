using System.Collections;
using UnityEngine;
using Cinemachine;

namespace BDE.Expedition.PlayerControls
{
    public class PlayerMovement : MonoBehaviour
    {
        public Camera mainCamera;

        public static bool isGameLoading = false;
        public float originalSpeed = 7.5f;
        public float jumpSpeed = 10.0f;
        public float maxJumpHoldTime = 0.1f;
        public float gravity = -20.0f;
        public Transform groundCheck;
        public float groundDistance = 0.1f;
        public LayerMask groundMask;
        public float coyoteTime = 0.2f;
        public float rotationSpeed = 10.0f;

        public float accelerationTime = 0.1f;
        public float decelerationTime = 0.1f;

        public ParticleSystem[] walkingParticleEffect;
        public GameObject jumpEffectPrefab;
        private bool areParticlesPlaying = false;

        private CharacterController characterController;
        private Animator currentAnimator;
        private float verticalVelocity = 0.0f;
        private int jumpCount = 0;
        private float jumpButtonHoldTime = 0f;
        private bool isGrounded;
        private float coyoteTimeCounter;
        private Vector3 currentVelocity;
        private Vector3 targetVelocity;
        private bool isJumpingToBoomerang = false;
        private GameObject currentBoomerang;

        private LedgeDetection ledgeDetection;
        private bool isHangingOnLedge = false;
        private bool hasLeftLedge = false;

        private InputHandler inputHandler;

        public CinemachineFreeLook freeLookCamera;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            ledgeDetection = GetComponent<LedgeDetection>();
            inputHandler = GetComponent<InputHandler>();

            SetCurrentAnimator();
            UpdateMovementProperties();
            coyoteTimeCounter = coyoteTime;
        }

        void Update()
        {
            if (isGrounded)
            {
                hasLeftLedge = false; // Reset hasLeftLedge when the player is grounded
            }

            if (!hasLeftLedge && ledgeDetection.IsLedgeDetected() && !isGrounded && !isHangingOnLedge)
            {
                StartHanging();
            }

            if (isHangingOnLedge)
            {
                HandleHangingInput();
            }
            else
            {
                GroundCheck();
                ProcessJumpInput();
                Move();
                ApplyGravity();
                HandleAnimatorStates();
            }
        }

        public void SetCurrentAnimator()
        {
            currentAnimator = CharacterManager.Instance.GetCurrentAnimator();
        }

        public void UpdateMovementProperties()
        {
            var movementProperties = CharacterManager.Instance.GetCurrentMovementProperties();
            originalSpeed = movementProperties.originalSpeed;
            jumpSpeed = movementProperties.jumpSpeed;
            maxJumpHoldTime = movementProperties.maxJumpHoldTime;
            gravity = movementProperties.gravity;
            rotationSpeed = movementProperties.rotationSpeed;
        }

        void GroundCheck()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && verticalVelocity < 0)
            {
                currentAnimator.SetBool("IsJumping", false);
                currentAnimator.SetBool("IsDoubleJumping", false);
                currentAnimator.SetBool("IsInAir", false);
                verticalVelocity = -2f;
                jumpCount = 0;

                if (!areParticlesPlaying)
                {
                    foreach (ParticleSystem particleEffect in walkingParticleEffect)
                    {
                        particleEffect.Play();
                    }
                    areParticlesPlaying = true;
                }
            }
            else if (!wasGrounded && isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                currentAnimator.SetBool("IsInAir", false);

                if (!areParticlesPlaying)
                {
                    foreach (ParticleSystem particleEffect in walkingParticleEffect)
                    {
                        particleEffect.Play();
                    }
                    areParticlesPlaying = true;
                }
            }
            else if (!isGrounded)
            {
                coyoteTimeCounter -= Time.deltaTime;
                currentAnimator.SetBool("IsInAir", true);

                if (areParticlesPlaying)
                {
                    foreach (ParticleSystem particleEffect in walkingParticleEffect)
                    {
                        particleEffect.Stop();
                    }
                    areParticlesPlaying = false;
                }
            }
        }

        void ProcessJumpInput()
        {
            if (inputHandler.JumpInput)
            {
                Jump(maxJumpHoldTime);
                inputHandler.ResetInputs();
            }

            if (inputHandler.SpecialAttackInput)
            {
                JumpToBoomerang();
                inputHandler.ResetInputs();
            }
        }

        void Move()
        {
            Vector3 inputVector = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y).normalized;

            // Transformeer de invoer vector naar wereldruimte
            Vector3 transformedInput = mainCamera.transform.TransformDirection(inputVector);
            transformedInput.y = 0f; // We negeren de y-component om ervoor te zorgen dat de beweging in het xz-vlak blijft
            transformedInput.Normalize();

            currentAnimator.SetFloat("Speed", transformedInput.magnitude);

            // Beweeg de speler in de wereldruimte gebaseerd op de getransformeerde invoer vector
            targetVelocity = transformedInput * originalSpeed;

            if (!isGrounded)
            {
                targetVelocity *= 0.9f;
            }

            if (transformedInput.magnitude > 0)
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, originalSpeed / accelerationTime * Time.deltaTime);
            }
            else
            {
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, originalSpeed / decelerationTime * Time.deltaTime);
            }

            // Verplaats de speler
            characterController.Move(currentVelocity * Time.deltaTime);

            // Draaien naar de bewegingsrichting alleen als er beweging is
            if (currentVelocity != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Voeg de verticale snelheid toe
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        void Jump(float heldTime)
        {
            if (isGrounded || coyoteTimeCounter > 0 || (jumpCount < 2 && !isGrounded))
            {
                float jumpHeightMultiplier = Mathf.Clamp(heldTime / maxJumpHoldTime, 0.75f, 1.25f);
                verticalVelocity = jumpSpeed * jumpHeightMultiplier;
                currentAnimator.SetBool("IsJumping", true);
                jumpCount++;

                if (jumpCount == 2)
                {
                    StartCoroutine(ActivateDoubleJumpAnimation());
                }
            }
        }

        void JumpToBoomerang()
        {
            if (currentBoomerang && currentBoomerang.GetComponent<Boomerang>().IsHovering)
            {
                StartCoroutine(JumpToBoomerangCoroutine(currentBoomerang.transform.position));
                isJumpingToBoomerang = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Boomerang") && isJumpingToBoomerang)
            {
                Jump(maxJumpHoldTime);
                currentAnimator.SetBool("IsDoubleJumping", true);
                isJumpingToBoomerang = false;
            }
        }

        void ApplyGravity()
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity += gravity * 1.5f * Time.deltaTime;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        void HandleAnimatorStates()
        {
            currentAnimator.SetBool("IsInAir", !isGrounded);

            if (currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throwing") && currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                currentAnimator.SetBool("IsThrowing", false);
            }
        }

        void StartHanging()
        {
            isHangingOnLedge = true;
            verticalVelocity = 0;
            currentVelocity = Vector3.zero;
            characterController.enabled = false; // Disable the CharacterController to prevent it from moving

            currentAnimator.SetBool("IsGrabbingLedge", true);
            jumpCount = 1;
        }

        void StopHanging()
        {
            isHangingOnLedge = false;
            characterController.enabled = true; // Re-enable the CharacterController
            hasLeftLedge = true; // Mark that the player has left the ledge

            currentAnimator.SetBool("IsGrabbingLedge", false);
        }

        void HandleHangingInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Logic to let the player jump off the ledge
                StopHanging();
                Jump(maxJumpHoldTime);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                // Logic to let the player drop from the ledge
                StopHanging();
            }
        }

        IEnumerator JumpToBoomerangCoroutine(Vector3 target)
        {
            float jumpDuration = 0.5f;
            float time = 0;
            Vector3 startPosition = transform.position;

            while (time < jumpDuration)
            {
                transform.position = Vector3.Lerp(startPosition, target, time / jumpDuration);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
            isJumpingToBoomerang = false;
        }

        IEnumerator ActivateDoubleJumpAnimation()
        {
            currentAnimator.SetBool("IsDoubleJumping", true);
            yield return new WaitForSeconds(0.5f);
            currentAnimator.SetBool("IsDoubleJumping", false);
        }

        // Coroutine om het prefab na een vertraging te vernietigen
        IEnumerator DestroyAfterSeconds(GameObject obj, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(obj);
        }

        public Vector3 GetForwardDirection()
        {
            return transform.forward;
        }
    }
}
