
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
    /// XNA TUTORIAL - BASICS PART 3
    /// 
    /// Demonstrates moving the sprite with the keys and gamepad.
    /// </summary>
    public class Basics3 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D particleTexture;

        Vector2 position = new Vector2(400,300); // Position of the particle, at start set to screen center
        float speed = 100; // Speed for moving the particle in pixels per second

        // GAME CONSTRUCTOR
        public Basics3()
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
            this.Window.Title = "XNA Tutorial - Basics Part 3";
            this.Window.AllowUserResizing = true;

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

            // Get elapsed time in second for animation calculation
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Captures which keys are pressed down at this exact moment and saves it in the keyboard states
            KeyboardState keyState = Keyboard.GetState();

            // Handle individual keys
            if (keyState.IsKeyDown(Keys.Left))
            {
                position += new Vector2(-1, 0) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                position += new Vector2(1, 0) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                position += new Vector2(0, -1) * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                position += new Vector2(0, 1) * elapsed * speed;
            }

            // If you are lucky enough to have the Xbox controller connected to windows
            // you get to control the particle using analog precision!
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // Handle left thumb stick. The input is already a 2D vector in the -1 to 1 range so
            // we need much less code on the gamepad side of things. The only problem we have is
            // that in screenspace, moving up is negative and down is positive so we have to take
            // this into account.
            position += padState.ThumbSticks.Left * new Vector2(1, -1) * elapsed * speed;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            // Instead of a fixed position we set the position variable as the target position
            spriteBatch.Draw(particleTexture, position, new Rectangle(0, 0, 256, 256), Color.White, 0,
                new Vector2(128, 128), 1, SpriteEffects.None, 1);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}