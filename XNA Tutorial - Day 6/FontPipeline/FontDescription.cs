#region File Description
//-----------------------------------------------------------------------------
// FontDescription.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// The TrueType font processor needs to know which font it should rasterize,
    /// along with settings such as the font size and which Unicode characters
    /// should be included in the output. This class holds that information.
    /// 
    /// We could invent a new file format to hold these font descriptions, and
    /// write a custom importer to read them, but there is no need because we can
    /// just use the XML serializer to load this description data! XNA includes
    /// an importer for "XML Content", so to add a new TrueType font, just create
    /// an XML description file, add it to your project, set the
    /// "XNA Framework Content" property to True, then select the XML Content
    /// importer and TrueTypeFontProcessor.
    /// </summary>
    public class FontDescription
    {
        public string Name;
        public float Size;
        public FontStyle Style;
        public int Spacing;
        public Region[] Regions;


        /// <summary>
        /// Describes what part of the Unicode character set we want to rasterize.
        /// </summary>
        public class Region
        {
            public char Start;
            public char End;
        }
    }
}
