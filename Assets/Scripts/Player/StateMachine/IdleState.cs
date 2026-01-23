using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine
{
    internal class IdleState : IState
    {
        private PlayerController player;
        private readonly int animationHash = Animator.StringToHash("Idle");

        public IdleState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
            player.PlayerAnimator.CrossFade(animationHash, 0, 0);
        }

        public void Execute()
        {
            if (player.IsDashing)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.dashState);
            }
            else if (!player.IsGrounded)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.jumpState);
            }
            else if (player.Rigidbody.linearVelocityX != 0)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
        }

        public void Exit()
        {
            Debug.Log("Leaving Idle.");
        }
    }
}
