using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    public class Outlaw : Agent
    {
        public int BoredomCountdown = 0;

        // Here is the StateMachine that the Outlaw uses to drive the agent's behaviour
        private StateMachine<Outlaw> stateMachine;
        public StateMachine<Outlaw> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        public Outlaw()
            : base()
        {
            stateMachine = new StateMachine<Outlaw>(this);
            stateMachine.CurrentState = new LurkInOutlawCamp();
            stateMachine.GlobalState = new OutlawGlobalState();

            Location = Location.outlawCamp;
        }

        // This method is invoked by the Game object as a result of XNA updates 
        public override void Update()
        {
            if (Location >= 0)
            {
                BoredomCountdown -= 1;
            }

            stateMachine.Update();
        }

        public Boolean Bored()
        {
            return (BoredomCountdown <= 0);
        }

        // This method is invoked when the agent receives a message
        public override bool HandleMessage(Telegram telegram)
        {
            return stateMachine.HandleMessage(telegram);
        }

        // This method is invoked when the agent senses
        public override bool HandleSenseEvent(Sense sense)
        {
            return stateMachine.HandleSenseEvent(sense);
        }
    }
}
