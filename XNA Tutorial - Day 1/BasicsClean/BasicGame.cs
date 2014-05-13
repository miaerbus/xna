
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

namespace Artificial.XNATutorial.Basics
{
    /// <summary>
    /// XNA TUTORIAL - BASICS CLEAN
    /// 
    /// This is clean version of part 7, ready as a copy/paste reference or a starting point
    /// for playing around with the example.
    /// </summary>
    public class BasicGame : Microsoft.Xna.Framework.Game
    {
        // Graphics objects
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D particleTexture;

        // Camera variables
        Vector3 cameraTarget;
        Vector3 cameraPosition = new Vector3(0, 0, 2000);
        float cameraSpeed = 2000;
        float cameraFOV = (float)Math.PI / 3f;
        float zoomSpeed = 1;

        // Game constants
        float speed = 300;


        // PARTICLE CLASS
        class Particle
        {
            public Vector3 Position;
            public Color Color;

            public Particle(Vector3 position, Color color)
            {
                Position = position;
                Color = color;
            }
        }

        // Game scene
        List<Particle> particles = new List<Particle>();
        Particle playerParticle;


        // GAME CONSTRUCTOR
        public BasicGame()
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
            content = new ContentManager(Services);

            // Set game object variables
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Basic Game";
            this.Window.AllowUserResizing = true;

            // Create player particle
            playerParticle = new Particle(Vector3.Zero, Color.Gold);
            particles.Add(playerParticle);

            // Create other scene particles
            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(new Vector3(random.Next(-1000, 1000), random.Next(-1000, 1000), random.Next(-1000, 1000)), Color.White));
            }

            base.Initialize();
        }


        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // Load the texture
                particleTexture = content.Load<Texture2D>("particle");
            }
        }


        // UNLOAD GRAPHICS CONTENT
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
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

            // Player movement
            if (keyState.IsKeyDown(Keys.Left))
            {
                playerParticle.Position += Vector3.Left * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                playerParticle.Position += Vector3.Right * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerParticle.Position += Vector3.Up * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                playerParticle.Position += Vector3.Down * elapsed * speed;
            }
            playerParticle.Position += new Vector3(padState.ThumbSticks.Left * new Vector2(1, 1) * elapsed * speed, 0);

            // Camera movement
            if (keyState.IsKeyDown(Keys.A))
            {
                cameraPosition += Vector3.Left * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                cameraPosition += Vector3.Right * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                cameraPosition += Vector3.Up * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                cameraPosition += Vector3.Down * elapsed * cameraSpeed;
            }
            cameraPosition += new Vector3(padState.ThumbSticks.Right * new Vector2(1, 1) * elapsed * cameraSpeed, 0);

            // Camera zooming
            if (keyState.IsKeyDown(Keys.R))
            {
                cameraFOV += cameraFOV * elapsed * zoomSpeed;
            }
            if (keyState.IsKeyDown(Keys.F))
            {
                cameraFOV -= cameraFOV * elapsed * zoomSpeed;
            }            
            cameraFOV += cameraFOV * padState.Triggers.Right * elapsed * zoomSpeed;
            cameraFOV -= cameraFOV * padState.Triggers.Left * elapsed * zoomSpeed;

            // Target the camera on the player position
            cameraTarget = playerParticle.Position;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            // Create the transformation using the current camera variables
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(cameraFOV, (float)device.Viewport.Width / (float)device.Viewport.Height, 10, 10000);
            Matrix screen = Matrix.CreateTranslation(1, -1, 0) * Matrix.CreateScale(device.Viewport.Width / 2, -device.Viewport.Height / 2, 1);
            float size = 256f;

            // Render the scene
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            for (int i = 0; i < particles.Count; i++)
            {
                // Calculate the screen position and depth
                Vector3 viewposition = Vector3.Transform(particles[i].Position, view);
                Vector4 position4D = Vector4.Transform(viewposition, projection * screen);
                position4D /= position4D.W;
                Vector2 position = new Vector2(position4D.X, position4D.Y);
                float depth = position4D.Z;

                // Calculate the scale
                position4D = Vector4.Transform(viewposition + Vector3.UnitY * size, projection * screen);
                position4D /= position4D.W;
                Vector2 position2 = new Vector2(position4D.X, position4D.Y);
                float scale = (position2 - position).Length() / 256f;

                // Render the particle
                spriteBatch.Draw(particleTexture, position, new Rectangle(0, 0, 256, 256), particles[i].Color, 0, new Vector2(128, 128), scale, SpriteEffects.None, depth);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}