#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Artificial.XNATutorial.CarGame
{
    /// <summary>
    /// XNA TUTORIAL - CAR GAME PART 6
    /// 
    /// We add an editor for levels instead of using randomly placing houses.
    /// </summary>
    public class CarGame : Microsoft.Xna.Framework.Game
    {
        // Random
        Random random = new Random();

        // Graphics objects
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        PrimitiveBatch primitiveBatch;

        // Camera
        Camera sceneCamera;
        Camera debugCamera;
        bool useDebugCamera = false;

        // Scene
        Scene<object> scene = new Scene<object>();
        List<Building> buildings = new List<Building>();
        List<Car> cars = new List<Car>();

        // Scene renderers
        MockupRenderer mockupRenderer;
        ABTRenderer abtRenderer1;
        ABTRenderer abtRenderer2;

        // Input
        KeyboardState oldKeyState;
        GamePadState oldPadState;

        // Debug output
        DebugOutput debug;

        // GAME CONSTRUCTOR
        public CarGame()
        {
            // Enable Antialliasing and make rendering happen as often as possible           
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            // Create graphics objects
            device = graphics.GraphicsDevice;

            spriteBatch = new SpriteBatch(device);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);

            primitiveBatch = new PrimitiveBatch(device);
            this.Services.AddService(typeof(PrimitiveBatch), primitiveBatch);

            content = new ContentManager(Services);
            this.Services.AddService(typeof(ContentManager), content);

            // Set game object variables
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Car Game Part 6";
            this.Window.AllowUserResizing = true;

            // Create cameras
            sceneCamera = new Camera();
            sceneCamera.FieldOfView = (float)Math.PI / 2.5f;
            sceneCamera.NearPlane = 10f;
            sceneCamera.FarPlane = 500f;
            sceneCamera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            sceneCamera.UpDirection = Vector3.Up;
            sceneCamera.Distance = 40;

            debugCamera = new Camera();
            debugCamera.FieldOfView = (float)Math.PI / 2.5f;
            debugCamera.NearPlane = 10f;
            debugCamera.FarPlane = 4000f;
            debugCamera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            debugCamera.UpDirection = Vector3.Backward;
            debugCamera.Distance = 300;
            debugCamera.Direction = new Vector3(0, 2, -1);
            debugCamera.Target = new Vector3(0, -50, 0);

            // Create renderers
            mockupRenderer = new MockupRenderer(this, scene);
            Components.Add(mockupRenderer);

            abtRenderer1 = new ABTRenderer(this, mockupRenderer.BuildingsABT);
            Components.Add(abtRenderer1);

            abtRenderer2 = new ABTRenderer(this, mockupRenderer.CarsABT);
            abtRenderer2.TreeTop = new Vector2(0.65f, 0);
            Components.Add(abtRenderer2);

            // Load level
            scene.Load(AppDomain.CurrentDomain.BaseDirectory + "test.level");

            // Create player car
            Car c = new Car();
            cars.Add(c);
            scene.Add(c);

            // Player car is standing still
            cars[0].Speed = 0;

            // Follow the player car
            sceneCamera.TargetThing = cars[0];

            // Create and add debug output component
            debug = new DebugOutput(this);
            Components.Add(debug);

            // Initialize components
            base.Initialize();
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            // Exit on back/escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Get elapsed time in seconds
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle input from keyboard and gamepad
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // Quick hack for player car movement
            float rotationspeed = 2f;
            float speedchange = 20f;
            if (keyState.IsKeyDown(Keys.Left))
            {
                cars[0].Rotation += elapsed * rotationspeed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                cars[0].Rotation -= elapsed * rotationspeed;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                cars[0].Speed += elapsed * speedchange;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                cars[0].Speed -= elapsed * speedchange;
            }

            if (keyState.IsKeyDown(Keys.C) && oldKeyState.IsKeyUp(Keys.C))
            {
                useDebugCamera = !useDebugCamera;
            }

            cars[0].Rotation -= padState.ThumbSticks.Left.X * elapsed * rotationspeed;
            cars[0].Speed += padState.Triggers.Right * elapsed * speedchange;
            cars[0].Speed -= padState.Triggers.Left * elapsed * speedchange;

            Vector3 d = debugCamera.Direction;
            Vector3 r = Matrix.Invert(debugCamera.ViewMatrix).Right;
            Vector3 u = Matrix.Invert(debugCamera.ViewMatrix).Up;
            d -= (r * padState.ThumbSticks.Right.X + u * padState.ThumbSticks.Right.Y) * 0.01f;
            d.Normalize();
            debugCamera.Direction = d;

            // Save input for next frame
            oldKeyState = keyState;
            oldPadState = padState;

            // Update all cars
            foreach (Car c in cars)
            {
                c.Update(gameTime);
            }


            // Adapt camera height to player car speed
            sceneCamera.Distance = 40 + (float)Math.Pow(Math.Abs(cars[0].Speed), 1.2f) * 0.2f;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            // Enable rendering transparent textures
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            device.RenderState.CullMode = CullMode.None;

            // Render using mockup graphics
            if (useDebugCamera)
            {
                mockupRenderer.Render(debugCamera, sceneCamera);
            }
            else
            {
                mockupRenderer.Render(sceneCamera);
            }

#if DEBUG
            // Write debug information
            debug.Write("FPS: " + (int)(1.0f / gameTime.ElapsedGameTime.TotalSeconds), Color.White);
            debug.Write("Meshes drawn: " + Globals.MeshesRendered, Color.White);
            debug.Write("Sprites drawn: " + Globals.SpritesRendered, Color.White);
            debug.Write("ABT items repositioned: " + Globals.ABTNodesRepositioned, Color.Yellow);
            Globals.MeshesRendered = 0;
            Globals.SpritesRendered = 0;
            Globals.ABTNodesRepositioned = 0;

            // Render ABT
            if (useDebugCamera)
            {
                abtRenderer1.Render(debugCamera, sceneCamera);
                abtRenderer2.Render(debugCamera, sceneCamera);
            }
            else
            {
                abtRenderer1.Render(sceneCamera);
                abtRenderer2.Render(sceneCamera);
            }
#endif

            base.Draw(gameTime);
        }
    }
}