using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    partial class Bomberman
    {
        public static Random Random = new Random();
        public static GraphicsDevice Device;
        public static DebugOutput Debug;

        public static Level CurrentLevel;
        public static float GridSize = 10;

        public static float StartSpeed = 20;
        public static float BombFuseTime = 4;
        public static float ObstacleDensity = 0.7f;

    }
}
