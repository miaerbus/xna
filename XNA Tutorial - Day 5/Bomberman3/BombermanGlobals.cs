using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    partial class Bomberman
    {
        public static Random Random = new Random();
        public static DebugOutput Debug;

        public static GraphicsDevice Device;

        public static AudioEngine audioEngine;
        public static WaveBank waveBank;
        public static SoundBank soundBank;


    }
}
