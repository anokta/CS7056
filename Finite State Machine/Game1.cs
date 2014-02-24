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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Miner Bob;
        MinersWife Elsa;
        Outlaw Jesse;
        Sheriff Wyatt;
        Undertaker Ripp;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        private Texture2D background;
        private Texture2D shuttle;
        private Texture2D earth;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(50);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Here's a little hack: The Miner and MinersWife must know each other's id in
            // order to communicate.  We calculate them inside each agent based on their
            // creation order, so the pair must always be created in this sequence.
            Bob = new Miner();
            Elsa = new MinersWife();
            Jesse = new Outlaw();
            Wyatt = new Sheriff();
            Ripp = new Undertaker();

            AgentManager.AddAgent(Bob);
            AgentManager.AddAgent(Elsa);
            AgentManager.AddAgent(Jesse);
            AgentManager.AddAgent(Wyatt);
            AgentManager.AddAgent(Ripp);

            // TODO: We could add more agents here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Arial");

            background = Content.Load<Texture2D>("Sprites/stars"); // change these names to the names of your images
            shuttle = Content.Load<Texture2D>("Sprites/shuttle");  // if you are using your own images.
            earth = Content.Load<Texture2D>("Sprites/earth");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        int countt = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                return;
            countt = (countt + 1) % 40;
            if (countt != 1) return;

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
          
            Message.gameTime = gameTime;
            Bob.Update();
            Elsa.Update();
            Jesse.Update();
            Wyatt.Update();
            Ripp.Update();
            Message.SendDelayedMessages();
            base.Update(gameTime);

            Printer.PrintMessageData("\n"); // Console.WriteLine("\n");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);

            spriteBatch.End();

            Printer.Draw(spriteBatch, spriteFont);


            base.Draw(gameTime);
        }
    }
}