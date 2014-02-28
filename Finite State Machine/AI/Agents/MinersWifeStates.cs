using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteStateMachine
{
    // In this state, the MinersWife agent does random house work
    public class DoHouseWork : State<MinersWife>
    {
        static Random rand = new Random();

        public override void Enter(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Time to do some more housework!");
        }

        public override void Execute(MinersWife minersWife)
        {
            switch (rand.Next(3))
            {
                case 0:
                    Printer.Print(minersWife.Id, "Moppin' the floor");
                    break;
                case 1:
                    Printer.Print(minersWife.Id, "Washin' the dishes");
                    break;
                case 2:
                    Printer.Print(minersWife.Id, "Makin' the bed");
                    break;
                default:
                    break;
            }
        }

        public override void Exit(MinersWife minersWife)
        {

        }

        public override bool OnMesssage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the MinersWife agent goes to the loo
    public class VisitBathroom : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Walkin' to the can. Need to powda mah pretty li'lle nose");
        }

        public override void Execute(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Ahhhhhh! Sweet relief!");
            minersWife.StateMachine.RevertToPreviousState();  // this completes the state blip
        }

        public override void Exit(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Leavin' the Jon");
        }

        public override bool OnMesssage(MinersWife minersWife, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the MinersWife prepares food
    public class CookStew : State<MinersWife>
    {
        public override void Enter(MinersWife minersWife)
        {
            if (!minersWife.Cooking)
            {
                // MinersWife sends a delayed message to herself to arrive when the food is ready
                Printer.Print(minersWife.Id, "Putting the stew in the oven");
                Message.DispatchMessage(2, minersWife.Id, minersWife.Id, MessageType.StewsReady);
                minersWife.Cooking = true;
            }
        }

        public override void Execute(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Fussin' over food");
        }

        public override void Exit(MinersWife minersWife)
        {
            Printer.Print(minersWife.Id, "Puttin' the stew on the table");
        }

        public override bool OnMesssage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
                    // Ignored here; handled in WifesGlobalState below
                    return false;
                case MessageType.StewsReady:
                    // Tell Miner that the stew is ready now by sending a message with no delay
                    Printer.PrintMessageData("Message handled by " + minersWife.Id + " at time ");
                    Printer.Print(minersWife.Id, "StewReady! Lets eat");
                    Message.DispatchMessage(0, minersWife.Id, minersWife.HusbandId, MessageType.StewsReady);
                    minersWife.Cooking = false;
                    minersWife.StateMachine.ChangeState(new DoHouseWork());
                    return true;
                default:
                    return false;
            }
        }
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class WifesGlobalState : State<MinersWife>
    {
        static Random rand = new Random();

        public override void Enter(MinersWife minersWife)
        {
           
        }

        public override void Execute(MinersWife minersWife)
        {
            // There's always a 10% chance of a state blip in which MinersWife goes to the bathroom
            if (rand.Next(10) == 1 && !minersWife.StateMachine.IsInState(new VisitBathroom()))
            {
                minersWife.StateMachine.ChangeState(new VisitBathroom());
            }
        }

        public override void Exit(MinersWife minersWife)
        {

        }

        public override bool OnMesssage(MinersWife minersWife, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
                    Printer.PrintMessageData("Message handled by " + minersWife.Id + " at time ");
                    Printer.Print(minersWife.Id, "Hi honey. Let me make you some of mah fine country stew");
                    minersWife.StateMachine.ChangeState(new CookStew());
                    return true;
                case MessageType.StewsReady:
                    return false;
                case MessageType.SheriffEncountered:
                    //Printer.PrintMessageData("Message handled by " + minersWife.Id + " at time ");
                    Printer.Print(minersWife.Id, "Good day to you too, sir!");
                    return true;
                default:
                    return false;
            }                 
        }
    }
}
