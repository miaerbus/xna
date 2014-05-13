using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Artificial.XNATutorial
{
    public class TextBatch : GameComponent
    {        
        // Graphics
        GraphicsDevice device;
        ContentManager content;
        Texture2D Font;
        SpriteBatch spriteBatch;

        // Transform
        Matrix screen;
        Matrix viewTransform;
        Matrix screenTransform;

        // Internal text representation
        struct ColoredString
        {
            public Vector3 Position;
            public Color Color;
            public StringBuilder Text;            
            public int Size;
            public bool Centered;
        }
        ColoredString[] worldText = new ColoredString[16];
        ColoredString[] screenText = new ColoredString[16];
        int worldTextCount = 0;
        int screenTextCount = 0;

        // CONSTRUCTOR
        public TextBatch(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();
            device = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            Font = content.Load<Texture2D>(@"Content\pixelfont");
            screen = Matrix.CreateTranslation(1, -1, 0) * Matrix.CreateScale(device.Viewport.Width / 2, -device.Viewport.Height / 2, 1);

            for (int i = 0; i < 16; i++)
            {
                worldText[i].Text = new StringBuilder(64);
                screenText[i].Text = new StringBuilder(64);
            }
        }

        // BEGIN
        public void Begin(Matrix view, Matrix projection)
        {
            // Set the transformations for text in world space
            viewTransform = view;
            screenTransform = projection * screen;
        }

        // ADD TEXT
        public StringBuilder GetNextScreenText()
        {
            screenText[screenTextCount].Text.Length = 0;
            return screenText[screenTextCount].Text;
        }
        public void Write(Vector2 position, int size, Color color, bool centered)
        {
            screenText[screenTextCount].Position.X = position.X;
            screenText[screenTextCount].Position.Y = position.Y;
            screenText[screenTextCount].Size = size;
            screenText[screenTextCount].Color = color;
            screenText[screenTextCount].Centered = centered;
            screenTextCount++;
            if (screenTextCount == screenText.Length)
            {
                Array.Resize<ColoredString>(ref screenText, screenTextCount * 2);
                for (int i = screenTextCount; i < screenText.Length; i++)
                {
                    screenText[i].Text = new StringBuilder(64);
                }
            }
        }

        public void Write(Vector3 position, String text, int size, Color color, bool centered)
        {
            worldText[worldTextCount].Position = position;
            worldText[worldTextCount].Text.Length = 0;
            worldText[worldTextCount].Text.Append(text);
            worldText[worldTextCount].Size = size;
            worldText[worldTextCount].Color = color;
            worldText[worldTextCount].Centered = centered;
            worldTextCount++;
            if (worldTextCount == worldText.Length)
            {
                Array.Resize<ColoredString>(ref worldText, worldTextCount * 2);
            }
        }

        // END
        public void End()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            device.SamplerStates[0].MagFilter = TextureFilter.Point;

            ColoredString s;
            int x,y;

            // world text
            for (int i = 0; i < worldTextCount; i++)
            {
                s = worldText[i];
                Vector3 viewposition = Vector3.Transform(s.Position, viewTransform);
                Vector4 position4D = Vector4.Transform(viewposition, screenTransform);
                position4D /= position4D.W;
                x = (int)position4D.X;
                y = (int)position4D.Y;
                write(spriteBatch, x, y, s.Text, s.Size, s.Color, s.Centered);
            }

            // screen text
            for (int i = 0; i < screenTextCount; i++)
            {
                s = screenText[i];
                x = (int)s.Position.X;
                y = (int)s.Position.Y;
                write(spriteBatch, x, y, s.Text, s.Size, s.Color, s.Centered);
            }

            worldTextCount = 0;
            screenTextCount = 0;
            spriteBatch.End();   
        }

        // INTERNAL WRITE ROUTINE
        private void write(SpriteBatch spriteBatch, int x, int y, StringBuilder text, int size, Color color, bool centered)
        {
            Vector2 position = new Vector2(x - (centered ? text.Length * 2 * size - 1 : 0), y);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int b = Convert.ToInt32(char.ToUpperInvariant(c));
                b -= 32;
                if (b >= 0 && b <= 64)
                {
                    int cy = b / 16;
                    int cx = b % 16;
                    spriteBatch.Draw(Font, position, new Rectangle(cx * 4 + 1, cy * 6 + 1, 3, 5), color, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                }
                position.X += 4 * size;
            }
        }
    }
}
