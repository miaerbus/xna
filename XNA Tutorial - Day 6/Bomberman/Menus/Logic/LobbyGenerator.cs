using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    public class LobbyGenerator
    {
        static BuildingSystem blockSystem;

        static LobbyGenerator()
        {
            // Block system
            blockSystem = new BuildingSystem();
            Quaternion rotate0 = Quaternion.Identity;
            Quaternion rotate90 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.PiOver2);
            Quaternion rotate180 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.Pi);
            Quaternion rotate270 = Quaternion.CreateFromAxisAngle(Vector3.Down, MathHelper.PiOver2 * 3);

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.N,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.W,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
                BuildingSystem.GridSide.E,
                new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.S,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));
            
            blockSystem.Templates.Add(
                BuildingSystem.GridSide.N | BuildingSystem.GridSide.E,
                new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.S,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
                BuildingSystem.GridSide.N | BuildingSystem.GridSide.W,
                new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.S | BuildingSystem.GridSide.E,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E | BuildingSystem.GridSide.N,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.E | BuildingSystem.GridSide.S,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.W | BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));

            blockSystem.Templates.Add(
               BuildingSystem.GridSide.E | BuildingSystem.GridSide.N | BuildingSystem.GridSide.S,
               new Building(BuildingType.Blocker, Vector3.Zero, rotate0));
        }

        public static Level Generate(TriggerAction exitAction, TriggerAction playAction)
        {
            Level level = new Level();
            int size = 20;
            level.Size = size;

            bool[,] blockers = new bool[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    blockers[x, z] = true;
                }
            }

            float gridSize = Bomberman.GridSize;
            float halfGridSize = gridSize * 0.5f;
            Vector3 position = Vector3.Zero;
            Quaternion rotation;

            int center = level.Size / 2;
            int lobbyX = 4;
            int lobbyZ = 3;
            int exit = 4;
            int joinX = center + lobbyX + 1;

            // Floor
            PlaceFloor(center - lobbyX, center + lobbyX, center - lobbyZ, center + lobbyZ, blockers);
            PlaceFloor(center, center, center + lobbyZ, center + lobbyZ + exit, blockers);
            PlaceFloor(center - lobbyX - 1, center - lobbyX - 1, center, center, blockers);
            PlaceFloor(center - lobbyX + 2, center - lobbyX + 2, center - lobbyZ - 1, center - lobbyZ - 1, blockers);

            // Join doors
            int ip = 0;
            for (int i = center - lobbyZ; i <= center + lobbyZ; i += 2)
            {
                PlaceFloor(joinX, joinX, i, i, blockers);
                position.X = joinX * gridSize;
                position.Z = i * gridSize;
                Door door = new Door(position, Quaternion.Identity, Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2));
                position.X = joinX * gridSize - halfGridSize;
                position.Z = i * gridSize + halfGridSize;
                Trigger ipEnter = new Trigger(position, gridSize * 0.3f, null, false);
                ipEnter.Tag = ip;
                ip++;
                level.Scene.Add(door);
                level.Scene.Add(ipEnter);
                position.X = joinX * gridSize + halfGridSize;
                position.Z = i * gridSize + halfGridSize;
                level.Scene.Add(new Building(BuildingType.LobbyDoorFrame, position, Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2)));
            }

            // Host door
            position.X = (center - lobbyX) *gridSize;
            position.Z = (center + 1) * gridSize;
            Door hostdoor = new Door(position, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2));
            level.Scene.Add(hostdoor);
            
            position.X = (center - lobbyX) * gridSize + halfGridSize;
            position.Z = center * gridSize + halfGridSize;
            Trigger hostOpener = new Trigger(position, gridSize * 0.3f, hostdoor.OpenOrClose, false);
            level.Scene.Add(hostOpener);
            
            position.X = (center - lobbyX - 1) * gridSize + halfGridSize;
            position.Z = center * gridSize + halfGridSize;
            level.Scene.Add(new Building(BuildingType.LobbyDoorFrame, position, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2)));
            
            position.X = (center - lobbyX) * gridSize + halfGridSize;
            position.Z = (center) * gridSize + halfGridSize;
            level.Scene.Add(new Building(BuildingType.LobbyHost, position, Quaternion.CreateFromAxisAngle(Vector3.Up,MathHelper.PiOver2)));


            // Exit
            position.X = center * gridSize + halfGridSize;
            position.Z = (center + lobbyZ + 1) * gridSize + halfGridSize;
            level.Scene.Add(new Building(BuildingType.LobbyExit, position, Quaternion.Identity));

            position.X = center * gridSize + halfGridSize;
            position.Z = (center + lobbyZ + 3) * gridSize;
            Trigger exitTrigger = new Trigger(position, gridSize * 0.3f, exitAction, true);
            level.Scene.Add(exitTrigger);


            // Play door
            position.X = (center - lobbyX + 2) * gridSize;
            position.Z = (center - lobbyZ) * gridSize;
            Door playdoor = new Door(position, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2), Quaternion.Identity);
            level.Scene.Add(playdoor);

            position.X = (center - lobbyX + 2) * gridSize + halfGridSize;
            position.Z = (center - lobbyZ) * gridSize + halfGridSize;
            Trigger playOpener = new Trigger(position, gridSize * 0.3f, playdoor.OpenOrClose, false);
            level.Scene.Add(playOpener);

            position.X = (center - lobbyX + 2) * gridSize + halfGridSize;
            position.Z = (center - lobbyZ) * gridSize - halfGridSize;
            level.Scene.Add(new Building(BuildingType.LobbyDoorFrame, position, Quaternion.Identity));
            level.Scene.Add(new Trigger(position, gridSize * 0.3f, playAction, true));

            position.X = (center - lobbyX + 2) * gridSize + halfGridSize;
            position.Z = (center - lobbyZ) * gridSize + halfGridSize;
            level.Scene.Add(new Building(BuildingType.LobbyPlay, position, Quaternion.Identity));

            position.X = (center - lobbyX + 2) * gridSize + halfGridSize;
            position.Z = (center - lobbyZ) * gridSize - halfGridSize;



            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (!blockers[x, z])
                    {
                        position.X = x * gridSize + halfGridSize;
                        position.Z = z * gridSize + halfGridSize;
                        rotation = Quaternion.CreateFromAxisAngle(Vector3.Down, (float)Bomberman.Random.Next(0, 4) * MathHelper.PiOver2);
                        BuildingType type = (BuildingType)Bomberman.Random.Next(1, 10);
                        level.Scene.Add(new Building(BuildingType.LobbyFloor, position, rotation));
                    }
                }
            }

            blockSystem.Generate(blockers, level.Scene);

            level.SetLobbyCamera();
            return level;
        }

        static void PlaceFloor(int x1, int x2, int z1, int z2, bool[,] blockers)
        {
            for (int x = x1; x <= x2; x++)
            {
                for (int z = z1; z <= z2; z++)
                {
                    blockers[x, z] = false;
                }
            }
        }
    }
}
