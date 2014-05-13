using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Bomberman : Microsoft.Xna.Framework.Game
    {
        // Instance
        public static Bomberman Game;

        // Graphics
        public GraphicsDeviceManager Graphics;

        // Simulator
        Simulator simulator;

        // States
        public enum States
        {
            None,
            Splash,
            MainMenu,
            Options,
            Gameplay,
            End
        }

        State currentState;
        States nextState;
        Splash splash;
        Gameplay gameplay;
        Item menu;
      
        // CONSTRUCTOR
        public Bomberman()
        {
            Game = this;

            // Graphics
            Graphics = new GraphicsDeviceManager(this);

            // Set graphics and game behaviour
            Graphics.PreferMultiSampling = true;
            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            Graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            this.Window.Title = "Retro Bomberman";
            this.IsFixedTimeStep = true;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            Debug = new DebugOutput(this);
            Components.Add(Debug);

            Device = Graphics.GraphicsDevice;

            // Prepare simulation engine
            simulator = new Simulator();
            simulator.Scene = new Scene();

            // Start in splash
            ActivateState(States.Splash);

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Update input
            Input.Update();

            // Simulate game
            simulator.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (currentState.Finished)
            {
                simulator.Remove(currentState.Parent);
                if (currentState.NextState != States.None) nextState = currentState.NextState;
                ActivateState(nextState);
            }

            base.Update(gameTime);
        }


        // ACTIVATE STATE
        void ActivateState(States state)
        {
            switch (state)
            {
                case States.Splash:
                    splash = new Splash(0);
                    simulator.Aggregate(splash);
                    currentState = splash.Part<State>();
                    nextState = States.MainMenu;
                    break;
                case States.MainMenu:
                    menu = new MainMenu(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.None;
                    break;
                case States.Options:
                    menu = new Options(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.None;
                    break;
                case States.Gameplay:
                    gameplay = new Gameplay(0);
                    simulator.Aggregate(gameplay);
                    currentState = gameplay.Part<State>();
                    nextState = States.Gameplay;
                    break;
                case States.End:
                    Exit();
                    break;
            }
        }
    }
}