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
    abstract public class Agent
    {
        private static int agents = 0;
        public static int AgentsCount
        {
            get { return agents;  }
        }

        // Every agent has a numerical id that is set when it is created
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private bool isDead;
        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        // The agent keeps track of its own location
        private Vector2 position;
        public Vector2 CurrentPosition
        {
            get { return position; }
            set { position = value; }
        }
        public Location Location
        {
            get { return LocationProperties.GetLocation(position); }
            set { position = LocationProperties.LocationCoords[(int)value]; }
        }
        

        // It also knows how much gold it's carrying
        protected int goldCarrying;
        public int GoldCarrying
        {
            get { return goldCarrying; }
            set { goldCarrying = value; }
        }

        //private Texture2D sprite;
        //public Texture2D Sprite
        //{
        //    get { return sprite; }
        //    set { sprite = value; }
        //}

        public Agent()//Texture2D sprite)
        {
            id = agents++;
           // this.sprite = sprite;
        }

        // Any agent must implement these methods
        abstract public void Update();
        abstract public bool HandleMessage(Telegram telegram);
    }
}
