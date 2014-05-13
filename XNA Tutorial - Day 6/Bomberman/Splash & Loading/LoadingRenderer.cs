using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.GUI;
using FontRenderer;

namespace Artificial.XNATutorial.Bomberman
{
    public class LoadingRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        Scene scene;

        // Font
        Font font;
        public Font Font
        {
            get
            {
                return font;
            }
        }

        // Background
        Texture2D background;

        // Resizing
        float scale;
        float aspectRatio;

        // CONSTRUCTOR
        public LoadingRenderer(Game game)
            : base(game)
        {
            Initialize();
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font
            font = content.Load<Font>(@"Content\GUI\font");

            // Background
            background = content.Load<Texture2D>(@"Content\GUI\background0");

            aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            scale = (float)GraphicsDevice.Viewport.Height / 600f;

        }

        // Transform
        Vector2 PlaceOnScreen(Vector3 position)
        {
            Vector2 result = new Vector2(position.X, position.Z);
            result -= new Vector2(400, 300);
            result *= scale;
            result +=new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.5f);
            return result;
        }


        // DRAW
        public override void Draw(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;

            // Prepare for rendering
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            GraphicsDevice.RenderState.CullMode = CullMode.None;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            // Background
            spriteBatch.Draw(
                background,
                PlaceOnScreen(new Vector3(400, 0, 300)),
                new Rectangle(0, 0, 600, 800),
                Color.White,
                0,
                new Vector2(300, 400),
                scale,
                SpriteEffects.None,
                1
                );


            font.DrawCentered("LOADING", PlaceOnScreen(new Vector3(400, 0, 300)), 1, Color.White, spriteBatch);

            spriteBatch.End();
        }
    }
}
