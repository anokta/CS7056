using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    public class PatrolRandomLocation : State<Sheriff>
    {
        static Random rand = new Random();

        public override void Enter(Sheriff sheriff)
        {
            Location nextLocation = sheriff.Location;
            while (nextLocation == Location.outlawCamp || nextLocation == sheriff.Location)
                nextLocation = (Location)rand.Next(Enum.GetNames(typeof(Location)).Length);

            Printer.Print(sheriff.Id, "Going to " + LocationPropertes.ToString(nextLocation) + "!");
            sheriff.Location = nextLocation;

            sheriff.OutlawSpotted = false;
        }

        public override void Execute(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Patrolling in " + LocationPropertes.ToString(sheriff.Location) + ".");

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
                sheriff.StateMachine.ChangeState(new PatrolRandomLocation());
            }
        }

        public override void Exit(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "Leaving " + LocationPropertes.ToString(sheriff.Location) + ".");
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
            Printer.Print(sheriff.Id, "Going to the bank.");
            sheriff.Location = Location.bank;
        }

        public override void Execute(Sheriff sheriff)
        {
            sheriff.MoneyInBank += sheriff.GoldCarrying;
            sheriff.GoldCarrying = 0;
            Printer.Print(sheriff.Id, "Depositing gold. Total savings now: " + sheriff.MoneyInBank);

            sheriff.StateMachine.ChangeState(new CelebrateTheDayInSaloon());
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
            Printer.Print(sheriff.Id, "Going to the saloon!");
            sheriff.Location = Location.saloon;
        }

        public override void Execute(Sheriff sheriff)
        {
            Printer.Print(sheriff.Id, "All drinks on me today!");

            sheriff.StateMachine.ChangeState(new PatrolRandomLocation());
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

                        sheriff.StateMachine.ChangeState(new StopByBankAndDepositGold());
                    }

                    return true;
                case MessageType.Dead:
                    sheriff.StateMachine.ChangeState(new DropDeadSheriff());
                    return true;
                case MessageType.Respawn:
                    sheriff.StateMachine.ChangeState(new PatrolRandomLocation());
                    return true;
                default:
                    return false;
            }
        }
    }
}
