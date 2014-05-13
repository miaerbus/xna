using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PhysicsDemo : Microsoft.Xna.Framework.Game
    {
        // Graphics
        GraphicsDeviceManager graphics;
        ContentManager content;

        // Renderer
        PhysicsRenderer renderer;
        ABTRenderer abtRenderer;

        // Simulator
        PhysicsSimulator simulator;

        // Scene
        Scene world;
        Camera camera;
        
        // CONSTRUCTOR
        public PhysicsDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            // Set graphics and game behaviour
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferMultiSampling = true;

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;           
        }


        // INITIALIZE
        protected override void Initialize()
        {
            // Create scene and engines
            world = new Scene();
            simulator = new PhysicsSimulator(world);
            renderer = new PhysicsRenderer(this, world, simulator);
            Components.Add(renderer);

            abtRenderer = new ABTRenderer(this, simulator.SceneABT);
            abtRenderer.RenderVolumes = true;
            Components.Add(abtRenderer);

            // Create scene
            Random random = new Random();
            int numParticles = 100;
            float bounds = 500;
            float speed = 10;
            Particle p;

            // Big planet
            p = new Particle();
            p.Mass = 10E15f;
            p.ParticleRadius = 100f;
            p.CoefficientOfRestitution = 0.7f;
            world.Add(p);

            // Asteroid
            p = new Particle();
            p.Position = Vector3.Up * 30000f;
            p.Velocity = Vector3.Down * 1000f;
            p.Mass = 50E12f;
            p.ParticleRadius = 30f;
            p.CoefficientOfRestitution = 0.0f;
            world.Add(p);

            // Little people
            for (int i = 0; i < numParticles; i++)
            {
                p = new Particle();
                Vector3 direction = new Vector3(((float)random.NextDouble() * 2f - 1f), ((float)random.NextDouble() * 2f - 1f), 0);
                direction.Normalize();
                p.Position = direction * random.Next(200, (int)bounds);
                p.Velocity = new Vector3(((float)random.NextDouble() * 2f - 1f) * speed, ((float)random.NextDouble() * 2f - 1f) * speed, 0);
                p.Mass = random.Next(500, 1500);
                p.ParticleRadius = p.Mass * 0.005f;
                p.CoefficientOfRestitution = 0.9f;
                world.Add(p);
            }            

            // Set camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 10000;
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            camera.NearPlane = 100;
            camera.FarPlane = 100000;
            camera.FieldOfView = (float)Math.PI/32f;

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Move camera
            float cameraSpeed = 4000;
            float zoomSpeed = 1;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.A))
            {
                camera.Target += Vector3.Left * elapsed * cameraSpeed * camera.FieldOfView;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                camera.Target += Vector3.Right * elapsed * cameraSpeed * camera.FieldOfView;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                camera.Target += Vector3.Up * elapsed * cameraSpeed * camera.FieldOfView;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                camera.Target += Vector3.Down * elapsed * cameraSpeed * camera.FieldOfView;
            }
            if (keyState.IsKeyDown(Keys.R))
            {
                camera.FieldOfView += camera.FieldOfView * elapsed * zoomSpeed;
            }
            if (keyState.IsKeyDown(Keys.F))
            {
                camera.FieldOfView -= camera.FieldOfView * elapsed * zoomSpeed;
            }
            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            if (Math.Abs(padState.ThumbSticks.Left.X) > 0.001f || Math.Abs(padState.ThumbSticks.Left.Y) > 0.001f)
            {
                camera.Target += new Vector3(padState.ThumbSticks.Left * new Vector2(1, 1) * elapsed * cameraSpeed * camera.FieldOfView, 0);
            }
            if (Math.Abs(padState.Triggers.Right) > 0.001f) camera.FieldOfView -= camera.FieldOfView * padState.Triggers.Right * elapsed * zoomSpeed;
            if (Math.Abs(padState.Triggers.Left) > 0.001f) camera.FieldOfView += camera.FieldOfView * padState.Triggers.Left * elapsed * zoomSpeed;

            // Simulate physics
            simulator.Simulate(gameTime);

            if (keyState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.DimGray);

            renderer.Render(camera);
            //abtRenderer.Render(camera);

            base.Draw(gameTime);
        }
    }
}