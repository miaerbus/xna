using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Artificial.XNATutorial
{   
    public class FPSDisplay : DrawableGameComponent
    {      
        // Graphics
        private ContentManager content;
        private Texture2D Font;
        private SpriteBatch spriteBatch;

        // FPS
        int updateFPSCount;
        int updateFPS;
        double updateFPSSecond;

        int renderFPSCount;
        int renderFPS;
        double renderFPSSecond;

        int magnifier = 2;

        // CONSTRUCTOR
        public FPSDisplay(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                Font = content.Load<Texture2D>(@"Content\pixelfont");
            }

            base.LoadGraphicsContent(loadAllContent);
        }

        public override void Update(GameTime gameTime)
        {
            updateFPSCount++;
            updateFPSSecond += gameTime.ElapsedGameTime.TotalSeconds;
            if (updateFPSSecond > 1)
            {
                updateFPS = updateFPSCount;
                updateFPSCount = 0;
                updateFPSSecond--;
            }
        }


        // DRAW COMPONENT
        public override void Draw(GameTime gameTime)
        {
            renderFPSCount++;
            renderFPSSecond += gameTime.ElapsedRealTime.TotalSeconds;
            if (renderFPSSecond > 1)
            {
                renderFPS = renderFPSCount;
                renderFPSCount = 0;
                renderFPSSecond--;
            }
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
            write(spriteBatch, 4 * magnifier, 6 * magnifier, renderFPS.ToString(), Color.White, false);
            write(spriteBatch, 4 * magnifier, 12 * magnifier, updateFPS.ToString(), Color.LightGray, false);
            spriteBatch.End();   
        }

        // INTERNAL WRITE ROUTINE
        private void write(SpriteBatch spriteBatch, int x, int y, string text, Color color, bool centered)
        {
            Vector2 position = new Vector2(x- (centered ? text.Length*2-1 : 0), y);
            text = text.ToUpper();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];                 
                int b = Convert.ToInt32(c);
                b -= 32;
                if (b >= 0 && b <= 64)
                {
                    int cy = b / 16;
                    int cx = b % 16;
                    spriteBatch.Draw(Font, position, new Rectangle(cx * 4 + 1, cy * 6 + 1, 3, 5), color, 0, Vector2.Zero, magnifier, SpriteEffects.None, 0);
                }
                position.X += 4 * magnifier;
            }
        }
    }
}
