
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
    /// XNA TUTORIAL - BASICS PART 4
    /// 
    /// Refactors the particle into a class for more instances.
    /// </summary>
    public class Basics4 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D particleTexture;

        float speed = 300;

        /// <summary>
        /// PARTICLE
        /// Holds data for each individual particle
        /// </summary>
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

        // GAME CONSTRUCTOR
        public Basics4()
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
            this.Window.Title = "XNA Tutorial - Basics Part 4";
            this.Window.AllowUserResizing = true;

            // Create the player particle at the screen center and add it to the list
            playerParticle = new Particle(new Vector2(400, 300), Color.Gold);
            particles.Add(playerParticle);

            // Create some stationary particles and add them to the list
            Random random = new Random(); // Generator of random numbers
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(
                    new Vector2(random.Next(-200, 1000), random.Next(-400, 1000)), // Random position
                    Color.White)); // White color
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

            // Now we have to move the playerParticle.
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

            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            playerParticle.Position += padState.ThumbSticks.Left * new Vector2(1, -1) * elapsed * speed;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            // Render all particles in the list
            for (int i = 0; i < particles.Count; i++)
            {
                spriteBatch.Draw(particleTexture, particles[i].Position, new Rectangle(0, 0, 256, 256),
                    particles[i].Color, 0, new Vector2(128, 128), 1, SpriteEffects.None, 1);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}