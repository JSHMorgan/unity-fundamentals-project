using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player.StateMachine
{
    [Serializable]
    internal class StateMachine
    {
        public IState CurrentState { get; set; }

        public MoveState moveState;
        public JumpState jumpState;
        public DashState dashState;
        public IdleState idleState;

        public StateMachine(PlayerController player) 
        {
            moveState = new MoveState(player);
            jumpState = new JumpState(player);
            dashState = new DashState(player);
            idleState = new IdleState(player);
        }
        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void TransitionTo(IState nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();
        }

        public void Execute()
        {
            if (CurrentState == null)
            {
                return;
            }
            CurrentState.Execute();
        }

    }
}
