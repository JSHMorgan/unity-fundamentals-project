using System;
using UnityEngine;

[RequireComponent (typeof(FallingBlockController))]
[RequireComponent (typeof(Animator))]
public class FallingBlockAnimationController : MonoBehaviour
{
    private Animator animator;
    private FallingBlockController fallingBlockController;
    private static readonly int fall = Animator.StringToHash("Fall");
    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int rest = Animator.StringToHash("Rest");
    
    private int currentState;

    private void Start()
    {
        fallingBlockController = GetComponent<FallingBlockController>();
        animator = GetComponent<Animator>();
        currentState = rest;
        animator.CrossFade("Rest", 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        int state = GetState();
        if (state == currentState)
        {
            return;
        }
        animator.CrossFade(state, 0, 0);
        currentState = state;
    }

    private int GetState()
    {
        return fallingBlockController.CurrentState switch
        {
            FallingBlockController.State.Fall => fall,
            FallingBlockController.State.Rest => rest,
            FallingBlockController.State.Reset => idle,
            _ => throw new ArgumentOutOfRangeException(nameof(fallingBlockController.CurrentState), $"Not expected state value: {fallingBlockController.CurrentState}")
        };
    }
}
