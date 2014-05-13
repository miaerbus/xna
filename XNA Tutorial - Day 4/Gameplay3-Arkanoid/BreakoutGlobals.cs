using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial.Breakout
{
    partial class Breakout
    {
        public static Random Random = new Random();

        public static int StartLives = 2;
        public static int Lives = StartLives;

        public static int Level = 1;
        public static int BricksLeft = 0;

        public static float[] StartSpeed = new float[] { 500, 350, 350 };

        public static float MaxAngleOnHit = 2f;
        public static float SpeedupOnHit = 1.01f;
        public static float PowerupFallSpeed = 80f;
        public static float PowerupChance = 0.08f;
    }
}
