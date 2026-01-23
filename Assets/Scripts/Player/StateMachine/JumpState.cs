using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine
{
    internal class JumpState : IState
    {
        private PlayerController player;
        private readonly int animationHash = Animator.StringToHash("Jump");
        public JumpState(PlayerController player)
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

            // If the player is not grounded, keep jumping.
            if (!player.IsGrounded)
            {
                return;
            }

            if (player.Rigidbody.linearVelocityX != 0)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
            else
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
            }
        }
        public void Exit()
        {
            // code that runs when we exit the state
        }
    }
}
