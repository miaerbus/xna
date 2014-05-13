using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class BuildingSystem
    {
        public enum GridSide : byte
        {
            N = 1,
            W = 2,
            S = 4,
            E = 8,
        }

        public Dictionary<GridSide, Building> Templates = new Dictionary<GridSide, Building>();

        public void Generate(bool[,] mask, Scene scene)
        {
            float gridSize = Bomberman.GridSize;
            float halfGridSize = gridSize * 0.5f;
            Vector3 position = Vector3.Zero;
            Quaternion rotation;

            for (int x = 1; x < mask.GetUpperBound(0); x++)
            {
                for (int z = 1; z < mask.GetUpperBound(1); z++)
                {
                    if (mask[x, z])
                    {
                        byte value = 0;
                        if (mask[x, z - 1]) value += (byte)GridSide.N;
                        if (mask[x + 1, z]) value += (byte)GridSide.E;
                        if (mask[x, z + 1]) value += (byte)GridSide.S;
                        if (mask[x - 1, z]) value += (byte)GridSide.W;

                        if (Templates.ContainsKey((GridSide)value))
                        {
                            Building template = Templates[(GridSide)value];
                            BuildingType type = template.Type;
                            position.X = x * gridSize + halfGridSize;
                            position.Z = z * gridSize + halfGridSize;
                            rotation = template.Part<PAngularPosition>().AngularPosition;
                            scene.Add(new Building(type, position, rotation));
                        }
                    }
                }
            }
        }
    }
}
