
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
    /// XNA TUTORIAL - BASICS PART 2
    /// 
    /// Demonstrates rendering a texture with Sprite Batch.
    /// </summary>
    public class Basics2 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device; // Provides access to the graphics card functionality.
        SpriteBatch spriteBatch; // Renders sprites (textures)
        Texture2D particleTexture; // Particle texture

        // GAME CONSTRUCTOR
        public Basics2()
        {
            // Create the graphics manager
            graphics = new GraphicsDeviceManager(this);

            // Confifure the device            
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;          
        }


        // INITIALIZE
        protected override void Initialize()
        {
            // Get the created device for quicker access
            device = graphics.GraphicsDevice;

            // Create a sprite batch that will draw using this device            
            spriteBatch = new SpriteBatch(device);

            content = new ContentManager(Services);

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Basics Part 2";
            this.Window.AllowUserResizing = true;

            base.Initialize();
        }


        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // Here we load all content added to the project from the solution explorer
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

            // Nothing to update yet. See next part for adding interactivity.

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            // Start the sprite batch. This is required because the sprite batch optimizes
            // rendering of multiple sprites and needs to initialize. The call without
            // any parameters defaults to SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred
            // and SaveStateMode.None.
            //
            // SpriteBlendMode controls whether sprites are alpha blended or added to the existing
            // render target (usefull for special effects like fire / explosions).
            //
            // SpriteSortMode controls in what order the batch draws the sprites. Immediate draws
            // individual sprites each time draw is called, while deffered waits until the end call.
            // You can also apply sorting by depth or texture which can improve performance or quality.
            //
            // Because the sprite batch changes the render state to draw the sprites it is desirable
            // to be able to restore the previous state. This can be done by setting SaveStateMode
            // to SaveState
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);
            
            // Draw the particle sprite. There are many overloads that let you pass in the parameters.
            // We are using the one that lets us choose an origin point on our texture that will then
            // match the target position. Also, if we apply rotation, the sprite rotates around that origin.
            //
            // One very important thing to know is that all source and destination coordinates are
            // in pixel space, meaning in our case that if we want to set the origin to the middle of our
            // texture, the coordinates are 128,128 (since the texture is 256x256) and the to render to the
            // middle of the screen, the position should be 300,400.
            spriteBatch.Draw(
                particleTexture, // Texture to render
                new Vector2(400, 300), // Where the origin should be positioned on the screen
                new Rectangle(0, 0, 256, 256), // Which part of texture to render
                Color.White, // Color to multiply the texture with (white is like multiplying by 1)
                0, // Angle in radians for which to rotate the texture
                new Vector2(128, 128), // Where on the texture is the origin, relative to the source rectangle
                1, // What is the scale of the texture
                SpriteEffects.None, // Used for fliping the texture horizontally or vertically
                0); // The depth of the texture by which it can be sorted

            // When all the sprites are added to the batch with Draw calls, the end call renders the sprites
            // unless SpriteSortMode is set to Immediate.
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}