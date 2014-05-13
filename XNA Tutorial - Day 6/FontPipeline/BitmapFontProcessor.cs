#region File Description
//-----------------------------------------------------------------------------
// BitmapFontProcessor.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// Custom content pipeline processor converts a specially marked 2D bitmap
    /// into a font. Each character should be arranged on the bitmap in a grid
    /// ordered from top left to bottom right. Monochrome characters should use
    /// white for solid areas and black for transparent areas. To include
    /// multicolored characters, add an alpha channel to the bitmap and use that
    /// to control which parts of the character are solid. The spaces between
    /// each character and around the edges of the grid should be filled with
    /// bright pink, red=255, green=0, blue=255. It doesn't matter if your grid
    /// includes lots of wasted space, because the processor will rearrange the
    /// characters, packing them together as tightly as possible.
    /// </summary>
    [ContentProcessor]
    public class BitmapFontProcessor : ContentProcessor<Texture2DContent, FontContent>
    {
        /// <summary>
        /// The main process method responsible for creating font data from a bitmap.
        /// </summary>
        public override FontContent Process(Texture2DContent input,
                                            ContentProcessorContext context)
        {
            // Convert the source texture to standard Color format.
            input.ConvertBitmapType(typeof(PixelBitmapContent<Color>));

            PixelBitmapContent<Color> source;
            source = (PixelBitmapContent<Color>)input.Mipmaps[0];

            // Split the source image into a list of individual glyph bitmaps.
            List<BitmapContent> glyphs = new List<BitmapContent>();

            foreach (Rectangle rectangle in FindGlyphs(source))
                glyphs.Add(ExtractGlyph(source, rectangle));

            // Pack all the glyphs into a single large texture.
            FontContent font = new FontContent();

            GlyphPacker.CropGlyphs(glyphs, font.Cropping);

            font.Texture.Mipmaps = GlyphPacker.ArrangeGlyphs(glyphs, font.Glyphs,
                                                             context);

            // Fill in a default character mapping table.
            for (int i = 0; i < glyphs.Count; i++)
                font.CharacterMap.Add((char)(' ' + i), i);

            return font;
        }


        /// <summary>
        /// Searches a 2D bitmap for characters that are
        /// surrounded by a marker pink color.
        /// </summary>
        static IEnumerable<Rectangle> FindGlyphs(PixelBitmapContent<Color> source)
        {
            for (int y = 1; y < source.Height; y++)
            {
                for (int x = 1; x < source.Width; x++)
                {
                    // Look for the top left corner of a character (a pixel that is
                    // not pink, but was pink immediately to the left and above it)
                    if ((source.GetPixel(x, y) != Color.Magenta) &&
                        (source.GetPixel(x - 1, y) == Color.Magenta) &&
                        (source.GetPixel(x, y - 1) == Color.Magenta))
                    {
                        // Measure the size of this character.
                        int w = 1, h = 1;

                        while ((x + w < source.Width) &&
                               (source.GetPixel(x + w, y) != Color.Magenta))
                        {
                            w++;
                        }

                        while ((y + h < source.Height) &&
                               (source.GetPixel(x, y + h) != Color.Magenta))
                        {
                            h++;
                        }

                        yield return new Rectangle(x, y, w, h);
                    }
                }
            }
        }


        /// <summary>
        /// Extracts a subregion from a larger 2D bitmap.
        /// </summary>
        static PixelBitmapContent<Color> ExtractGlyph(PixelBitmapContent<Color> source,
                                                      Rectangle rectangle)
        {
            PixelBitmapContent<Color> glyph;
            glyph = new PixelBitmapContent<Color>(rectangle.Width, rectangle.Height);

            BitmapContent.Copy(source, rectangle, glyph,
                               new Rectangle(0, 0, rectangle.Width, rectangle.Height));

            // If the bitmap doesn't already have an alpha channel, create one now.
            if (!BitmapContainsAlpha(glyph))
                ConvertGreyToAlpha(glyph);

            return glyph;
        }


        /// <summary>
        /// Checks whether a bitmap contains anything other than solid alpha.
        /// </summary>
        static bool BitmapContainsAlpha(PixelBitmapContent<Color> bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    byte alpha = bitmap.GetPixel(x, y).A;

                    if (alpha != 0xFF)
                        return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Converts greyscale luminosity to alpha data.
        /// </summary>
        static void ConvertGreyToAlpha(PixelBitmapContent<Color> bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    // Average the red, green and blue values to compute brightness.
                    int alpha = (pixel.R + pixel.G + pixel.B) / 3;

                    bitmap.SetPixel(x, y, new Color(255, 255, 255, (byte)alpha));
                }
            }
        }
    }
}
