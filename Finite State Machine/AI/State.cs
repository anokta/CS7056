using System;
using System.Collections.Generic;
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
    abstract public class State<T>
    {
        // This will be executed when the state is entered
        abstract public void Enter(T agent);

        // This is called by the Agent's update function each update step
        abstract public void Execute(T agent);

        // This will be executed when the state is exited
        abstract public void Exit(T agent);

        // This will be executed when the agent receives a message
        abstract public bool OnMesssage(T agent, Telegram telegram);
    }
}
