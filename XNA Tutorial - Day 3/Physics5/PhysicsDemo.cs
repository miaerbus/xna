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
        RigidBody body;
       
        // CONSTRUCTOR
        public PhysicsDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            // Set graphics and game behaviour
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferMultiSampling = true;

            this.IsFixedTimeStep = false;
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
            RigidBody b;
            ConvexPolygon c;

            // Gravitation
            //world.Add(new ConstantAcceleration(Vector3.Down*9.81f));

            // Big rotational thing-a-dingy
            c = new ConvexPolygon();
            c.Points.Add(Vector3.Left - Vector3.Up * 0.1f);
            c.Points.Add(Vector3.Left + Vector3.Up * 0.1f);
            c.Points.Add(Vector3.Right + Vector3.Up * 0.1f);
            c.Points.Add(Vector3.Right - Vector3.Up * 0.1f);
            c.CalculatePlanes();
            b = new RigidBody();
            b.ConvexPolyhedron = c;
            b.BoundingSphere = new BoundingSphere(Vector3.Zero, 1.1f);
            b.Position = Vector3.Right*2+Vector3.Up*2;
            b.Mass = 50f;
            b.CoefficientOfRestitution = 0.7f;
            b.AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver4);
            b.AngularMass = Matrix.Identity * b.Mass / 12f;            
            b.AngularMomentum = Vector3.Transform(Vector3.Forward*10f, b.AngularMass);
            world.Add(b);
            mount = new PositionPoint(b.Position);
            pc = new PositionConstraint();
            pc.Item1 = mount;
            pc.Item2 = b;
            pc.MinimumDistance = 0;
            pc.MaximumDistance = 0;
            world.Add(pc);

            // Big triangular thing-a-ringy
            c = new ConvexPolygon();
            c.Points.Add(new Vector3(-0.1f, 0.1f, 0));
            c.Points.Add(new Vector3(0.1f, 0.1f, 0));
            c.Points.Add(Vector3.Down * 3f);
            c.CalculatePlanes();
            b = new RigidBody();
            b.ConvexPolyhedron = c;
            b.BoundingSphere = new BoundingSphere(Vector3.Zero, 3.1f);
            b.Position = Vector3.Up * 2 + Vector3.Left * 3;
            b.Mass = 100f;
            b.CoefficientOfRestitution = 0.7f;
            b.AngularPosition = Quaternion.Identity;
            b.AngularMass = Matrix.Identity * b.Mass / 12f;
            world.Add(b);
            mount = new PositionPoint(b.Position);
            pc = new PositionConstraint();
            pc.Item1 = mount;
            pc.Item2 = b;
            pc.MinimumDistance = 0;
            pc.MaximumDistance = 0;
            world.Add(pc);

            // Big movable polyhedron
            c = new ConvexPolygon();
            c.Points.Add(new Vector3(-0.5f, 0.5f, 0));
            c.Points.Add(new Vector3(0.7f, 0.7f, 0));
            c.Points.Add(new Vector3(0.9f, 0.2f, 0));
            c.Points.Add(new Vector3(0.3f, -0.5f, 0));
            c.Points.Add(new Vector3(-0.1f, -0.8f, 0));
            c.Points.Add(new Vector3(-0.3f, -0.3f, 0));
            c.CalculatePlanes();
            b = new RigidBody();
            b.ConvexPolyhedron = c;
            b.BoundingSphere = new BoundingSphere(Vector3.Zero, 1.1f);
            b.Mass = 1000f;
            b.CoefficientOfRestitution = 0.7f;
            b.AngularPosition = Quaternion.Identity;
            b.AngularMass = Matrix.Identity * b.Mass / 12f;
            body = b;
            world.Add(b);

            // Big controllable polyhedron
            c = new ConvexPolygon();
            c.Points.Add(Vector3.Left - Vector3.Up);
            c.Points.Add(Vector3.Left + Vector3.Up);
            c.Points.Add(Vector3.Right + Vector3.Up);
            c.Points.Add(Vector3.Right - Vector3.Up);
            c.CalculatePlanes();
            b = new RigidBody();
            b.ConvexPolyhedron = c;
            b.BoundingSphere = new BoundingSphere(Vector3.Zero, 1.6f);
            b.Position = Vector3.Down * 2 + Vector3.Left * 0.1f;
            b.Mass = 500f;
            b.CoefficientOfRestitution = 0.7f;
            b.AngularPosition = Quaternion.Identity + Quaternion.CreateFromAxisAngle(Vector3.Forward, 0.1f);
            b.AngularMass = Matrix.Identity * b.Mass / 12f;
            body = b;
            world.Add(b);

            // Little people
            for (int i = 0; i < 100; i++)
            {
                p = new Particle();
                Vector3 direction = new Vector3(((float)random.NextDouble() * 2f - 1f), ((float)random.NextDouble() * 2f - 1f), 0);
                direction.Normalize();
                p.Position = direction * random.Next(5, 10);
                p.Mass = random.Next(500, 1500);
                p.ParticleRadius = p.Mass * 0.00005f;
                p.CoefficientOfRestitution = 0.9f;
                world.Add(p);
            }

            p = new Particle();
            p.Position = new Vector3(2.5f, 2.5f, 0f);
            p.Mass = 50f;
            p.ParticleRadius = 0.2f;
            p.CoefficientOfRestitution = 0.0f;
            world.Add(p);


            // Set camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 10;
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
            float cameraSpeed = 2f;
            float zoomSpeed = 1f;
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
            float forcePower = 50000f;
            if (keyState.IsKeyDown(Keys.Left))
            {
                body.AccumulatedForce += Vector3.Left * elapsed * forcePower;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                body.AccumulatedForce += Vector3.Right * elapsed * forcePower;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                body.AccumulatedForce += Vector3.Up * elapsed * forcePower;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                body.AccumulatedForce += Vector3.Down * elapsed * forcePower;
            }
            body.AccumulatedForce += new Vector3(padState.ThumbSticks.Right * elapsed * forcePower, 0);

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