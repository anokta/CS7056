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
            Printer.Print(undertaker.Id, "Going back to the office!");
            undertaker.TargetLocation = Location.undertakers;
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
                    undertaker.TargetLocation = AgentManager.GetAgent(telegram.Sender).Location;
                    undertaker.StateMachine.ChangeState(new LookForDeadBodies());

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
                undertaker.StateMachine.ChangeState(new DragOffTheBody());
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
            undertaker.TargetLocation = Location.cemetery;
            AgentManager.GetAgent(undertaker.CorpseID).Location = Location.cemetery;
        }

        public override void Execute(Undertaker undertaker)
        {
            Printer.Print(undertaker.Id, "Dragging the body off. . . R.I.P.");

            Message.DispatchMessage(2, undertaker.Id, undertaker.CorpseID, MessageType.Respawn);

            undertaker.StateMachine.ChangeState(new HoverInTheOffice());
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

    public class UndertakerTravelToTarget : State<Undertaker>
    {
        private static AStar pathFinder = new AStar();
        private List<Tile> path;

        public override void Enter(Undertaker undertaker)
        {
            path = pathFinder.FindPath(undertaker.CurrentPosition, LocationProperties.LocationCoords[(int)undertaker.TargetLocation]);
            undertaker.Location = (Location)(-1);
        }

        public override void Execute(Undertaker undertaker)
        {
            if (path.Count > 0)
            {
                foreach (Tile tile in path)
                {
                    tile.TintColor = Color.Black;
                    tile.TintAlpha = 0.5f;
                }

                undertaker.CurrentPosition = path[0].Position;
                path.RemoveAt(0);
            }
            else
            {
                undertaker.Location = undertaker.TargetLocation;
                undertaker.StateMachine.RevertToPreviousState();
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
