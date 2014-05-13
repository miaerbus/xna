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
    public class DebugOutput : DrawableGameComponent
    {      
        // Graphics
        private ContentManager content;
        private Texture2D Font;
        private SpriteBatch spriteBatch;

        // Internal text representation
        struct ColoredString
        {
            public Color Color;
            public String Text;
            public ColoredString(String InitialText, Color InitialColor)
            {
                Color = InitialColor;
                Text = InitialText;
            }
        }
        private List<ColoredString> DebugOutputs = new List<ColoredString>();

        // CONSTRUCTOR
        public DebugOutput(Game game)
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

        // ADD TEXT
        public void Write(String text, Color color)
        {
            DebugOutputs.Add(new ColoredString(text, color));
        }

        // DRAW COMPONENT
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
            int y=10;
            foreach (ColoredString s in DebugOutputs)
            {
                write(spriteBatch, 12, y, s.Text, s.Color, false);
                y += 20;
            }
            DebugOutputs.Clear();
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
                    spriteBatch.Draw(Font, position, new Rectangle(cx * 4 + 1, cy * 6 + 1, 3, 5), color, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                }
                position.X += 4*2;
            }
        }
    }
}
