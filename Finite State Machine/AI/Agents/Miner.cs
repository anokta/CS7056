using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteStateMachine
{
    // This class implements a Miner agent; the agent creates and maintains its own 
    // StateMachine that it invokes whenever the game asks it to update (triggered
    // by XNA's Update() method)
    public class Miner : Agent
    {
        // The following can be tweaked to change the basic behaviour of the Miner
        public int MaxNuggets = 3;
        public int ThirstLevel = 5;
        public int ComfortLevel = 5;
        public int TirednessThreshold = 5;

        // Here is the StateMachine that the Miner uses to drive the agent's behaviour
        private StateMachine<Miner> stateMachine;
        public StateMachine<Miner> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        // This is used to keep track of which other agent is our wife
        private int wifeId;
        public int WifeId
        {
            get { return wifeId; }
            set { wifeId = value; }
        }

        // And it knows its bank balance at any point in time
        private int moneyInBank;
        public int MoneyInBank
        {
            get { return moneyInBank; }
            set { moneyInBank = value; }
        }

        // The agent's thirst level increases by one for each update
        private int howThirsty;
        public int HowThirsty
        {
            get { return howThirsty; }
            set { howThirsty = value; }
        }

        // The agent's fatigue level is changed by the state machine
        private int howFatigued;
        public int HowFatigued
        {
            get { return howFatigued; }
            set { howFatigued = value; }
        }

        // The constructor invokes the base class constructor, which then creates 
        // an id for the new agent object and then creates and initalises the agent's
        // StateMachine
        public Miner()
            : base()
        {
            stateMachine = new StateMachine<Miner>(this);
            stateMachine.CurrentState = new GoHomeAndSleepTillRested();
            stateMachine.GlobalState = new MinerGlobalState();
            wifeId = this.Id + 1;  // hack hack

            Location = Location.shack;
        }

        // This method is invoked by the Game object as a result of XNA updates 
        public override void Update()
        {
            if (Location >= 0)
            {
                howThirsty += 1;
            }
            StateMachine.Update();
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

        // This method checks whether the agent's pockets are full or not, depending on the predefined level
        public Boolean PocketsFull()
        {
            if (goldCarrying >= MaxNuggets)
                return true;
            else
                return false;
        }

        // This method checks whether the agent is thirsty or not, depending on the predefined level
        public Boolean Thirsty()
        {
            if (howThirsty >= ThirstLevel)
                return true;
            else
                return false;
        }

        // This method checks whether the agent is fatigued or not, depending on the predefined level
        public Boolean Fatigued()
        {
            if (howFatigued >= TirednessThreshold)
                return true;
            else
                return false;
        }

        // This method checks whether the agent feels rich enough, depending on the predefined level
        public Boolean Rich()
        {
            if (moneyInBank >= ComfortLevel)
                return true;
            else
                return false;
        }
    }
}
