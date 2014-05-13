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
        public static float GridSize = 10;
        public static float StartSpeed = 20;
        public static float BombFuseTime = 4;
        public static float ObstacleDensity = 0.7f;
        public static float TotalTime = 60.999f;

        public static Level Level;

        public static Random Random = new Random();
        public static DebugOutput Debug;

        public static GraphicsDevice Device;

        public static AudioEngine AudioEngine;
        public static WaveBank WaveBank;
        public static SoundBank SoundBank;

        public static List<Player> Players;
        public static Controller[] Controllers = new Controller[6];
        public static List<Player> ActivePlayers = new List<Player>();

        public static MenuRenderer MenuRenderer;
        public static WorldRenderer WorldRenderer;
        public static LoadingRenderer LoadingRenderer;

        public static bool MenuRendererLoaded;
        public static bool WorldRendererLoaded;

#if WINDOWS
        public static Multiplayer Multiplayer;
#endif

    }
}
