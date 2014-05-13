#region File Description
//-----------------------------------------------------------------------------
// FontContent.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Collections.Generic;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// This class holds font data in a format that can be used by the content
    /// pipeline. It is similar to the Font class from the runtime FontRender
    /// project, but leaves out all the rendering code, and stores the font
    /// texture as a content pipeline Texture2DContent object rather than the
    /// Graphics.Texture2D used by the runtime Font.
    /// </summary>
    public class FontContent
    {
        public Texture2DContent Texture = new Texture2DContent();
        public List<Rectangle> Glyphs = new List<Rectangle>();
        public List<Rectangle> Cropping = new List<Rectangle>();
        public Dictionary<char, int> CharacterMap = new Dictionary<char, int>();
    }
}
