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
    public partial class Gameplay : Part
    {
        // GLOBALS

        // Constants
        public static float GridSize = 10;
        public static float StartSpeed = 20;
        public static float BombFuseTime = 4;
        public static float ObstacleDensity = 0.7f;
        public static float totalTime = 60.999f;

        // Variables
        public static Level Level;
        public static float timeLeft;
        public static bool GameEnded;


        // Gameplay component
        int updateOrder;

        // Renderer
        WorldRenderer renderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;

        // CONSTRUCTOR
        public Gameplay(int updateOrder)
        {
            this.updateOrder = updateOrder;
            Require<State>().NextState = Bomberman.States.MainMenu;
        }


        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            // Initialize variables
            GameEnded = false;
            timeLeft = totalTime;

            // Create scene
            Level = LevelGenerator.Generate(9);
            Level.AddCharacter(new Character(PlayerIndex.One, CharacterType.Alien));
            Level.AddCharacter(new Character(PlayerIndex.Two, CharacterType.Pumpkin));

            // Prepare simulation engine
            simulator = new Simulator();
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
            renderer = new WorldRenderer(Bomberman.Game);
            renderer.DrawOrder = 0;
            Bomberman.Game.Components.Add(renderer);

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
            //Bomberman.Game.Components.Add(physicsRenderer);
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            Bomberman.Game.Components.Remove(renderer);
        }

        // GLOBAL RULES
        void Update(float dt)
        {
            // Simulate game
            simulator.Simulate(dt);

            // Decrease game times
            if (timeLeft > 0 && !GameEnded)
            {
                timeLeft -= dt;
            }

            if (Input.WasKeyPressed(Keys.Escape))
            {
                Part<State>().Finished = true;
            }

            if (GameEnded &&  (Input.WasKeyPressed(Keys.Enter) || Input.WasKeyPressed(Keys.Space)))
            {
                Part<State>().NextState = Bomberman.States.Gameplay;
                Part<State>().Finished = true;
            }
        }
    }
}
