using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
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

        // Transitions
        TransitionRenderer transitionRenderer;
        bool inTransitionFrame = false;

        // States
        public enum States
        {
            None,
            Splash,
            LoadingMenu,
            LoadingWorld,
            PlayerSelection,
            OfflinePlay,
            OnlinePlay,
            MainMenu,
            Lobby,
            EditConnection,
            Options,
            OfflineGameplay,
            OnlineGameplay,
            End
        }

        State currentState;
        States thisState;
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
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(33.3);
        }

        void Load()
        {
#if WINDOWS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Bomberman\players.xml";
            if (File.Exists(path))
            {
                FileStream file = new FileStream(path, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                Bomberman.Players = serializer.Deserialize(file) as List<Player>;
                file.Close();

            }
            else
            {
                Bomberman.Players = new List<Player>();
            }
#else
            Bomberman.Players = new List<Player>();
            for (int i = 0; i < 4; i++)
            {
                Player player = new Player();
                player.Name = "PLAYER " + (i + 1).ToString();
                player.ControllerType = (ControlType)i + 2;
                Bomberman.Players.Add(player);
            }
#endif
            // Try to connect players
            for (int i = 0; i < Bomberman.Players.Count; i++)
            {
                if (Bomberman.Players[i].ControllerType != ControlType.None)
                {
                    int j = (int)Bomberman.Players[i].ControllerType;
                    if (Bomberman.Controllers[j] != null)
                    {
                        if (Bomberman.Controllers[j].Player == null)
                        {
                            Bomberman.Controllers[j].PlayerIndex = i;
                            Bomberman.Players[i].Controller = Bomberman.Controllers[j];
                            Bomberman.ActivePlayers.Add(Bomberman.Players[i]);
                        }
                    }
                }
            }
        }

        void Save()
        {
#if WINDOWS
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Bomberman";
            string path = directory + @"\players.xml";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(directory);
            }
            FileStream file = new FileStream(path, FileMode.Create);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
            serializer.Serialize(file, Bomberman.Players);
            file.Close();
#endif
        }

        void LoadRenderers()
        {
#if XBOX
            Thread.CurrentThread.SetProcessorAffinity(new int[] { 4 });
#endif
            Bomberman.MenuRenderer = new MenuRenderer(this, null);
            MenuRendererLoaded = true;
            Bomberman.WorldRenderer = new WorldRenderer(this, null);
            WorldRendererLoaded = true;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            Device = Graphics.GraphicsDevice;

            // Create renderers on seperate thread
            Thread loadingThread = new Thread(new ThreadStart(LoadRenderers));
            loadingThread.Start();

            Bomberman.LoadingRenderer = new LoadingRenderer(this);

            // Create controllers
#if WINDOWS
            Bomberman.Controllers[0] = new Controller(new Vector3(), ControlType.KeyboardLeft);
            Bomberman.Controllers[1] = new Controller(new Vector3(), ControlType.KeyboardRight);
#endif
            for (int i = 0; i < 4; i++)
            {
                if (GlobalInput.gamePadState[i].IsConnected)
                {
                    Bomberman.Controllers[i + 2] = new Controller(new Vector3(), (ControlType)(i + 2));
                }
            }

            Load();

#if DEBUG
            Debug = new DebugOutput(this);
            Components.Add(Debug);
            Components.Add(new FPSDisplay(this));
#endif


            // Create sound engine
            AudioEngine = new AudioEngine(@"Content\Sound\Bomberman.xgs");
            WaveBank = new WaveBank(AudioEngine, @"Content\Sound\Wave Bank.xwb");
            SoundBank = new SoundBank(AudioEngine, @"Content\Sound\Sound Bank.xsb");

            // Prepare simulation engine
            simulator = new Simulator();
            simulator.Scene = new Scene();

            transitionRenderer = new TransitionRenderer(this);
            Components.Add(transitionRenderer);

            // Start in splash
            ActivateState(States.Splash);

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            if (inTransitionFrame)
            {
                simulator.Remove(currentState.Parent);
                if (MenuRendererLoaded==false && thisState == States.Splash)
                {
                    ActivateState(States.LoadingMenu);
                }
                else if (WorldRendererLoaded == false && (nextState == States.Lobby || nextState == States.OfflineGameplay))
                {
                    ActivateState(States.LoadingWorld);
                }
                else
                {
                    if (currentState.NextState != States.None) nextState = currentState.NextState;
                    ActivateState(nextState);
                }

                inTransitionFrame = false;
            }
            
            // Update input
            GlobalInput.Update();

            // Simulate game
            simulator.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (currentState.Finished)
            {
                inTransitionFrame = true;
                transitionRenderer.Transition();
            }

            //if (GlobalInput.WasKeyPressed(Keys.Escape) || GlobalInput.gamePadState[0].Buttons.Back == ButtonState.Pressed)
            //{
            //    Exit();
            //}

            base.Update(gameTime);
        }


        // ACTIVATE STATE
        void ActivateState(States state)
        {
            thisState = state;
            switch (state)
            {
                case States.Splash:
                    splash = new Splash(0);
                    simulator.Aggregate(splash);
                    currentState = splash.Part<State>();
#if WINDOWS
                    if (Bomberman.ActivePlayers.Count > 0)
                    {
                        nextState = States.MainMenu;
                    }
                    else
                    {
                        nextState = States.PlayerSelection;
                    }
#else
                    nextState = States.MainMenu;
#endif
                    break;
                case States.LoadingMenu:
                    menu = new Loading(0,false);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    break;
                case States.LoadingWorld:
                    menu = new Loading(0,true);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    break;
                case States.PlayerSelection:
                    menu = new PlayerSelection(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.MainMenu;
                    break;
                case States.OfflinePlay:
                    menu = new CharacterSelection(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.OfflineGameplay;
                    break;
                case States.MainMenu:
                    menu = new MainMenu(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.None;
                    break;
#if WINDOWS
                case States.OnlinePlay:
                    menu = new CharacterSelection(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.Lobby;
                    break;
                case States.Lobby:
                    menu = new Lobby(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.None;
                    break;
                case States.OnlineGameplay:
                    gameplay = new Gameplay(0,false);
                    simulator.Aggregate(gameplay);
                    currentState = gameplay.Part<State>();
                    nextState = States.Lobby;
                    break;
#endif
                case States.Options:
                    menu = new Options(0);
                    simulator.Aggregate(menu);
                    currentState = menu.Part<State>();
                    nextState = States.None;
                    break;
                case States.OfflineGameplay:
                    gameplay = new Gameplay(0,true);
                    simulator.Aggregate(gameplay);
                    currentState = gameplay.Part<State>();
                    nextState = States.MainMenu;
                    break;
                case States.End:
                    Exit();
                    break;
            }
        }

        protected override void EndRun()
        {
            Save();
        }
    }
}