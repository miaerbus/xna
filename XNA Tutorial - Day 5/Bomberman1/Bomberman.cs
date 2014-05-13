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
        // Graphics
        GraphicsDeviceManager graphics;

        // Renderer
        WorldRenderer renderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;
      
        // CONSTRUCTOR
        public Bomberman()
        {
            // Graphics
            graphics = new GraphicsDeviceManager(this);

            // Set graphics and game behaviour
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;

            this.Window.Title = "Retro Bomberman";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            Debug = new DebugOutput(this);
            Components.Add(Debug);

            Device = graphics.GraphicsDevice;

            // Create scene
            CurrentLevel = LevelGenerator.Generate(9);
            CurrentLevel.AddCharacter(new Character(PlayerIndex.One, CharacterType.Alien));
            CurrentLevel.AddCharacter(new Character(PlayerIndex.Two, CharacterType.Pumpkin));

            // Prepare simulation engine
            simulator = new Simulator();
            simulator.Scene = CurrentLevel.Scene;
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new ClassicalMechanics(20));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleParticleCollisions(0));
            simulator.Aggregate(new ParticleConvexCollisions(1));
            simulator.Aggregate(new ConstantsApplier(40, 15));
            simulator.Aggregate(new Rules(100,100));
         
            // Create main renderer
            renderer = new WorldRenderer(this);            
            renderer.DrawOrder = 0;
            Components.Add(renderer);

            // Create debug renderers
            abtRenderer = new ABTRenderer(this, simulator.Part<SceneABT>().ABT);
            abtRenderer.DrawOrder = 1;
            abtRenderer.RenderVolumes = true;
            abtRenderer.Camera = CurrentLevel.Camera;
            //Components.Add(abtRenderer);

            physicsRenderer = new PhysicsRenderer(this, CurrentLevel.Scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = CurrentLevel.Camera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            //Components.Add(physicsRenderer);

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Update input
            Input.Update();
            if (Input.keyState.IsKeyDown(Keys.Escape)) Exit();

            // Simulate game
            simulator.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }
    }
}