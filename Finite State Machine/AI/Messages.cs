using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace FiniteStateMachine
{
    // The types of messages that our agents can send to each other
    public enum MessageType
    {
        HiHoneyImHome,
        StewsReady,
        SheriffEncountered,
        Gunfight,
        Dead,
        Respawn
    }

    // Telegrams are the messages that are sent between agents -- don't create these yourself, just call DispatchMessage()
    public struct Telegram
    {
        public double DispatchTime;
        public int Sender;
        public int Receiver;
        public MessageType messageType;

        public Telegram(double dt, int s, int r, MessageType mt)
        {
            DispatchTime = dt;
            Sender = s;
            Receiver = r;
            messageType = mt;
        }
    }

    // This static class encapsulates all the message-related functions in our game
    public static class Message
    {
        public static List<Telegram> telegramQueue = new List<Telegram>();
        public static GameTime gameTime;

        // This message is used by agents to dispatch messages to other agents -- use this from your own agents
        public static void DispatchMessage(double delay, int sender, int receiver, MessageType messageType)
        {
            Agent sendingAgent = AgentManager.GetAgent(sender);
            Agent receivingAgent = AgentManager.GetAgent(receiver);

            Telegram telegram = new Telegram(0, sender, receiver, messageType);
            if (delay <= 0.0f)
            {
                Printer.PrintMessageData("Instant telegram dispatched by " + sender + " for " + receiver + " message is " + MessageToString(messageType));
                SendMessage(receivingAgent, telegram);
            }
            else
            {
                telegram.DispatchTime = (int)gameTime.TotalGameTime.Ticks + delay;
                telegramQueue.Add(telegram);
                Printer.PrintMessageData("Delayed telegram from " + sender + " recorded at time " + gameTime.TotalGameTime.Ticks);
            }
        }

        // This sends any messages that are due for delivery; invoked at each tick by the game's Update() method
        public static void SendDelayedMessages()
        {
            for (int i = 0; i < telegramQueue.Count; i++)
            {
                if (telegramQueue[i].DispatchTime <= gameTime.TotalGameTime.Ticks)
                {
                    Agent receivingAgent = AgentManager.GetAgent(telegramQueue[i].Receiver);
                    SendMessage(receivingAgent, telegramQueue[i]);
                    telegramQueue.RemoveAt(i);
                }
            }
        }

        // Attempt to send a message to a particular agent; called by the preceding two methods -- don't call this from your own agents
        public static void SendMessage(Agent agent, Telegram telegram)
        {
            if (!agent.HandleMessage(telegram))
            {
                Printer.PrintMessageData("Message not handled");
            }
        }

        // Converts a message to string format
        public static String MessageToString(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.HiHoneyImHome:
                    return "Hi Honey I'm Home";
                case MessageType.StewsReady:
                    return "Stew's Ready";
                case MessageType.SheriffEncountered:
                    return "Sheriff Encountered";
                case MessageType.Gunfight:
                    return "Gunfight";
                case MessageType.Dead:
                    return "Dead Notice";
                case MessageType.Respawn:
                    return "Respawn";
                default:
                    return "Not recognized";
            }
        }
    }
}