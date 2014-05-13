using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using FontRenderer;

namespace Artificial.XNATutorial.Bomberman
{
    class ScoreRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        Level level;

        // Font
        Font font;

        // CONSTRUCTOR
        public ScoreRenderer(Game game, Level level)
            : base(game)
        {
            this.level = level;            
            Initialize();
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Font
            font = content.Load<Font>(@"Content\GUI\font");
        }


        // DRAW
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            // Draw the GUI            
            string time = ((int)Gameplay.TimeLeft).ToString();
            if (Gameplay.TimeLeft <= 0) time = "0";
            if (Gameplay.TimeLeft < 10) time = "0" + time;
            font.DrawCentered(time, new Vector2(GraphicsDevice.Viewport.Width * 0.5f, 30), 1, Color.White, spriteBatch);

            spriteBatch.End();
        }        
    }
}
