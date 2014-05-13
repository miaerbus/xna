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
        Particle pendulum;
        
        // CONSTRUCTOR
        public PhysicsDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            // Set graphics and game behaviour
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(10);
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
            Particle p;
            PositionPoint mount;
            PositionConstraint pc;
            Spring s;

            // Gravitation
            world.Add(new ConstantAcceleration(Vector3.Down*9.81f));

            // Newton's cradle
            int numPendulums = 5;
            float pendulumRadius = 0.01f;
            float pendulumLength = 0.2f;
            for (int i = 0; i < numPendulums; i++)
            {
                p = new Particle();
                p.Mass = 0.1f;
                p.ParticleRadius = pendulumRadius;
                p.CoefficientOfRestitution = 1f;
                p.Position = Vector3.Right*0.1f + Vector3.Right * pendulumRadius * 2 * i;
                world.Add(p);
                mount = new PositionPoint(p.Position + Vector3.Up * pendulumLength);
                world.Add(mount);
                pc = new PositionConstraint();
                pc.Item1 = mount;
                pc.Item2 = p;
                pc.MinimumDistance = pendulumLength;
                pc.MaximumDistance = pendulumLength;
                world.Add(pc);
                pendulum = p;
            }

            // Newton's cradle with springs
            for (int i = 0; i < numPendulums; i++)
            {
                p = new Particle();
                p.Mass = 0.1f;
                p.ParticleRadius = pendulumRadius;
                p.CoefficientOfRestitution = 1f;
                p.Position = Vector3.Left * 0.1f + Vector3.Left * pendulumRadius * 2 * i;
                world.Add(p);
                mount = new PositionPoint(p.Position + Vector3.Up * pendulumLength);
                world.Add(mount);
                s = new Spring();
                s.Item1 = mount;
                s.Item2 = p;
                s.RelaxedDistance = pendulumLength;
                s.ForceConstant = 0.1f;
                world.Add(s);
                pendulum = p;
            }

            // Long pendulum
            int numParts = 10;
            float partRadius = 0.001f;
            float partLength = 0.02f;
            IPosition previous = new PositionPoint(Vector3.Up * numParts * partLength);
            for (int i = 0; i < numParts; i++)
            {
                p = new Particle();
                p.Mass = 0.1f;
                p.ParticleRadius = i<numParts-1 ? partRadius : pendulumRadius;
                p.CoefficientOfRestitution = 1f;
                p.Position = previous.Position + Vector3.Down * partLength;
                world.Add(p);
                pc = new PositionConstraint();
                pc.Item1 = previous;
                pc.Item2 = p;
                pc.MinimumDistance = partLength;
                pc.MaximumDistance = partLength;
                world.Add(pc);
                previous = p;
                pendulum = p;
            }



            // Set camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 1;
            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            camera.NearPlane = 0.01f;
            camera.FarPlane = 100;
            camera.FieldOfView = MathHelper.PiOver4;

            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Move camera
            float cameraSpeed = 0.5f;
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
                camera.Target += new Vector3(padState.ThumbSticks.Left * elapsed * cameraSpeed * camera.FieldOfView, 0);
            }
            if (Math.Abs(padState.Triggers.Right) > 0.001f) camera.FieldOfView -= camera.FieldOfView * padState.Triggers.Right * elapsed * zoomSpeed;
            if (Math.Abs(padState.Triggers.Left) > 0.001f) camera.FieldOfView += camera.FieldOfView * padState.Triggers.Left * elapsed * zoomSpeed;

            // Add forces
            float forcePower = 200f;
            if (keyState.IsKeyDown(Keys.Right))
            {
                pendulum.AccumulatedForce += Vector3.Right * elapsed * forcePower;
            }
            pendulum.AccumulatedForce += new Vector3(padState.ThumbSticks.Right * elapsed * forcePower, 0);
     

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