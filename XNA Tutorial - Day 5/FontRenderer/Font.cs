#region File Description
//-----------------------------------------------------------------------------
// Font.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FontRenderer
{
    /// <summary>
    /// The main runtime font class. These objects are loaded from XNB format
    /// using the ContentManager, and contain all the information needed to
    /// render text to the screen.
    /// </summary>
    public class Font
    {
        #region Fields

        // A single texture contains all the characters of the font.
        Texture2D textureValue;

        // Remember where in the texture each character is stored.
        List<Rectangle> glyphData;

        // Remember how much blank space was cropped around the edge of each character.
        List<Rectangle> croppingData;

        // Map Unicode character values to indices into glyphData and croppingData.
        Dictionary<char, int> characterMap;

        #endregion
        

        /// <summary>
        /// The constructor is internal, because fonts should only be created
        /// by loading from XNB files (using the ContentManager, which calls
        /// into the FontReader helper class).
        /// </summary>
        internal Font(Texture2D texture, List<Rectangle> glyphs,
                      List<Rectangle> cropping, Dictionary<char, int> charMap)
        {
            textureValue = texture;
            glyphData = glyphs;
            croppingData = cropping;
            characterMap = charMap;
        }


        /// <summary>
        /// Draws text to the screen.
        /// </summary>
        public void Draw(string text, Vector2 position, float scale,
                         Color color, SpriteBatch spriteBatch)
        {
            foreach (char ch in text)
            {
                // Look up what font glyph corresponds to this Unicode character.
                int index;

                if (!characterMap.TryGetValue(ch, out index))
                    index = glyphData.Count - 1;

                // Look up what part of the texture represents this character.
                Rectangle glyph = glyphData[index];
                Rectangle cropping = croppingData[index];

                // Draw the character.
                Vector2 pos = position + new Vector2(cropping.X, cropping.Y) * scale;

                spriteBatch.Draw(textureValue, pos, glyph, color, 0,
                                 Vector2.Zero, scale, SpriteEffects.None, 0);

                // Move the position further to the right.
                position.X += cropping.Width * scale;
            }
        }

        public void DrawCentered(string text, Vector2 position, float scale,
                         Color color, SpriteBatch spriteBatch)
        {
            Draw(text, new Vector2(position.X - CalculateWidth(text, scale) * 0.5f, position.Y - CalculateHeight(scale) * 0.5f), scale, color, spriteBatch);
        }

        /// <summary>
        /// Calculates the width of text rendered with this font
        /// </summary>
        public float CalculateWidth(string text, float scale)
        {
            float width = 0;

            foreach (char ch in text)
            {

                // Look up what font glyph corresponds to this Unicode character.
                int index;

                if (!characterMap.TryGetValue(ch, out index))
                    index = glyphData.Count - 1;

                // Look up what part of the texture represents this character.
                Rectangle glyph = glyphData[index];
                Rectangle cropping = croppingData[index];

                width += cropping.Width * scale;
            }

            return width;
        }

        public float CalculateHeight(float scale)
        {
            return croppingData[0].Height * scale;
        }


        /// <summary>
        /// Gets the combined font texture page.
        /// </summary>
        public Texture2D Texture
        {
            get { return textureValue; }
        }
    }
}
