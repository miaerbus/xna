using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class SplashRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;

        Texture2D logos;
        Rectangle the3dlevel = new Rectangle(0, 0, 640, 480);
        Rectangle skyreaper = new Rectangle(640, 0, 384, 512);

        float fadeTime = 2;
        float stayTime = 2;

        float time = 0;
        int state = 0;
        Fader logoOpacity;

        public bool Finished
        {
            get
            {
                return state > 6;
            }
        }

        // CONSTRUCTOR
        public SplashRenderer(Game game)
            : base(game)
        {
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            logos = content.Load<Texture2D>(@"Content\Gui\logos");
            logoOpacity = new Fader(0, 255, fadeTime);
        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DimGray);

            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            logoOpacity.Update(elapsed);

            switch (state)
            {
                case 0:
                    logoOpacity.FadeToMax();
                    state++;
                    break;
                case 1:
                    if (logoOpacity.Finished)
                    {
                        state++;
                        time = stayTime;
                    }
                    break;
                case 2:
                    time -= elapsed;
                    if (time < 0)
                    {
                        state++;
                        logoOpacity.FadeToMin();
                    }
                    break;
                case 3:
                    if (logoOpacity.Finished)
                    {
                        logoOpacity.FadeToMax();
                        state++;
                    }
                    break;
                case 4:
                    if (logoOpacity.Finished)
                    {
                        state++;
                        time = stayTime;
                    }
                    break;
                case 5:
                    time -= elapsed;
                    if (time < 0)
                    {
                        state++;
                        logoOpacity.FadeToMin();
                    }
                    break;
                case 6:
                    if (logoOpacity.Finished)
                    {                        
                        state++;
                    }
                    break;
            }

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (state < 4)
                spriteBatch.Draw(
                logos,
                new Vector2((GraphicsDevice.Viewport.Width - the3dlevel.Width) * 0.5f,
                (GraphicsDevice.Viewport.Height - the3dlevel.Height) * 0.5f),
                the3dlevel,
                new Color(255, 255, 255,
                (byte)logoOpacity.Value));
            else
                spriteBatch.Draw(
                logos,
                new Vector2((GraphicsDevice.Viewport.Width - skyreaper.Width) * 0.5f,
                (GraphicsDevice.Viewport.Height - skyreaper.Height) * 0.5f),
                skyreaper,
                new Color(255, 255, 255,
                (byte)logoOpacity.Value));

            spriteBatch.End();
                        
        }        
    }
}
