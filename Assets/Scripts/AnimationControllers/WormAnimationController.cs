using System;
using UnityEngine;

[RequireComponent(typeof(WanderingEnemyController))]
[RequireComponent(typeof(Animator))]
public class WormAnimationController : MonoBehaviour
{
    private Animator animator;
    private WanderingEnemyController enemyController;
    private static readonly int walk = Animator.StringToHash("Walk");
    private static readonly int rest = Animator.StringToHash("Rest");
    private int currentState;

    void Start()
    {
        enemyController = GetComponent<WanderingEnemyController>();
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
        return enemyController.Rb2D.linearVelocityX switch
        {
            > 0 or < 0 => walk,
            0 => rest,
            _ => throw new ArgumentOutOfRangeException(nameof(enemyController.Rb2D.linearVelocityX), $"Not expected state value: {enemyController.Rb2D.linearVelocityX}")
        };
    }
}
