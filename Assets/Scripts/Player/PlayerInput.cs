using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInput : MonoBehaviour
{
    public enum State
    {
        Moving,
        Jumping,
        Dashing,
        Idle
    }

    [SerializeField]
    private bool useLegacyInput = false;

    public State PlayerState { get ; private set; }
    public float Direction { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsDashing { get; private set; }

    private PlayerInputActions playerInput;
    
    private void Start()
    {
        if (useLegacyInput)
        {
            return;
        }

        playerInput = new PlayerInputActions();
        playerInput.Player.Enable();

        playerInput.Player.Move.performed += MovePerformed;
        playerInput.Player.Move.canceled += MoveCancelled;

        playerInput.Player.Jump.performed += JumpPerformed;
        playerInput.Player.Jump.canceled += JumpCancelled;

        playerInput.Player.Dash.performed += DashPerformed;
        playerInput.Player.Dash.canceled += DashCancelled;
    }

    private void Update()
    {
        if (useLegacyInput)
        {
            Direction = Input.GetAxisRaw("Horizontal");
            IsJumping = Input.GetKeyDown(KeyCode.Space);
            IsDashing = Input.GetKeyDown(KeyCode.J);
        }
    }

    private void MovePerformed (InputAction.CallbackContext context)
    {
        if (PlayerState == State.Idle)
        {
            PlayerState = State.Moving;
        }
        Direction = context.ReadValue<float>();
    }

    private void MoveCancelled (InputAction.CallbackContext context)
    {
        if (PlayerState == State.Moving)
        {
            PlayerState = State.Idle;
        }
        PlayerState = State.Idle;
    }

    private void JumpPerformed (InputAction.CallbackContext context)
    {
        IsJumping = true;
    }

    private void JumpCancelled (InputAction.CallbackContext context)
    {
        IsJumping = false;
        PlayerState = State.Idle;
    }

    private void DashPerformed (InputAction.CallbackContext context)
    {
        IsDashing = true;
    }

    private void DashCancelled (InputAction.CallbackContext context)
    {
        IsDashing = false;
        PlayerState = State.Idle;
    }
}
