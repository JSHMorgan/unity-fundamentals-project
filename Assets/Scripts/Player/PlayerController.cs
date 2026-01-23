using Assets.Scripts.Player.StateMachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    internal PlayerInput PlayerInput { get; private set; }
    internal StateMachine PlayerStateMachine { get; private set; }
    internal Animator PlayerAnimator { get; private set; }
    internal bool IsGrounded { get; private set; }
    internal bool IsDashing { get; private set; }
    internal Rigidbody2D Rigidbody { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput = GetComponent<PlayerInput>();
        PlayerAnimator = GetComponent<Animator>();

        PlayerStateMachine = new StateMachine(this);
        PlayerStateMachine.Initialize(PlayerStateMachine.idleState);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStateMachine.Execute();
    }
}
