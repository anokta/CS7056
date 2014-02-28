using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    public class LurkInOutlawCamp : State<Outlaw>
    {
        static Random rand = new Random();

        public override void Enter(Outlaw outlaw)
        {
            outlaw.Location = Location.outlawCamp;
            Printer.Print(outlaw.Id, "Going back home, sweet home!");

            outlaw.BoredomCountdown = rand.Next(1, 10);
        }

        public override void Execute(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Chilling in " + LocationProperties.ToString(outlaw.Location) + ".");

            if (outlaw.Bored())
            {
                outlaw.StateMachine.ChangeState(new LurkInCemetery());
            }
        }

        public override void Exit(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Leaving the camp.");
        }

        public override bool OnMesssage(Outlaw agent, Telegram telegram)
        {
            return false;
        }
    }

    public class LurkInCemetery : State<Outlaw>
    {
        static Random rand = new Random();

        public override void Enter(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Going to the cemetery!");
            outlaw.Location = Location.cemetery;

            outlaw.BoredomCountdown = rand.Next(1, 10);
        }

        public override void Execute(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Lurking in the cemetery.");

            if (outlaw.Bored())
            {
                outlaw.StateMachine.ChangeState(new LurkInOutlawCamp());
            }
        }

        public override void Exit(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Leaving the cemetery.");
        }

        public override bool OnMesssage(Outlaw agent, Telegram telegram)
        {
            return false;
        }
    }

    public class AttemptToRobBank : State<Outlaw>
    {
        static Random rand = new Random();

        public override void Enter(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Going to the bank, Let's EARN some money!");
            outlaw.Location = Location.bank;
        }

        public override void Execute(Outlaw outlaw)
        {
            outlaw.GoldCarrying += rand.Next(1, 10);
            Printer.Print(outlaw.Id, "Total harvest now: " + outlaw.GoldCarrying);

            outlaw.StateMachine.RevertToPreviousState();
        }

        public override void Exit(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Breaking away from the bank.");
        }

        public override bool OnMesssage(Outlaw agent, Telegram telegram)
        {
            return false;
        }
    }

    public class DropDeadOutlaw : State<Outlaw>
    {

        public override void Enter(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "I will come back, from dead!");
            outlaw.IsDead = true;
        }

        public override void Execute(Outlaw outlaw)
        {
        }

        public override void Exit(Outlaw outlaw)
        {
            outlaw.IsDead = false;
            outlaw.Location = Location.outlawCamp;
            Printer.Print(outlaw.Id, "I am back, from dead!");
        }

        public override bool OnMesssage(Outlaw agent, Telegram telegram)
        {
            return false;
        }
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class OutlawGlobalState : State<Outlaw>
    {
        static Random rand = new Random();

        public override void Enter(Outlaw outlaw)
        {
        }

        public override void Execute(Outlaw outlaw)
        {
            if (rand.Next(20) == 1 && !outlaw.StateMachine.IsInState(new AttemptToRobBank()))
            {
                outlaw.StateMachine.ChangeState(new AttemptToRobBank());
            }
        }

        public override void Exit(Outlaw outlaw)
        {

        }

        public override bool OnMesssage(Outlaw outlaw, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.SheriffEncountered:
                    Printer.Print(outlaw.Id, "Prepare to die instead, sheriff!");
                    Message.DispatchMessage(0, outlaw.Id, telegram.Sender, MessageType.Gunfight);
                    return true;
                case MessageType.Dead:
                    outlaw.StateMachine.ChangeState(new DropDeadOutlaw());
                    return true;
                case MessageType.Respawn:
                    outlaw.StateMachine.ChangeState(new LurkInOutlawCamp());
                    return true;
            }
            return false;
        }
    }
}
