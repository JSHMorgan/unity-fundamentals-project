using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine
{
    internal class MoveState : IState
    {
        private PlayerController player;
        private readonly int animationHash = Animator.StringToHash("Move");
        public MoveState(PlayerController player)
        {
            this.player = player;
        }

        public void Enter()
        {
            player.PlayerAnimator.CrossFade(animationHash, 0, 0);
        }

        public void Execute()
        {
            // Here we add logic to detect if the conditions exist to
            // transition to another state
        }

        public void Exit()
        {
            // code that runs when we exit the state
        }
    }
}
