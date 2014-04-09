using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteStateMachine
{
    // This class implements a MinersWife agent; the agent creates and maintains its own 
    // StateMachine that it invokes whenever the game asks it to update (triggered
    // by XNA's Update() method)
    public class MinersWife : Agent
    {
        private StateMachine<MinersWife> stateMachine;
        public StateMachine<MinersWife> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        // This is used to keep track of which other agent is our husband
        private int husbandId;
        public int HusbandId
        {
            get { return husbandId; }
            set { husbandId = value; }
        }

        private Boolean cooking;
        public Boolean Cooking
        {
            get { return cooking; }
            set { cooking = value; }
        }

        // The constructor invokes the base class constructor, which then creates 
        // an id for the new agent object and then creates and initalises the agent's
        // StateMachine
        public MinersWife() : base()
        {
            stateMachine = new StateMachine<MinersWife>(this);
            stateMachine.CurrentState = new DoHouseWork();
            stateMachine.GlobalState = new WifesGlobalState();
            husbandId = this.Id - 1;  // hack hack

            Location = Location.shack;
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

        // This method is invoked when the agent senses
        public override bool HandleSenseEvent(Sense sense)
        {
            return stateMachine.HandleSenseEvent(sense);
        }
    }
}
