using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
            if (virtualMoveDirection.sqrMagnitude >= 0.9)
            {
                VirtualSprintInput(true);
            }
            else
            {
                VirtualSprintInput(false);
            }
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualMorphInput(bool virtualMorphState)
        {
            starterAssetsInputs.MorphInput();
        }
        
        public void VirtualSelectInput(bool virtualSelectState)
        {
            starterAssetsInputs.ChooseInput(virtualSelectState);
        }

        public void VirtualDeadInput(bool virtualDeadState)
        {
            starterAssetsInputs.DeadInput(virtualDeadState);
        }

        public void VirtualVoiceInput(bool virtualMorphState)
        {
            starterAssetsInputs.ConnectInput();
        }
    }

}
