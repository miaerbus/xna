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

namespace Artificial.XNATutorial.Breakout
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Breakout : Microsoft.Xna.Framework.Game
    {
        // Graphics
        GraphicsDeviceManager graphics;

        // Renderer
        BreakoutRenderer renderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;

        // Scene
        Camera camera;
        public static Scene Scene;
        public static Paddle Paddle;
        public static List<Ball> Balls = new List<Ball>();
      
        // CONSTRUCTOR
        public Breakout()
        {
            // Graphics
            graphics = new GraphicsDeviceManager(this);

            // Set graphics and game behaviour
            graphics.PreferMultiSampling = true;
            this.Window.Title = "Retro Arkanoid";
        }


        // INITIALIZE
        protected override void Initialize()
        {
            // Create scene
            Scene = new Scene();

            // Create camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 300;
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            camera.NearPlane = 1f;
            camera.FarPlane = 1000;
            camera.FieldOfView = MathHelper.PiOver2;

            // Prepare simulation engine
            simulator = new Simulator(Scene);
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new ClassicalMechanics(20));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleConvexCollisions(0));
            BreakoutRules rules = new BreakoutRules(100, 100);
            simulator.Aggregate(rules);

            // Create main renderer
            renderer = new BreakoutRenderer(this, Scene, camera);
            renderer.DrawOrder = 0;
            Components.Add(renderer);

            // Create debug renderers
            abtRenderer = new ABTRenderer(this, simulator.As<SceneABT>().ABT);
            abtRenderer.DrawOrder = 1;
            abtRenderer.RenderVolumes = true;
            abtRenderer.Camera = camera;
            abtRenderer.Enabled = false;
            abtRenderer.Visible = false;
            Components.Add(abtRenderer);

            physicsRenderer = new PhysicsRenderer(this, Scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = camera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            physicsRenderer.Enabled = false;
            physicsRenderer.Visible = false;
            Components.Add(physicsRenderer);

            // Prepare scene
            rules.Reset();

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Update input
            Input.Update();
            if (Input.keyState.IsKeyDown(Keys.Escape)) Exit();

            // Simulate game
            simulator.Simulate(gameTime);

            base.Update(gameTime);
        }
    }
}