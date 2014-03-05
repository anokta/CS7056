using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    public class Undertaker : Agent
    {
        public int CorpseID = -1;

        // Here is the StateMachine that the Outlaw uses to drive the agent's behaviour
        private StateMachine<Undertaker> stateMachine;
        public StateMachine<Undertaker> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        public Undertaker()
            : base()
        {
            stateMachine = new StateMachine<Undertaker>(this);
            stateMachine.CurrentState = new HoverInTheOffice();
            stateMachine.GlobalState = new UndertakerGlobalState();

            Location = Location.undertakers;
            TargetLocation = Location.undertakers;
        }

        // This method is invoked by the Game object as a result of XNA updates 
        public override void Update()
        {
            stateMachine.Update();
        }

        // This method is invoked when the agent receives a message
        public override bool HandleMessage(Telegram telegram)
        {
            return stateMachine.HandleMessage(telegram);
        }
    }
}
