using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class LurkInOutlawCamp : State<Outlaw>
    {
        static Random rand = new Random();

        public override void Enter(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Back home, sweet home!");

            outlaw.BoredomCountdown = rand.Next(1, 10);
        }

        public override void Execute(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Chilling in " + LocationProperties.ToString(outlaw.Location) + ".");

            if (outlaw.Bored())
            {
                outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.cemetery, new LurkInCemetery()));
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
            Printer.Print(outlaw.Id, "Arrived in the cemetery!");

            outlaw.BoredomCountdown = rand.Next(1, 10);
        }

        public override void Execute(Outlaw outlaw)
        {
            Printer.Print(outlaw.Id, "Lurking in the cemetery.");

            if (outlaw.Bored())
            {
                outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.outlawCamp, new LurkInOutlawCamp()));
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
            Printer.Print(outlaw.Id, "Arrived in bank, Let's EARN some money!");
        }

        public override void Execute(Outlaw outlaw)
        {
            outlaw.GoldCarrying += rand.Next(1, 10);
            Printer.Print(outlaw.Id, "Total harvest now: " + outlaw.GoldCarrying);

            outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(outlaw.StateMachine.PreviousState.GetType() == typeof(LurkInOutlawCamp) ? Location.outlawCamp : Location.cemetery, outlaw.StateMachine.PreviousState));
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

    public class OutlawTravelToTarget : TravelToTarget<Outlaw>
    {
        public OutlawTravelToTarget(Location target, State<Outlaw> state)
        {
            targetPosition = LocationProperties.LocationCoords[(int)target];
            targetState = state;
        }

        public override void Enter(Outlaw outlaw)
        {
            path = pathFinder.FindPath(outlaw.CurrentPosition, targetPosition);

            Printer.Print(outlaw.Id, "Walkin' to " + LocationProperties.ToString(LocationProperties.GetLocation(targetPosition)) + ".");
        }

        public override void Execute(Outlaw outlaw)
        {
            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count; ++i)
                {
                    path[i].TintColor = Color.Red;
                    path[i].TintAlpha = 0.5f;
                }

                outlaw.CurrentPosition = path[0].Position;
                path.RemoveAt(0);
            }
            else
            {
                outlaw.CurrentPosition = targetPosition;

                State<Outlaw> previousState = outlaw.StateMachine.PreviousState;
                outlaw.StateMachine.ChangeState(targetState);
                outlaw.StateMachine.PreviousState = previousState;
            }
        }

        public override void Exit(Outlaw outlaw)
        {
            path.Clear();
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
            if (!outlaw.IsDead)
            {
                if (outlaw.StateMachine.CurrentState.GetType() != typeof(OutlawTravelToTarget))
                {
                    if (rand.Next(20) == 1 && !outlaw.StateMachine.IsInState(new AttemptToRobBank()))
                    {
                        outlaw.StateMachine.ChangeState(new OutlawTravelToTarget(Location.bank, new AttemptToRobBank()));
                    }
                }
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
