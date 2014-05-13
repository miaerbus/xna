using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    public class LevelGenerator
    {
        static BuildingSystem roadSystem;

        static LevelGenerator()
        {
            // Create road system
            roadSystem = new BuildingSystem();
            Quaternion rotate0 = Quaternion.Identity;
            Quaternion rotate90 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.PiOver2);
            Quaternion rotate180 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.Pi);
            Quaternion rotate270 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.PiOver2 * 3);

            roadSystem.Templates.Add(
                BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
                new Building(BuildingType.RoadI, Vector3.Zero, rotate90));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E,
               new Building(BuildingType.RoadI, Vector3.Zero, rotate0));

            roadSystem.Templates.Add(
                BuildingSystem.GridSide.N | BuildingSystem.GridSide.E,
                new Building(BuildingType.RoadL, Vector3.Zero, rotate90));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.S,
               new Building(BuildingType.RoadL, Vector3.Zero, rotate270));

            roadSystem.Templates.Add(
                BuildingSystem.GridSide.N | BuildingSystem.GridSide.W,
                new Building(BuildingType.RoadL, Vector3.Zero, rotate0));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.S | BuildingSystem.GridSide.E,
               new Building(BuildingType.RoadL, Vector3.Zero, rotate180));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E | BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
               new Building(BuildingType.RoadX, Vector3.Zero, rotate0));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E | BuildingSystem.GridSide.N,
               new Building(BuildingType.RoadT, Vector3.Zero, rotate180));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E | BuildingSystem.GridSide.S,
               new Building(BuildingType.RoadT, Vector3.Zero, rotate0));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.W  | BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
               new Building(BuildingType.RoadT, Vector3.Zero, rotate90));

            roadSystem.Templates.Add(
               BuildingSystem.GridSide.E | BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
               new Building(BuildingType.RoadT, Vector3.Zero, rotate270));
        }

        public static Level Generate(int size)
        {
            Level level = new Level();
            level.Size = size;

            bool[,] roads = new bool[size, size];

            float gridSize = Bomberman.GridSize;
            float halfGridSize = gridSize * 0.5f;
            Vector3 position = Vector3.Zero;
            Quaternion rotation;

            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    position.X = x * gridSize + halfGridSize;
                    position.Z = z * gridSize + halfGridSize;
                    rotation = Quaternion.CreateFromAxisAngle(Vector3.Down, (float)Bomberman.Random.Next(0, 4) * MathHelper.PiOver2);
                    bool building = true;
                    if (x > 0 && x < size - 1 && z > 0 && z < size - 1)
                    {
                        building = (x % 2 == 0 && z % 2 == 0);
                    }
                    if (building)
                    {
                        BuildingType type = (BuildingType)Bomberman.Random.Next(1, 10);
                        level.Scene.Add(new Building(type, position, rotation));
                    }
                    else
                    {
                        roads[x, z] = true;
                        if (!(x < 3 && z < 3 || x > size - 4 && z > size - 4))
                        {
                            if (Bomberman.Random.NextDouble() < Bomberman.ObstacleDensity)
                            {
                                level.Scene.Add(new Building((BuildingType)(200 + Bomberman.Random.Next(1, 3)), position, rotation));
                            }
                        }
                    }
                }
            }

            roadSystem.Generate(roads, level.Scene);

            level.SetTopDownCamera();
            level.Start[PlayerIndex.One] = new Vector3(size * gridSize - halfGridSize * 3, halfGridSize, size * gridSize - halfGridSize * 3);
            level.Start[PlayerIndex.Two] = new Vector3(halfGridSize * 3, halfGridSize, halfGridSize * 3);
            return level;
        }
    }
}
