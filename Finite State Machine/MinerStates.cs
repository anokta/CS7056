using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteStateMachine
{
    // This class implements the state in which the Miner agent mines for gold
    public class EnterMineAndDigForNugget : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Printer.Print(miner.Id, "Walkin' to the goldmine");
            miner.Location = Location.goldMine;
        }

        public override void Execute(Miner miner)
        {
            miner.GoldCarrying += 1;
            miner.HowFatigued += 1;
            Printer.Print(miner.Id, "Pickin' up a nugget");
            if (miner.PocketsFull())
            {
                miner.StateMachine.ChangeState(new VisitBankAndDepositGold());
            }
            if (miner.Thirsty())
            {
                miner.StateMachine.ChangeState(new QuenchThirst());
            }
        }

        public override void Exit(Miner miner)
        {
            Printer.Print(miner.Id, "Ah'm leaving the gold mine with mah pockets full o' sweet gold");
        }

        public override bool OnMesssage(Miner agent, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the miner goes to the bank and deposits gold
    public class VisitBankAndDepositGold : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Printer.Print(miner.Id, "Goin' to the bank. Yes siree");
            miner.Location = Location.bank;
        }

        public override void Execute(Miner miner)
        {
            miner.MoneyInBank += miner.GoldCarrying;
            miner.GoldCarrying = 0;
            Printer.Print(miner.Id, "Depositing gold. Total savings now: " + miner.MoneyInBank);
            if (miner.Rich())
            {
                Printer.Print(miner.Id, "WooHoo! Rich enough for now. Back home to mah li'lle lady");
                miner.StateMachine.ChangeState(new GoHomeAndSleepTillRested());
            }
            else
            {
                miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
            }
        }

        public override void Exit(Miner miner)
        {
            Printer.Print(miner.Id, "Leavin' the Bank");
        }

        public override bool OnMesssage(Miner agent, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the miner goes home and sleeps
    public class GoHomeAndSleepTillRested : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Printer.Print(miner.Id, "Walkin' Home");
            miner.Location = Location.shack;
            Message.DispatchMessage(0, miner.Id, miner.WifeId, MessageType.HiHoneyImHome);
        }

        public override void Execute(Miner miner)
        {
            if (miner.HowFatigued < miner.TirednessThreshold)
            {
                Printer.Print(miner.Id, "All mah fatigue has drained away. Time to find more gold!");
                miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
            }
            else
            {
                miner.HowFatigued--;
                Printer.Print(miner.Id, "ZZZZZ....");
            }
        }

        public override void Exit(Miner miner)
        {

        }

        public override bool OnMesssage(Miner miner, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.HiHoneyImHome:
                    return false;
                case MessageType.StewsReady:
                    Printer.PrintMessageData("Message handled by " + miner.Id + " at time ");
                    Printer.Print(miner.Id, "Okay Hun, ahm a comin'!");
                    miner.StateMachine.ChangeState(new EatStew());
                    return true;
                default:
                    return false;
            }
        }
    }

    // In this state, the miner goes to the saloon to drink
    public class QuenchThirst : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            if (miner.Location != Location.saloon)
            {
                Printer.Print(miner.Id, "Boy, ah sure is thusty! Walking to the saloon");
                miner.Location = Location.saloon;
            }
        }

        public override void Execute(Miner miner)
        {
            // Buying whiskey costs 2 gold but quenches thirst altogether
            miner.HowThirsty = 0;
            miner.MoneyInBank -= 2;
            Printer.Print(miner.Id, "That's mighty fine sippin' liquer");
            miner.StateMachine.ChangeState(new EnterMineAndDigForNugget());
        }

        public override void Exit(Miner miner)
        {
            Printer.Print(miner.Id, "Leaving the saloon, feelin' good");
        }

        public override bool OnMesssage(Miner agent, Telegram telegram)
        {
            return false;
        }
    }

    // In this state, the miner eats the food that Elsa has prepared
    public class EatStew : State<Miner>
    {
        public override void Enter(Miner miner)
        {
            Printer.Print(miner.Id, "Smells Reaaal goood Elsa!");
        }

        public override void Execute(Miner miner)
        {
            Printer.Print(miner.Id, "Tastes real good too!");
            miner.StateMachine.RevertToPreviousState();
        }

        public override void Exit(Miner miner)
        {
            Printer.Print(miner.Id, "Thankya li'lle lady. Ah better get back to whatever ah wuz doin'");
        }

        public override bool OnMesssage(Miner agent, Telegram telegram)
        {
            return false;
        }
    }

    // If the agent has a global state, then it is executed every Update() cycle
    public class MinerGlobalState : State<Miner>
    {
        public override void Enter(Miner miner)
        {

        }

        public override void Execute(Miner miner)
        {
        }

        public override void Exit(Miner miner)
        {

        }

        public override bool OnMesssage(Miner agent, Telegram telegram)
        {
            switch (telegram.messageType)
            {
                case MessageType.SheriffEncountered:
                    //Printer.PrintMessageData("Message handled by " + agent.Id + " at time ");
                    Printer.Print(agent.Id, "Good day to you too, sheriff!");
                    return true;
                default:
                    return false;
            }
        }
    }
}
