#region File Description
//-----------------------------------------------------------------------------
// FontWriter.cs
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
#endregion

namespace FontPipeline
{
    /// <summary>
    /// Content pipeline support class for saving out Font objects.
    /// </summary>
    [ContentTypeWriter]
    public class FontWriter : ContentTypeWriter<FontContent>
    {
        /// <summary>
        /// Saves font data into an XNB file.
        /// </summary>
        protected override void Write(ContentWriter output, FontContent value)
        {
            output.WriteObject(value.Texture);
            output.WriteObject(value.Glyphs);
            output.WriteObject(value.Cropping);
            output.WriteObject(value.CharacterMap);
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the font data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FontRenderer.FontReader, FontRenderer, " +
                "Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what CLR type the font
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "FontRenderer.Font, FontRenderer, Version=1.0.0.0, Culture=neutral"; 
        }
    }
}
