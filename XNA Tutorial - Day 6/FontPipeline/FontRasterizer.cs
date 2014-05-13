#region File Description
//-----------------------------------------------------------------------------
// FontRasterizer.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// XNA and System.Drawing both define a Color type, so we alias these
// to avoid a conflict.
using GdiColor = System.Drawing.Color;
using XnaColor = Microsoft.Xna.Framework.Graphics.Color;

#endregion

namespace FontPipeline
{
    /// <summary>
    /// Helper class for rendering out TrueType font data
    /// using functions from System.Drawing.
    /// </summary>
    public sealed class FontRasterizer : IDisposable
    {
        #region Fields

        Font font;
        StringFormat format;
        Brush brush;
        Graphics graphics;
        Bitmap bitmap;

        int spacing;

        #endregion


        /// <summary>
        /// Constructs a new font rasterizer.
        /// </summary>
        public FontRasterizer(FontDescription description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            spacing = description.Spacing;

            try
            {
                font = new Font(description.Name, description.Size, description.Style);

                format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;

                brush = new SolidBrush(GdiColor.White);

                CreateTemporaryBitmap(1, 1);
            }
            catch
            {
                Dispose();
                throw;
            }
        }


        /// <summary>
        /// Cleans up temporary resources when we are finished.
        /// </summary>
        public void Dispose()
        {
            if (font != null)
                font.Dispose();

            if (format != null)
                format.Dispose();

            if (brush != null)
                brush.Dispose();

            if (graphics != null)
                graphics.Dispose();

            if (bitmap != null)
                bitmap.Dispose();
        }


        /// <summary>
        /// Rasterizes a single character into an XNA format bitmap.
        /// </summary>
        public BitmapContent Rasterize(char ch)
        {
            string text = ch.ToString();

            // Measure the size of this character.
            SizeF size = graphics.MeasureString(text, font);

            int width = (int)Math.Ceiling(size.Width);
            int height = (int)Math.Ceiling(size.Height);

            // If it is bigger than our current output bitmap,
            // allocate a larger bitmap.
            if ((width > bitmap.Width) || (height > bitmap.Height))
            {
                graphics.Dispose();
                bitmap.Dispose();

                CreateTemporaryBitmap(width, height);
            }

            // Render the text.
            graphics.Clear(GdiColor.Black);
            graphics.DrawString(text, font, brush, 0, 0, format);
            graphics.Flush();

            return GdiToXna(width, height);
        }


        /// <summary>
        /// Instantiates the System.Drawing Bitmap and Graphics objects
        /// needed for rendering out a font character.
        /// </summary>
        void CreateTemporaryBitmap(int width, int height)
        {
            bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            graphics = Graphics.FromImage(bitmap);
        }


        /// <summary>
        /// Converts a System.Drawing output bitmap to XNA format.
        /// </summary>
        BitmapContent GdiToXna(int width, int height)
        {
            int left = 0;
            int right = width - 1;

            // Windows is smart about kerning characters. We aren't. Therefore we
            // just crop out any redundant padding that GDI may have put around
            // the character, replacing it with a user configurable constant value.
            while ((left < right) && (ColumnIsEmpty(left, height)))
                left++;

            while ((right > left) && (ColumnIsEmpty(right, height)))
                right--;

            // Don't let space characters get cropped right down to zero though!
            if (right <= left)
            {
                left = 0;
                right = width - 1;
            }

            // Create the output XNA bitmap.
            int paddedWidth = right - left + 1 + spacing;

            PixelBitmapContent<XnaColor> output;
            output = new PixelBitmapContent<XnaColor>(paddedWidth, height);

            // Convert the pixel values.
            for (int y = 0; y < height; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    byte value = bitmap.GetPixel(x, y).R;

                    XnaColor color = new XnaColor(255, 255, 255, value);

                    output.SetPixel(x - left, y, color);
                }
            }

            return output;
        }


        /// <summary>
        /// Helper for cropping the left and right extents
        /// of the System.Drawing output image.
        /// </summary>
        bool ColumnIsEmpty(int x, int height)
        {
            for (int y = 0; y < height; y++)
            {
                if (bitmap.GetPixel(x, y).R != 0)
                    return false;
            }

            return true;
        }
    }
}
