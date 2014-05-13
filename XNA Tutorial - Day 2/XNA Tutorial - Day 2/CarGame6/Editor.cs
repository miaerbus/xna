#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Artificial.XNATutorial.CarGame
{
    public class Editor : Microsoft.Xna.Framework.Game
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

        // Scene
        Scene<object> scene = new Scene<object>();
        Scene<object> proxyScene = new Scene<object>();
        List<Building> buildings = new List<Building>();

        // Scene renderers
        MockupRenderer mockupRenderer;
        MockupRenderer proxyMockupRenderer;

        // Input
        KeyboardState keyState;
        KeyboardState oldKeyState;
        GamePadState padState;
        GamePadState oldPadState;
        MouseState mouseState;
        MouseState oldMouseState;

        Point tile;
        Vector3 cursor;

        Vector3 cursorOrigin;
        Camera cameraOrigin = new Camera();

        // Editor
        delegate void InputModeHandler();
        InputModeHandler handleInput;
        
        object proxy;
        int proxyIndex = -1;
        int objectAddType;
        IPosition proxyPosition;
        bool showProxy;

        object selection;
        int selectionIndex = -1;
        IPosition selectionPosition;
        Vector3[] selectionBox;

        float gridSize = 10;
        bool snapToGrid;

        string levelFilename = "test.level";
        
        // Debug output
        DebugOutput debug;

        // GAME CONSTRUCTOR
        public Editor()
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
            this.Window.Title = "XNA Tutorial - Car Game Editor";
            this.Window.AllowUserResizing = true;

            // Create cameras
            sceneCamera = new Camera();
            sceneCamera.FieldOfView = (float)Math.PI / 2.5f;
            sceneCamera.NearPlane = 1f;
            sceneCamera.FarPlane = 10000f;
            sceneCamera.AspectRatio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            sceneCamera.UpDirection = Vector3.Backward;
            sceneCamera.Direction = new Vector3(0, 1, -1);
            sceneCamera.Distance = 200;

            // Create renderers
            mockupRenderer = new MockupRenderer(this, scene);
            Components.Add(mockupRenderer);

            proxyMockupRenderer = new MockupRenderer(this, proxyScene);
            Components.Add(proxyMockupRenderer);

            // Create and add debug output component
            debug = new DebugOutput(this);
            Components.Add(debug);

            // Setup editor
            handleInput = ObjectSelectionMode;
            selectionBox = BoundingBox.CreateFromSphere(new BoundingSphere(new Vector3(0, 0, gridSize / 2f), gridSize / 2f)).GetCorners();

            // Initialize components
            base.Initialize();
        }

        // INPUT HELPERS
        bool isKeyPressed(Keys key)
        {
            return (keyState.IsKeyDown(key) && oldKeyState.IsKeyUp(key));
        }

        bool isMouseLeftPressed()
        {
            return (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released);
        }

        bool isMouseRightPressed()
        {
            return (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released);
        }

        // MOUSE POSITION ON GROUND
        Vector3 mousePosition(Camera camera)
        {
            Vector3[] frustumBounds = camera.Frustum.GetCorners();
            Vector3 TopLeft = frustumBounds[0];
            Vector3 Right = frustumBounds[1] - frustumBounds[0];
            Vector3 Down = frustumBounds[3] - frustumBounds[0];
            float rightPart = (float)mouseState.X / (float)device.Viewport.Width;
            float downPart = (float)mouseState.Y / (float)device.Viewport.Height;
            Vector3 mouseOnNearPlane = TopLeft + Right * rightPart + Down * downPart;
            Vector3 cameraPosition = camera.Position;
            float distanceToGround = Vector3.Dot(Vector3.Backward, mouseOnNearPlane);
            Vector3 mouseDirection = mouseOnNearPlane - cameraPosition;
            mouseDirection.Normalize();
            float factor = Vector3.Dot(Vector3.Backward, mouseDirection);
            float distance = distanceToGround / factor;
            return mouseOnNearPlane - mouseDirection * distance;
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
            keyState = Keyboard.GetState();
            padState = GamePad.GetState(PlayerIndex.One);
            mouseState = Mouse.GetState();

            // Calculate the current tile we're pointing on with the mouse
            cursor = mousePosition(sceneCamera);
            tile = new Point((int)Math.Floor(cursor.X / gridSize), (int)Math.Floor(cursor.Y / gridSize));         
#if DEBUG
            debug.Write("MOUSE: " + mouseState.X + ", " + mouseState.Y, Color.White);
            debug.Write("MOUSE ON GROUND: " + cursor.X + ", " + cursor.Y + ", " + cursor.Z, Color.White);
            debug.Write("TILE: " + tile.X + ", " + tile.Y, Color.White);
#endif

            // Move camera
            if (isMouseRightPressed())
            {
                // Save camera position
                cameraOrigin.Target = sceneCamera.Target;
                cameraOrigin.FieldOfView = sceneCamera.FieldOfView;
                cameraOrigin.NearPlane = sceneCamera.NearPlane;
                cameraOrigin.FarPlane = sceneCamera.FarPlane;
                cameraOrigin.AspectRatio = sceneCamera.AspectRatio;
                cameraOrigin.UpDirection = sceneCamera.UpDirection;
                cameraOrigin.Direction = sceneCamera.Direction;
                cameraOrigin.Distance = sceneCamera.Distance;

                cursorOrigin = cursor;
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                Vector3 c = mousePosition(cameraOrigin);
                sceneCamera.Target = cameraOrigin.Target - (c - cursorOrigin);
            }

            // Select input mode
            if (isKeyPressed(Keys.D0))
            {
                // Object selection
                StartObjectSelectionMode();
            }
            if (isKeyPressed(Keys.D1))
            {
                // Add house
                objectAddType = 1;
                StartObjectAddMode();
            }

            // Snap to grid
            if (isKeyPressed(Keys.G))
            {
                snapToGrid = !snapToGrid;
            }

            // Delete item
            if (isKeyPressed(Keys.Delete))
            {
                if (selectionIndex > -1)
                {
                    scene.Remove(selectionIndex);
                    selectionIndex = -1;
                }
            }

            // Save scene
            if (isKeyPressed(Keys.S) && (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl)))
            {
                scene.Save(AppDomain.CurrentDomain.BaseDirectory + levelFilename);
                System.Console.Beep();
            }

            // Open scene
            if (isKeyPressed(Keys.O) && (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl)))
            {
                scene.Load(AppDomain.CurrentDomain.BaseDirectory + levelFilename);
                System.Console.Beep();
            }


            // Execute input handling
            handleInput();

            // Save input for next frame
            oldKeyState = keyState;
            oldPadState = padState;
            oldMouseState = mouseState;

            base.Update(gameTime);
        }

        // OBJECT SELECTION MODE
        void StartObjectSelectionMode()
        {
            handleInput = ObjectSelectionMode;
            showProxy = false;
        }

        void ObjectSelectionMode()
        {
            if (isMouseLeftPressed())
            {
                // find closest item
                float minLength = float.PositiveInfinity;
                int minIndex = -1;
                int[] indices = scene.GetItemIndices();
                for (int i=0; i<indices.Length;i++)
                {
                    IPosition item = (IPosition)scene[indices[i]];
                    float l = (item.Position - cursor).Length();
                    if (l < minLength)
                    {
                        minLength = l;
                        minIndex = indices[i];
                    }
                }
                if (minLength < gridSize)
                {
                    selectionIndex = minIndex;
                }
                else
                {
                    selectionIndex = -1;
                }
                if (selectionIndex > -1)
                {
                    selection = scene[selectionIndex];
                    selectionPosition = (IPosition)selection;
                }
            }
        }

        // OBJECT ADD MODE
        void StartObjectAddMode()
        {
            handleInput = ObjectAddMode;
            showProxy = true;
            selectionIndex = -1;

            // Remove previous proxy
            if (proxyIndex > -1)
            {
                proxyScene.Remove(proxyIndex);
            }

            switch (objectAddType)
            {
                case 1:
                    proxy = new Building();
                    proxyPosition = (IPosition)proxy;
                    proxyIndex = proxyScene.Add(proxy);
                    break;
            }        
        }

        void ObjectAddMode()
        {
            // Move proxy to cursor
            if (snapToGrid)
            {
                proxyPosition.Position = new Vector3(tile.X * gridSize + gridSize / 2f, tile.Y * gridSize + gridSize / 2f, 0);
            }
            else
            {
                proxyPosition.Position =cursor;
            }           

            // Add object
            if (isMouseLeftPressed())
            {
                scene.Add(proxy);
                StartObjectAddMode();
            }
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

            // Draw grid
            primitiveBatch.Begin(sceneCamera.ViewMatrix, sceneCamera.ProjectionMatrix);
            int w=10;
            for (float i = -w * gridSize; i <= w * gridSize; i += gridSize)
            {
                byte b = (byte)((float)(w - Math.Abs(i / gridSize)) / (float)w * 255f);
                Color c = new Color(0, 0, 0, b);
                primitiveBatch.DrawLine(new Vector3((-w + tile.X) * gridSize, i + tile.Y * gridSize, 0), new Vector3((w + tile.X) * gridSize, i + tile.Y * gridSize, 0), 1, c);
                primitiveBatch.DrawLine(new Vector3(i + tile.X * gridSize, (-w + tile.Y) * gridSize, 0), new Vector3(i + tile.X * gridSize, (w + tile.Y) * gridSize, 0), 1, c);
            }

            // Draw selection box
            if (selectionIndex > -1)
            {
                primitiveBatch.DrawBoxWireframe(selectionPosition.Position, selectionBox, 1, Color.Gold);
            }

            primitiveBatch.End();

            // Draw proxy
            if (showProxy) proxyMockupRenderer.Render(sceneCamera);

            // Draw scene
            mockupRenderer.Render(sceneCamera);

#if DEBUG
            // Write debug information
            Globals.MeshesRendered = 0;
            Globals.SpritesRendered = 0;
            Globals.ABTNodesRepositioned = 0;
#endif

            base.Draw(gameTime);
        }
    }
}