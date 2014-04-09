using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    public class Sheriff : Agent
    {
        public Boolean OutlawSpotted = false;

        // Here is the StateMachine that the Sheriff uses to drive the agent's behaviour
        private StateMachine<Sheriff> stateMachine;
        public StateMachine<Sheriff> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        // And it knows its bank balance at any point in time
        private int moneyInBank;
        public int MoneyInBank
        {
            get { return moneyInBank; }
            set { moneyInBank = value; }
        }

        public Sheriff()
            : base()
        {
            stateMachine = new StateMachine<Sheriff>(this);
            stateMachine.CurrentState = new PatrolRandomLocation();
            stateMachine.GlobalState = new SheriffGlobalState();

            Location = Location.sheriffsOffice;
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

        static Random rand = new Random();
        public Location ChooseNextLocation()
        {
            Location nextLocation = Location;
            while (nextLocation == Location.outlawCamp || nextLocation == Location)
                nextLocation = (Location)rand.Next(Enum.GetNames(typeof(Location)).Length);

            return nextLocation;
        }
    }
}
