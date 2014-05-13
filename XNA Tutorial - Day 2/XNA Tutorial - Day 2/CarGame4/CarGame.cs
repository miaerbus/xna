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
    /// XNA TUTORIAL - CAR GAME PART 4
    /// 
    /// We optimize rendering by using an adaptive binary tree (ABT)
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

        // Camera
        Camera sceneCamera;

        // Scene        
        Scene<object> scene = new Scene<object>(); // Now we use a specialized scene object, which generates
                                                   // events when new objects are inserted so the external
                                                   // scene representations, like the ABTs, can update accordingly

        List<Building> buildings = new List<Building>();
        List<Car> cars = new List<Car>();

        // Scene renderers
        MockupRenderer mockupRenderer;

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
            content = new ContentManager(Services);
            this.Services.AddService(typeof(ContentManager), content);

            // Set game object variables
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Car Game Part 4";
            this.Window.AllowUserResizing = true;

            // Create camera
            sceneCamera = new Camera();
            sceneCamera.FieldOfView = (float)Math.PI / 2.5f;
            sceneCamera.NearPlane = 10f;
            sceneCamera.FarPlane = 1000f;
            sceneCamera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            sceneCamera.UpDirection = Vector3.Up;
            sceneCamera.Distance = 40;

            // Create renderers
            mockupRenderer = new MockupRenderer(this, scene);
            Components.Add(mockupRenderer);

            // Create buildings
            int numBuildings = 200;
            int buildingsGrid = 10;
            for (int i = 0; i < numBuildings; i++)
            {
                Building b = new Building();
                b.Position = new Vector3(random.Next(-buildingsGrid, buildingsGrid) * 20 + 10, random.Next(-buildingsGrid, buildingsGrid) * 20 + 10, 0);
                b.Rotation = (float)Math.PI / 2f * random.Next(0, 4);
                scene.Add(b);
            }

            // Create cars
            int numCars = 200;
            int carsGrid = 10;
            for (int i = 0; i < numCars; i++)
            {
                Car c = new Car();
                c.Position = new Vector3(random.Next(-carsGrid, carsGrid) * 20, random.Next(-carsGrid, carsGrid) * 20, 0);
                c.Rotation = (float)Math.PI / 2f * random.Next(0, 4);
                c.Speed = (float)random.NextDouble() * 10 + 5;
                cars.Add(c);
                scene.Add(c);
            }
            
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
            cars[0].Rotation -= padState.ThumbSticks.Left.X * elapsed * rotationspeed;
            cars[0].Speed += padState.Triggers.Right * elapsed * speedchange;
            cars[0].Speed -= padState.Triggers.Left * elapsed * speedchange;

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
            mockupRenderer.Render(sceneCamera);

#if DEBUG
            // Write debug information
            debug.Write("FPS: " + (int)(1.0f / gameTime.ElapsedGameTime.TotalSeconds), Color.White);
            debug.Write("Meshes drawn: " + Globals.MeshesRendered, Color.White);
            debug.Write("Sprites drawn: " + Globals.SpritesRendered, Color.White);
            // Write how many items had to be repositioned inside the ABT
            debug.Write("ABT items repositioned: " + Globals.ABTNodesRepositioned, Color.Yellow);
            Globals.MeshesRendered = 0;
            Globals.SpritesRendered = 0;
            Globals.ABTNodesRepositioned = 0;
#endif

            base.Draw(gameTime);
        }
    }
}