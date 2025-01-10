using UnityEngine;

namespace BDE.Expedition.PlayerControls
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Movement Keys")]
        public KeyCode moveUp = KeyCode.W;
        public KeyCode moveDown = KeyCode.S;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;

        [Header("Action Keys")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode specialAttackKey = KeyCode.E;
        public KeyCode throwAttackKey = KeyCode.Mouse1; // Right mouse button
        public KeyCode normalAttackKey = KeyCode.Mouse0; // Left mouse button
        public KeyCode jumpToBoomerangKey = KeyCode.E;
        public KeyCode stopHangingKey = KeyCode.S;

        public Vector2 MovementInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool SpecialAttackInput { get; private set; }
        public bool AttackInput { get; private set; }
        public bool ThrowAttackInput { get; private set; }
        public bool JumpToBoomerangInput { get; private set; }
        public bool StopHangingInput { get; private set; }

        void Update()
        {
            MovementInput = GetMovementInput();
            JumpInput = Input.GetKeyDown(jumpKey);
            SpecialAttackInput = Input.GetKeyDown(specialAttackKey);
            AttackInput = Input.GetKeyDown(normalAttackKey);
            ThrowAttackInput = Input.GetKeyDown(throwAttackKey);
            JumpToBoomerangInput = Input.GetKeyDown(jumpToBoomerangKey);
            StopHangingInput = Input.GetKeyDown(stopHangingKey);
        }

        private Vector2 GetMovementInput()
        {
            float x = 0f;
            float y = 0f;

            if (Input.GetKey(moveUp))
                y += 1f;
            if (Input.GetKey(moveDown))
                y -= 1f;
            if (Input.GetKey(moveLeft))
                x -= 1f;
            if (Input.GetKey(moveRight))
                x += 1f;

            return new Vector2(x, y).normalized;
        }

        public void ResetInputs()
        {
            JumpInput = false;
            SpecialAttackInput = false;
            AttackInput = false;
            ThrowAttackInput = false;
            JumpToBoomerangInput = false;
            StopHangingInput = false;
        }
    }
}
