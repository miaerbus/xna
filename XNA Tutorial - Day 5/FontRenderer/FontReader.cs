#region File Description
//-----------------------------------------------------------------------------
// FontReader.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace FontRenderer
{
    /// <summary>
    /// Content pipeline support class for loading Font objects from the
    /// compiled XNB format. This is invoked whenever you use
    /// ContentManager.Load to read a Font object.
    /// </summary>
    public class FontReader : ContentTypeReader<Font>
    {
        protected override Font Read(ContentReader input, Font existingInstance)
        {
            // Read the font texture.
            Texture2D texture = input.ReadObject<Texture2D>();

            // Read the lists of character positions.
            List<Rectangle> glyphs = input.ReadObject<List<Rectangle>>();
            List<Rectangle> cropping = input.ReadObject<List<Rectangle>>();

            // Read the table mapping Unicode characters to glyph indices.
            Dictionary<char, int> charMap = input.ReadObject<Dictionary<char, int>>();

            return new Font(texture, glyphs, cropping, charMap);
        }
    }
}
