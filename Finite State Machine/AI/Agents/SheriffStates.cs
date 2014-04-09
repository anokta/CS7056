using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class PatrolRandomLocation : State<Sheriff>
    {
        static Random rand = new Random();

        public override void Enter(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Arrived!");
         
            sheriff.OutlawSpotted = false;
        }

        public override void Execute(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Patrolling in " + LocationProperties.ToString(sheriff.Location) + ".");

            if(sheriff.Location >= 0)
            for (int i = 0; i < Agent.AgentsCount; ++i)
            {
                if ((i != sheriff.Id) && (sheriff.Location == AgentManager.GetAgent(i).Location))
                {
                    if (typeof(Outlaw) == AgentManager.GetAgent(i).GetType()) // outlaw spotted
                    {
                        Printer.Print(sheriff.Id, "Sure glad to see you bandit, but hand me those guns.");
                        sheriff.OutlawSpotted = true;
                    }
                    else // greetings
                    {
                        Printer.Print(sheriff.Id, "Good day, townie!");
                    }

                    Message.DispatchMessage(0, sheriff.Id, i, MessageType.SheriffEncountered);

                    if (sheriff.OutlawSpotted) break;
                }
            }

            if (!sheriff.OutlawSpotted)
            {
                sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(sheriff.ChooseNextLocation(), new PatrolRandomLocation()));
            }
        }

        public override void Exit(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Leaving " + LocationProperties.ToString(sheriff.Location) + ".");
        }

        public override bool OnMesssage(Sheriff agent, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the sheriff goes to the bank and deposits gold
    public class StopByBankAndDepositGold : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Arrived in bank.");
        }

        public override void Execute(Sheriff sheriff)
        {
            sheriff.MoneyInBank += sheriff.GoldCarrying;
            sheriff.GoldCarrying = 0;
            Printer.Print(sheriff.Id, "Depositing gold. Total savings now: " + sheriff.MoneyInBank);

            sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(Location.saloon, new CelebrateTheDayInSaloon()));
        }

        public override void Exit(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Leaving the Bank, time to celebrate!");
        }

        public override bool OnMesssage(Sheriff agent, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the sheriff goes to the bank and deposits gold
    public class CelebrateTheDayInSaloon : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Arrived in the saloon!");
        }

        public override void Execute(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "All drinks on me today!");

            sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(sheriff.ChooseNextLocation(), new PatrolRandomLocation()));
        }

        public override void Exit(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Leaving the saloon.");
        }

        public override bool OnMesssage(Sheriff agent, Telegram telegram)
        {
            return false;
        }
    }


    public class DropDeadSheriff : State<Sheriff>
    {
        public override void Enter(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Goodbye, cruel world!");
            sheriff.IsDead = true;
        }

        public override void Execute(Sheriff sheriff)
        {
        }

        public override void Exit(Sheriff sheriff)
        {
            sheriff.IsDead = false;
            sheriff.Location = Location.sheriffsOffice;

            Printer.Print(sheriff.Id, "It's a miracle, I am alive!");
        }

        public override bool OnMesssage(Sheriff agent, Telegram telegram)
        {
            return false;
        }
    }

    public class SheriffTravelToTarget : TravelToTarget<Sheriff>
    {
        public SheriffTravelToTarget(Location target, State<Sheriff> state)
        {
            targetPosition = LocationProperties.LocationCoords[(int)target];
            targetState = state;
        }

        public override void Enter(Sheriff sheriff)
        {
            path = pathFinder.FindPath(sheriff.CurrentPosition, targetPosition);

            Printer.Print(sheriff.Id, "Walkin' to " + LocationProperties.ToString(LocationProperties.GetLocation(targetPosition)) + ".");
        }

        public override void Execute(Sheriff sheriff)
        {
            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count; ++i)
                {
                    path[i].TintColor = Color.Yellow;
                    path[i].TintAlpha = 0.5f;
                }

                sheriff.CurrentPosition = path[0].Position;
                path.RemoveAt(0);
            }
            else
            {
                sheriff.CurrentPosition = targetPosition;

                State<Sheriff> previousState = sheriff.StateMachine.PreviousState;
                sheriff.StateMachine.ChangeState(targetState);
                sheriff.StateMachine.PreviousState = previousState;
            }
        }

        public override void Exit(Sheriff sheriff)
        {
            path.Clear();
        }

        public override bool OnMesssage(Sheriff agent, Telegram telegram)
        {
            return false;
        }
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class SheriffGlobalState : State<Sheriff>
    {
        static Random rand = new Random();

        public override void Enter(Sheriff sheriff)
        {
        }

        public override void Execute(Sheriff sheriff)
        {
        }

        public override void Exit(Sheriff sheriff)
        {

        }

        public override bool OnMesssage(Sheriff sheriff, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.Gunfight:
                    // Notify the undertaker
                    Message.DispatchMessage(0, sheriff.Id, Agent.AgentsCount - 1, MessageType.Gunfight);

                    // Gunfight
                    Outlaw outlaw = (AgentManager.GetAgent(telegram.Sender) as Outlaw);

                    if (rand.Next(10) == 1) // sheriff dies
                    {

                        outlaw.GoldCarrying += sheriff.GoldCarrying;
                        sheriff.GoldCarrying = 0;

                        Message.DispatchMessage(0, sheriff.Id, sheriff.Id, MessageType.Dead);
                    }
                    else // outlaw dies
                    {
                        Printer.Print(sheriff.Id, "I am not coward, but I am so strong. It is hard to die.");

                        sheriff.GoldCarrying += outlaw.GoldCarrying;
                        outlaw.GoldCarrying = 0;

                        Message.DispatchMessage(0, sheriff.Id, outlaw.Id, MessageType.Dead);

                        sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(Location.bank, new StopByBankAndDepositGold()));
                    }

                    return true;
                case MessageType.Dead:
                    sheriff.StateMachine.ChangeState(new DropDeadSheriff());
                    return true;
                case MessageType.Respawn:
                    sheriff.StateMachine.ChangeState(new SheriffTravelToTarget(sheriff.ChooseNextLocation(), new PatrolRandomLocation()));
                    return true;
                default:
                    return false;
            }
        }
    }
}
