#region File Description
//-----------------------------------------------------------------------------
// TrueTypeFontProcessor.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// Custom content pipeline processor converts a font description class,
    /// which contains the name and size of a TrueType font, into an actual
    /// font that we can use in a game.
    /// </summary>
    [ContentProcessor]
    public class TrueTypeFontProcessor : ContentProcessor<FontDescription, FontContent>
    {
        /// <summary>
        /// The main process method responsible for creating
        /// font data from a description class.
        /// </summary>
        public override FontContent Process(FontDescription input,
                                            ContentProcessorContext context)
        {
            FontContent font = new FontContent();

            // Rasterizer helper wraps TrueType functionality from System.Drawing.
            using (FontRasterizer rasterizer = new FontRasterizer(input))
            {
                List<BitmapContent> glyphs = new List<BitmapContent>();

                // For each character wanted by this font.
                foreach (FontDescription.Region region in input.Regions)
                {
                    for (char ch = region.Start; ch <= region.End; ch++)
                    {
                        // Record a Unicode to index mapping table.
                        font.CharacterMap.Add(ch, glyphs.Count);

                        // Rasterize the character glyph.
                        glyphs.Add(rasterizer.Rasterize(ch));
                    }
                }

                // Pack all the glyphs into a single large texture.
                GlyphPacker.CropGlyphs(glyphs, font.Cropping);

                font.Texture.Mipmaps = GlyphPacker.ArrangeGlyphs(glyphs, font.Glyphs,
                                                                 context);
            }

            return font;
        }
    }
}
