
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
    /// XNA TUTORIAL - BASICS PART 5
    /// 
    /// Adding camera.
    /// </summary>
    public class Basics5 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D particleTexture;

        float speed = 300;

        // PARTICLE
        class Particle
        {
            public Vector2 Position;
            public Color Color;

            public Particle(Vector2 position, Color color)
            {
                Position = position;
                Color = color;
            }
        }

        List<Particle> particles = new List<Particle>();
        Particle playerParticle;

        // Camera variables
        Vector2 cameraTarget; // What coordinate is at the center of the screen
        float cameraSpeed = 300; // Factor for moving the camera
        float cameraZoom = 1; // The zoom of the camera
        float zoomSpeed = 2; // Factor for zooming the camera

        // GAME CONSTRUCTOR
        public Basics5()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            content = new ContentManager(Services);

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Basics Part 5";
            this.Window.AllowUserResizing = true;

            playerParticle = new Particle(Vector2.Zero, Color.Gold); // Now we can place the player
                                                                     // starting position to zero
            particles.Add(playerParticle);

            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(
                    new Vector2(random.Next(-1000, 1000),       // Place particles uniformely around
                    random.Next(-1000, 1000)), Color.White));   // the world's zero coordinate
            }

            base.Initialize();
        }


        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Left))
            {
                playerParticle.Position += new Vector2(-1, 0) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                playerParticle.Position += new Vector2(1, 0) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerParticle.Position += new Vector2(0, -1) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                playerParticle.Position += new Vector2(0, 1) * elapsed * speed;
            }
            // We add camera movement to WASD in the same way as before, changing the cameraTarget
            if (keyState.IsKeyDown(Keys.A))
            {
                cameraTarget += new Vector2(-1, 0) * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                cameraTarget += new Vector2(1, 0) * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                cameraTarget += new Vector2(0, -1) * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                cameraTarget += new Vector2(0, 1) * elapsed * cameraSpeed;
            }
            // We add zooming to R and F
            if (keyState.IsKeyDown(Keys.R))
            {
                cameraZoom += cameraZoom * elapsed * zoomSpeed;
            }
            if (keyState.IsKeyDown(Keys.F))
            {
                cameraZoom -= cameraZoom * elapsed * zoomSpeed;
            }

            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            playerParticle.Position += padState.ThumbSticks.Left * new Vector2(1, -1) * elapsed * speed;
            // Same for gamepad, just using the right thumb stick and triggers
            cameraTarget += padState.ThumbSticks.Right * new Vector2(1, -1) * elapsed * cameraSpeed;
            cameraZoom += cameraZoom * padState.Triggers.Right * elapsed * zoomSpeed;
            cameraZoom -= cameraZoom * padState.Triggers.Left * elapsed * zoomSpeed;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            for (int i = 0; i < particles.Count; i++)
            {
                // Now we have to adjust the particle position in relation to camera
                Vector2 position = particles[i].Position - cameraTarget; // Move target to zero
                position *= cameraZoom; // Scale around the new origin (= cameraTarget)
                position += new Vector2(400,300); // Offset to center

                // Scale the sprite as well
                float scale = cameraZoom;

                spriteBatch.Draw(particleTexture, position, new Rectangle(0, 0, 256, 256),
                    particles[i].Color, 0, new Vector2(128, 128), scale, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}