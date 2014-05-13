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

        // House
        Object3D house;
        
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
            house = new Object3D();
            house.Model = content.Load<Model>(@"content\3dhouse");
            house.NormalMatrix = Matrix.CreateScale(0.015f) * Matrix.CreateRotationZ(MathHelper.PiOver4) * Matrix.CreateTranslation(Vector3.Forward*0.5f);

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

            c = new ConvexPolygon();
            c.Points.Add(Vector3.Left);
            c.Points.Add(Vector3.Up);
            c.Points.Add(Vector3.Right);
            c.Points.Add(Vector3.Down);
            c.CalculatePlanes();
            b = new RigidBody();
            b.ConvexPolyhedron = c;
            b.BoundingSphere = new BoundingSphere(Vector3.Zero, 1);
            b.Position = Vector3.Right;
            b.Mass = 1f;
            b.AngularPosition = Quaternion.Identity;
            b.AngularMass = Matrix.Identity * b.Mass / 12f;
            //b.AngularMomentum = Vector3.Transform(Vector3.Forward * 2, b.AngularMass);
            body = b;
            world.Add(b);

            // Set camera
            camera = new Camera();
            camera.Direction = Vector3.Forward;
            camera.Distance = 5;
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

            // Add torque
            float torquePower = 10f;
            if (keyState.IsKeyDown(Keys.Right))
            {
                body.AccumulatedTorque += Vector3.Forward * elapsed * torquePower;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                body.AccumulatedTorque += Vector3.Backward * elapsed * torquePower;
            }
            body.AccumulatedTorque += new Vector3(-padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X, 0) * elapsed * torquePower;
     

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

            graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            graphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            house.Render(body.Rotation * Matrix.CreateTranslation(body.Position), camera, camera);

            renderer.Render(camera);
            //abtRenderer.Render(camera);


            base.Draw(gameTime);
        }
    }
}