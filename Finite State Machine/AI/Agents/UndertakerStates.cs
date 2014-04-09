using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class HoverInTheOffice : State<Undertaker>
    {
        static Random rand = new Random();

        public override void Enter(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Arrived in the office!");
        }

        public override void Execute(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Hovering in the office.");
        }

        public override void Exit(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Leaving the office.");
        }

        public override bool OnMesssage(Undertaker undertaker, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.Gunfight:
                    Printer.Print(undertaker.Id, "Let's get down to business!");

                    undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(AgentManager.GetAgent(telegram.Sender).CurrentPosition, new LookForDeadBodies()));

                    return true;
                default:
                    return false;
            }
        }
    }

    public class LookForDeadBodies : State<Undertaker>
    {
        public override void Enter(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Arrived in " + LocationProperties.ToString(undertaker.Location) + ".");
        }

        public override void Execute(Undertaker undertaker)
        {
            for (int i = 0; i < Agent.AgentsCount; ++i)
            {
                if (AgentManager.GetAgent(i).IsDead)
                {
                    undertaker.CorpseID = i;
                }
            }

            Printer.Print(undertaker.Id, "Found the corpse of " + AgentManager.GetAgent(undertaker.CorpseID).GetType().Name + ".");

            if (undertaker.CorpseID >= 0)
            {
                undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(Location.cemetery, new DragOffTheBody()));
            }
        }

        public override void Exit(Undertaker undertaker)
        {
            if (undertaker.Location != Location.cemetery)
            {
                Printer.Print(undertaker.Id, "Leaving " + LocationProperties.ToString(undertaker.Location) + ".");
            }
        }

        public override bool OnMesssage(Undertaker undertaker, Telegram telegram)
        {
            return false;
        }
    }

    public class DragOffTheBody : State<Undertaker>
    {
        public override void Enter(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Carrying the body to the tombs in the cemetery!");
        }

        public override void Execute(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Dragging the body off. . . R.I.P.");

            Message.DispatchMessage(0, undertaker.Id, undertaker.CorpseID, MessageType.Respawn);
            undertaker.CorpseID = -1;

            undertaker.StateMachine.ChangeState(new UndertakerTravelToTarget(Location.undertakers, new HoverInTheOffice()));
        }

        public override void Exit(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Leaving the cemetery");
        }

        public override bool OnMesssage(Undertaker undertaker, Telegram telegram)
        {
            return false;
        }
    }

    public class UndertakerTravelToTarget : TravelToTarget<Undertaker>
    {
        public UndertakerTravelToTarget(Location target, State<Undertaker> state)
        {
            targetPosition = LocationProperties.LocationCoords[(int)target];
            targetState = state;
        }

        public UndertakerTravelToTarget(Vector2 target, State<Undertaker> state)
        {
            targetPosition = target;
            targetState = state;
        }

        public override void Enter(Undertaker undertaker)
        {
            path = pathFinder.FindPath(undertaker.CurrentPosition, targetPosition);

            Printer.Print(undertaker.Id, "Walkin' to " + LocationProperties.ToString(LocationProperties.GetLocation(targetPosition)) + ".");
        }

        public override void Execute(Undertaker undertaker)
        {
            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count; ++i)
                {
                    path[i].TintColor = Color.Black;
                    path[i].TintAlpha = 0.5f;
                }

                undertaker.CurrentPosition = path[0].Position;
                path.RemoveAt(0);
            }
            else
            {
                undertaker.CurrentPosition = targetPosition;

                State<Undertaker> previousState = undertaker.StateMachine.PreviousState;
                undertaker.StateMachine.ChangeState(targetState);
                undertaker.StateMachine.PreviousState = previousState;
            }

            if (undertaker.CorpseID >= 0)
            {
                AgentManager.GetAgent(undertaker.CorpseID).CurrentPosition = undertaker.CurrentPosition;
            }
        }

        public override void Exit(Undertaker undertaker)
        {
            path.Clear();
        }

        public override bool OnMesssage(Undertaker agent, Telegram telegram)
        {
            return false;
        }
    }


    // If the agent has a global state, then it is executed every Update() cycle
    public class UndertakerGlobalState : State<Undertaker>
    {
        static Random rand = new Random();

        public override void Enter(Undertaker undertaker)
        {
        }

        public override void Execute(Undertaker undertaker)
        {
        }

        public override void Exit(Undertaker undertaker)
        {

        }

        public override bool OnMesssage(Undertaker undertaker, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.SheriffEncountered:
                    Printer.Print(undertaker.Id, "Thank you sheriff, any 'sad' news?");
                    return true;
                default:
                    return false;
            }
        }
    }
}
