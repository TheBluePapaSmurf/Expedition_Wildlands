using UnityEngine;
using UnityEngine.InputSystem;

namespace BDE.Expedition.PlayerControls
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Input Settings")]

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool JumpInputHeld { get; private set; }
        public bool AttackInput { get; private set; }
        public bool ThrowAttackInput { get; private set; }
        public bool SpecialAttackInput { get; private set; }
        public bool StopHangingInput { get; private set; }

        private InputActions inputActions;
        private bool jumpInputConsumed = false;

        void Awake()
        {
            inputActions = new InputActions();

            // Subscribe to input events
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;

            inputActions.Player.Jump.performed += OnJump;
            inputActions.Player.Jump.canceled += OnJump;

            inputActions.Player.Attack.performed += OnAttack;
            inputActions.Player.ThrowAttack.performed += OnThrowAttack;
            inputActions.Player.SpecialAttack.performed += OnSpecialAttack;
            inputActions.Player.StopHanging.performed += OnStopHanging;
        }

        void OnEnable()
        {
            inputActions.Player.Enable();
        }

        void OnDisable()
        {
            inputActions.Player.Disable();
        }

        void OnDestroy()
        {
            if (inputActions != null)
            {
                // Unsubscribe from input events
                inputActions.Player.Move.performed -= OnMove;
                inputActions.Player.Move.canceled -= OnMove;

                inputActions.Player.Jump.performed -= OnJump;
                inputActions.Player.Jump.canceled -= OnJump;

                inputActions.Player.Attack.performed -= OnAttack;
                inputActions.Player.ThrowAttack.performed -= OnThrowAttack;
                inputActions.Player.SpecialAttack.performed -= OnSpecialAttack;
                inputActions.Player.StopHanging.performed -= OnStopHanging;

                inputActions.Dispose();
            }
        }

        void Update()
        {
            // Reset one-frame inputs after they've been processed
            if (jumpInputConsumed)
            {
                JumpInput = false;
                jumpInputConsumed = false;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                JumpInput = true;
                JumpInputHeld = true;
            }
            else if (context.canceled)
            {
                JumpInputHeld = false;
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                AttackInput = true;
        }

        public void OnThrowAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                ThrowAttackInput = true;
        }

        public void OnSpecialAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                SpecialAttackInput = true;
        }

        public void OnStopHanging(InputAction.CallbackContext context)
        {
            if (context.performed)
                StopHangingInput = true;
        }

        public void ConsumeJumpInput()
        {
            jumpInputConsumed = true;
        }

        public void ResetActionInputs()
        {
            AttackInput = false;
            ThrowAttackInput = false;
            SpecialAttackInput = false;
            StopHangingInput = false;
        }
    }
}
