
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
    /// XNA TUTORIAL - BASICS PART 6
    /// 
    /// Introduces matrix transformations.
    /// </summary>
    public class Basics6 : Microsoft.Xna.Framework.Game
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
        Vector2 cameraTarget;
        float cameraSpeed = 300;
        float cameraZoom = 1;
        float zoomSpeed = 2;

        // GAME CONSTRUCTOR
        public Basics6()
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
            this.Window.Title = "XNA Tutorial - Basics Part 6";
            this.Window.AllowUserResizing = true;

            playerParticle = new Particle(Vector2.Zero, Color.Gold);
            particles.Add(playerParticle);

            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(
                    new Vector2(random.Next(-1000, 1000), random.Next(-1000, 1000)), Color.White));
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

            // Here we use a matrix to store information about the transform since it is the same for
            // the whole frame. We use the exact same changes using translation for moving and scale
            // for multiplying.
            //
            // All the matrices are multiplied in the same order as we changed the position before.
            // If we would use different order when directly changing the position the result would
            // not be the same, so this holds also for matrix multiplication. It is not comutative.
            Matrix transform =
                Matrix.CreateTranslation(new Vector3(-cameraTarget, 0)) * // Move target to zero
                Matrix.CreateScale(cameraZoom) * // Scale around the new origin
                Matrix.CreateTranslation(400, 300, 0); // Offset to center

            for (int i = 0; i < particles.Count; i++)
            {
                // Transform the position from world space to screen space with the view matrix
                Vector2 position = Vector2.Transform(particles[i].Position, transform);
                // Transform a unit vector and use its length for scale. We use the TransformNormal
                // method so it treats the vector as a direction only (= normal) instead as a
                // position coordinate.
                float scale = Vector2.TransformNormal(Vector2.UnitX, transform).Length();

                spriteBatch.Draw(particleTexture, position, new Rectangle(0, 0, 256, 256),
                    particles[i].Color, 0, new Vector2(128, 128), scale, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}