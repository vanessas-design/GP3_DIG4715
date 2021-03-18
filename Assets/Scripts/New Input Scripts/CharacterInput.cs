using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class CharacterInput : MonoBehaviour, IInteractInput, IMoveInput, IRotationInput, ISkillInput
    {
        [Header("Input Commands")] public Command interactInput;
        public Command movementInput;
        public Command analogRotationInput;
        public Command mouseRotationInput;
        public Command skillInput;
 
        private PlayerInputActions inputActions;

        private Vector3 rotationDirection;
        private Vector3 moveDirection;
        private bool isPressingInteract;
        private bool isUsingSkill;

        public Vector3 MoveDirection { get; private set; }
        public Vector3 RotationDirection { get; set; }
        public bool IsUsingSkill { get; private set; }
        public bool IsPressingInteract { get; private set; }

        private void Awake()
        {
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();

            if (movementInput)
                inputActions.Player.Movement.performed += OnMoveInput;

            if (analogRotationInput)
                inputActions.Player.AnalogAim.performed += OnAnalogAimInput;

            if (mouseRotationInput)
                inputActions.Player.MouseAim.performed += OnMouseAimInput;
            
            if (skillInput)
                inputActions.Player.Skill.performed += OnSkillButton;
            
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            moveDirection = new Vector3(value.x, 0, value.y);

            if (movementInput != null)
                movementInput.Execute();
        }
       
        private void OnAnalogAimInput(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            RotationDirection = new Vector3(value.x, 0, value.y);

            if (analogRotationInput != null)
                analogRotationInput.Execute();
        }

        private void OnMouseAimInput(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();

            RotationDirection = new Vector3(value.x, 0, value.y);

            if (mouseRotationInput != null)
                mouseRotationInput.Execute();
        }

        private void OnInteractButton(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();

            IsPressingInteract = value >= 0.15f;

            if (interactInput != null && IsPressingInteract);
                interactInput.Execute();
        }

        private void OnSkillButton(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();

            IsUsingSkill = value >= 0.15f;

            if (skillInput != null && IsUsingSkill)
                skillInput.Execute();
        }

        private void OnDisable()
        {
            inputActions.Player.Movement.performed -= OnInteractButton;

            inputActions.Disable();
        }
    }
}
