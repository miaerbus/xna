
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
    /// XNA TUTORIAL - BASICS MENU
    /// 
    /// This is used for selecting the startup game.
    /// </summary>
    public class BasicsMenu : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D basicsMenu;

        // GAME CONSTRUCTOR
        public BasicsMenu()
        {
            Program.NextGame = -1;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
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
            this.Window.Title = "XNA Tutorial - Basics";
            this.Window.AllowUserResizing = true;

            base.Initialize();
        }


        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                basicsMenu = content.Load<Texture2D>("basics");
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
                Program.NextGame = -1;
                this.Exit();
            }

            int y = Mouse.GetState().Y;
            Program.NextGame = y / 64;
            if (Program.NextGame < 1 || Program.NextGame > 7)
            {
                Program.NextGame = -1;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            spriteBatch.Draw(basicsMenu, Vector2.Zero, Color.White);
            if (Program.NextGame != -1)
            {
                Rectangle crop = new Rectangle(0, 64 * Program.NextGame, 512, 64);
                spriteBatch.Draw(basicsMenu, crop, crop, Color.Gold);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}