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

namespace Artificial.XNATutorial.Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Pong : Microsoft.Xna.Framework.Game
    {
        // Graphics
        GraphicsDeviceManager graphics;

        // Renderer
        PongRenderer renderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;

        // Scene
        Scene scene;
        Camera camera;
      
        // CONSTRUCTOR
        public Pong()
        {
            // Graphics
            graphics = new GraphicsDeviceManager(this);

            // Set graphics and game behaviour
            graphics.PreferMultiSampling = true;
            this.Window.Title = "Retro Pong";
        }


        // INITIALIZE
        protected override void Initialize()
        {
            // Create scene
            scene = new Scene();

            // Create camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 300;
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            camera.NearPlane = 1f;
            camera.FarPlane = 1000;
            camera.FieldOfView = MathHelper.PiOver2;

            // Prepare simulation engine
            simulator = new Simulator(scene);
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new ClassicalMechanics(20));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleConvexCollisions(0));
            simulator.Aggregate(new PongRules(100,100));

            // Create main renderer
            renderer = new PongRenderer(this, scene, camera);
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

            physicsRenderer = new PhysicsRenderer(this, scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = camera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            physicsRenderer.renderBoundingSphere = true;
            physicsRenderer.Enabled = false;
            physicsRenderer.Visible = false;
            Components.Add(physicsRenderer);

            // Prepare scene
            scene.Add(new Paddle(PlayerIndex.One));
            scene.Add(new Paddle(PlayerIndex.Two));
            scene.Add(new Ball());
            scene.Add(new CollisionPlane(new Plane(Vector3.Down, -250)));
            scene.Add(new CollisionPlane(new Plane(Vector3.Up, -250)));     

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