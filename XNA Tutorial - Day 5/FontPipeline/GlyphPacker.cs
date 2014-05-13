#region File Description
//-----------------------------------------------------------------------------
// GlyphPacker.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// Helper class for arranging many small bitmaps onto a single larger surface.
    /// </summary>
    public static class GlyphPacker
    {
        // How much padding should we include around the arranged images?
        const int padding = 1;


        /// <summary>
        /// Packs a list of bitmaps into a single big texture,
        /// recording where each one was stored.
        /// </summary>
        public static BitmapContent ArrangeGlyphs(IList<BitmapContent> sourceGlyphs,
                                                  ICollection<Rectangle> outputGlyphs,
                                                  ContentProcessorContext context)
        {
            if (sourceGlyphs.Count == 0)
                throw new InvalidContentException("There are no glyphs to arrange");

            // Build up a list of all the glyphs needing to be arranged.
            List<ArrangedGlyph> glyphs = new List<ArrangedGlyph>();

            for (int i = 0; i < sourceGlyphs.Count; i++)
            {
                ArrangedGlyph glyph = new ArrangedGlyph();

                glyph.Width = sourceGlyphs[i].Width + padding * 2;
                glyph.Height = sourceGlyphs[i].Height + padding * 2;

                glyph.Index = i;

                glyphs.Add(glyph);
            }

            // Sort so the largest glyphs get arranged first.
            glyphs.Sort(CompareGlyphSizes);

            // Work out how big the output bitmap should be.
            int outputWidth = GuessOutputWidth(sourceGlyphs);
            int outputHeight = 0;
            int totalGlyphSize = 0;

            // Choose positions for each glyph, one at a time.
            for (int i = 0; i < glyphs.Count; i++)
            {
                PositionGlyph(glyphs, i, outputWidth);

                outputHeight = Math.Max(outputHeight, glyphs[i].Y + glyphs[i].Height);

                totalGlyphSize += glyphs[i].Width * glyphs[i].Height;
            }

            // Sort the glyphs back into index order.
            glyphs.Sort(CompareGlyphIndices);

            // Some graphics cards don't support textures that aren't
            // powers of two, so make sure our output size is good.
            outputHeight = RoundUpToPowerOfTwo(outputHeight);

            context.Logger.LogImportantMessage(
                "Packed {0} glyphs into a {1}x{2} texture, {3}% efficiency",
                glyphs.Count, outputWidth, outputHeight,
                totalGlyphSize * 100 / outputWidth / outputHeight);

            return CopyGlyphsToOutput(glyphs, sourceGlyphs, outputGlyphs,
                                      outputWidth, outputHeight);
        }


        /// <summary>
        /// Once the arranging is complete, copies the bitmap data for each
        /// glyph to its chosen position in the single larger output bitmap.
        /// </summary>
        static BitmapContent CopyGlyphsToOutput(List<ArrangedGlyph> glyphs,
                                                IList<BitmapContent> sourceGlyphs,
                                                ICollection<Rectangle> outputGlyphs,
                                                int width, int height)
        {
            BitmapContent output = new PixelBitmapContent<Color>(width, height);

            foreach (ArrangedGlyph glyph in glyphs)
            {
                BitmapContent sourceGlyph = sourceGlyphs[glyph.Index];

                Rectangle sourceRegion = new Rectangle(0, 0, sourceGlyph.Width,
                                                             sourceGlyph.Height);
                
                Rectangle destinationRegion = new Rectangle(glyph.X + padding,
                                                            glyph.Y + padding,
                                                            sourceGlyph.Width,
                                                            sourceGlyph.Height);

                BitmapContent.Copy(sourceGlyph, sourceRegion,
                                   output, destinationRegion);

                outputGlyphs.Add(destinationRegion);
            }

            return output;
        }


        /// <summary>
        /// Internal helper class keeps track of a glyph while it is being arranged.
        /// </summary>
        class ArrangedGlyph
        {
            public int Index;

            public int X;
            public int Y;

            public int Width;
            public int Height;
        }


        /// <summary>
        /// Works out where to position a single glyph.
        /// </summary>
        static void PositionGlyph(List<ArrangedGlyph> glyphs,
                                  int index, int outputWidth)
        {
            int x = 0;
            int y = 0;

            while (true)
            {
                // Is this position free for us to use?
                int intersects = FindIntersectingGlyph(glyphs, index, x, y);

                if (intersects < 0)
                {
                    glyphs[index].X = x;
                    glyphs[index].Y = y;

                    return;
                }

                // Skip past the existing glyph that we collided with.
                x = glyphs[intersects].X + glyphs[intersects].Width;

                // If we ran out of room to move to the right,
                // try the next line down instead.
                if (x + glyphs[index].Width > outputWidth)
                {
                    x = 0;
                    y++;
                }
            }
        }


        /// <summary>
        /// Checks if a proposed glyph position collides with anything
        /// that we already arranged.
        /// </summary>
        static int FindIntersectingGlyph(List<ArrangedGlyph> glyphs,
                                         int index, int x, int y)
        {
            int w = glyphs[index].Width;
            int h = glyphs[index].Height;

            for (int i = 0; i < index; i++)
            {
                if (glyphs[i].X >= x + w)
                    continue;

                if (glyphs[i].X + glyphs[i].Width <= x)
                    continue;

                if (glyphs[i].Y >= y + h)
                    continue;

                if (glyphs[i].Y + glyphs[i].Height <= y)
                    continue;

                return i;
            }

            return -1;
        }


        /// <summary>
        /// Comparison function for sorting glyphs by size.
        /// </summary>
        static int CompareGlyphSizes(ArrangedGlyph a, ArrangedGlyph b)
        {
            int aSize = a.Height * 1024 + a.Width;
            int bSize = b.Height * 1024 + b.Width;

            return bSize.CompareTo(aSize);
        }


        /// <summary>
        /// Comparison function for sorting glyphs by their original indices.
        /// </summary>
        static int CompareGlyphIndices(ArrangedGlyph a, ArrangedGlyph b)
        {
            return a.Index.CompareTo(b.Index);
        }


        /// <summary>
        /// Heuristic guesses what might be a good output width for a list of glyphs.
        /// </summary>
        static int GuessOutputWidth(IList<BitmapContent> sourceGlyphs)
        {
            int maxWidth = 0;
            int totalSize = 0;

            foreach (BitmapContent glyph in sourceGlyphs)
            {
                maxWidth = Math.Max(maxWidth, glyph.Width);
                totalSize += glyph.Width * glyph.Height;
            }

            int width = Math.Max((int)Math.Sqrt(totalSize), maxWidth);

            return RoundUpToPowerOfTwo(width);
        }


        /// <summary>
        /// Rounds a value up to the next larger power of two.
        /// </summary>
        static int RoundUpToPowerOfTwo(int value)
        {
            int powerOfTwo = 1;

            while (powerOfTwo < value)
                powerOfTwo <<= 1;

            return powerOfTwo;
        }

        
        /// <summary>
        /// Crops unused space from around the edge of each entry in a list of bitmaps.
        /// </summary>
        public static void CropGlyphs(IList<BitmapContent> glyphs,
                                      ICollection<Rectangle> outputCropping)
        {
            for (int i = 0; i < glyphs.Count; i++)
            {
                // Does this glyph need to be cropped?
                BitmapContent glyph = glyphs[i];

                Rectangle croppedSize = ChooseCropping(glyph);

                if ((croppedSize.Width < glyph.Width) ||
                    (croppedSize.Height < glyph.Height))
                {
                    // Replace the bitmap with a cropped version of itself.
                    BitmapContent croppedGlyph;

                    croppedGlyph = new PixelBitmapContent<Color>(croppedSize.Width,
                                                                 croppedSize.Height);

                    BitmapContent.Copy(glyph, croppedSize, croppedGlyph,
                            new Rectangle(0, 0, croppedSize.Width, croppedSize.Height));

                    glyphs[i] = croppedGlyph;

                    // Store how much we cropped by, so the renderer can draw
                    // the glyph back in the original uncropped position.
                    outputCropping.Add(new Rectangle(croppedSize.X, croppedSize.Y,
                                                     glyph.Width, glyph.Height));
                }
            }
        }


        /// <summary>
        /// Works out how much a glyph bitmap should be cropped.
        /// </summary>
        static Rectangle ChooseCropping(BitmapContent glyph)
        {
            PixelBitmapContent<Color> bitmap = (PixelBitmapContent<Color>)glyph;

            Rectangle crop = new Rectangle(0, 0, glyph.Width, glyph.Height);

            // Crop the left.
            while ((crop.Width > 1) &&
                   (BitmapIsEmpty(bitmap, crop.X, crop.Y, 1, crop.Height)))
            {
                crop.X++;
                crop.Width--;
            }

            // Crop the right.
            while ((crop.Width > 1) &&
                   (BitmapIsEmpty(bitmap, crop.Right - 1, crop.Y, 1, crop.Height)))
                crop.Width--;

            // Crop the top.
            while ((crop.Height > 1) &&
                   (BitmapIsEmpty(bitmap, crop.X, crop.Y, crop.Width, 1)))
            {
                crop.Y++;
                crop.Height--;
            }

            // Crop the bottom.
            while ((crop.Height > 1) &&
                   (BitmapIsEmpty(bitmap, crop.X, crop.Bottom - 1, crop.Width, 1)))
                crop.Height--;

            return crop;
        }


        /// <summary>
        /// Checks whether an area of a bitmap contains any non-zero alpha values.
        /// </summary>
        static bool BitmapIsEmpty(PixelBitmapContent<Color> bitmap,
                                  int x, int y, int w, int h)
        {
            for (int u = 0; u < w; u++)
            {
                for (int v = 0; v < h; v++)
                {
                    byte alpha = bitmap.GetPixel(x + u, y + v).A;

                    if (alpha > 0)
                        return false;
                }
            }

            return true;
        }
    }
}
