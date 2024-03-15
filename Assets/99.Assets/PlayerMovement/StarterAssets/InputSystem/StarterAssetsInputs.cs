using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool crouch;
        public bool aim;

        [Header("Movement Settings")] public bool analogMovement;

        //[Header("Mouse Cursor Settings")] public bool cursorInputForLook = true;


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (GameManager.instance.GetIsMovingAllowed())
                MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (GameManager.instance.GetCursorInput())
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            if (GameManager.instance.GetIsMovingAllowed())
                JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            if (GameManager.instance.GetIsMovingAllowed())
                SprintInput(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            if (GameManager.instance.GetIsMovingAllowed())
                CrouchInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            if (GameManager.instance.GetIsMovingAllowed())
                AimInput(value.isPressed);
        }
#endif

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void CrouchInput(bool newCrouchState)
        {
            crouch = newCrouchState;
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
    }
}