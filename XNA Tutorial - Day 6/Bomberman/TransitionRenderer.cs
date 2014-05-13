using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class TransitionRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;

        Texture2D previousScreen;

        float fadeTime = 0.5f;
        Fader opacity;


        // CONSTRUCTOR
        public TransitionRenderer(Game game)
            : base(game)
        {
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            opacity = new Fader(0, 255, fadeTime);
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);
            previousScreen = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, ResourceUsage.ResolveTarget, SurfaceFormat.Color, ResourceManagementMode.Manual);
        }

        bool capture;
        public void Transition()
        {
            capture = true;
            opacity.Value = 255;
            opacity.FadeToMin();
        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            if (elapsed > 0.05f) elapsed = 0.05f;
            opacity.Update(elapsed);

            if (capture)
            {
                GraphicsDevice.ResolveBackBuffer(previousScreen);
                capture = false;
            }

            if (!opacity.Finished)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(previousScreen, Vector2.Zero, new Color(255, 255, 255, (byte)opacity.Value));
                spriteBatch.End();
            }
        }        
    }
}
