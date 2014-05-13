using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Gameplay : Part
    {
        // GLOBALS

        // Variables
        public Level Level;
        public static float TimeLeft;
        public static bool GameEnded;


        // Gameplay component
        int updateOrder;

        // Renderer
        WorldRenderer renderer;
        ScoreRenderer scoreRenderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;

        bool offline;

        // CONSTRUCTOR
        public Gameplay(int updateOrder, bool offline)
        {
            this.updateOrder = updateOrder;
            this.offline = offline;
            Require<State>();
        }


        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            // Initialize variables
            GameEnded = false;
            TimeLeft = Bomberman.TotalTime;

            // Create scene
            Level = LevelGenerator.Generate(9, Bomberman.ActivePlayers.Count);
            Bomberman.Level = Level;

            for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
            {
                Bomberman.ActivePlayers[i].Controller.ActivateGameControls();
                Bomberman.ActivePlayers[i].Character.Reset();
                Bomberman.ActivePlayers[i].Character.BombCount = 3;
                Bomberman.ActivePlayers[i].Character.ExplosionPower = 3;
                Bomberman.ActivePlayers[i].Character.Part<PPosition>().Position = Level.Start[i];
                Bomberman.ActivePlayers[i].Character.Part<PAngularPosition>().AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2);
                Level.AddCharacter(Bomberman.ActivePlayers[i].Character);
            }

            // Prepare simulation engine
            simulator = new Simulator(4);
            simulator.Scene = Level.Scene;
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new ClassicalMechanics(20));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleParticleCollisions(0));
            simulator.Aggregate(new ParticleConvexCollisions(1));
            simulator.Aggregate(new ConstantsApplier(40, 15));
            simulator.Aggregate(new Rules(100, 100));

            // Create main renderer
            renderer = Bomberman.WorldRenderer;
            renderer.level = Level;
            renderer.DrawOrder = 0;
            Bomberman.Game.Components.Add(renderer);

            scoreRenderer = new ScoreRenderer(Bomberman.Game, Level);
            scoreRenderer.DrawOrder = 100;
            Bomberman.Game.Components.Add(scoreRenderer);

            // Create debug renderers
            abtRenderer = new ABTRenderer(Bomberman.Game, simulator.Part<SceneABT>().ABT);
            abtRenderer.DrawOrder = 1;
            abtRenderer.RenderVolumes = true;
            abtRenderer.Camera = Level.Camera;
            //Bomberman.Game.Components.Add(abtRenderer);

            physicsRenderer = new PhysicsRenderer(Bomberman.Game, Level.Scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = Level.Camera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            physicsRenderer.renderBoundingSphere = true;
            //Bomberman.Game.Components.Add(physicsRenderer);
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            Bomberman.Game.Components.Remove(renderer);
            Bomberman.Game.Components.Remove(scoreRenderer);
        }

        // GLOBAL RULES
        void Update(float dt)
        {
            for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
            {
                Bomberman.ActivePlayers[i].Controller.Update(dt);
            }

            // Simulate game
            simulator.Simulate(dt);

            // Decrease game time
            if (TimeLeft > 0 && !GameEnded)
            {
                TimeLeft -= dt;
            }

            if (GlobalInput.WasKeyPressed(Keys.Escape) || GlobalInput.gamePadState[0].Buttons.Back == ButtonState.Pressed)
            {
                Part<State>().Finished = true;
            }

            if (GameEnded && (GlobalInput.WasKeyPressed(Keys.Enter) || GlobalInput.WasKeyPressed(Keys.Space) || GlobalInput.gamePadState[0].Buttons.Start==ButtonState.Pressed))
            {
                if (offline)
                {
                    Part<State>().NextState = Bomberman.States.OfflineGameplay;
                }
                else
                {
#if WINDOWS
                    Part<State>().NextState = Bomberman.States.OnlineGameplay;
#endif
                }
                Part<State>().Finished = true;
            }
        }
    }
}
