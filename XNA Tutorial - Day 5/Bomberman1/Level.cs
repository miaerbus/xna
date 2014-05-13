using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Level
    {
        public Camera Camera;
        public Scene Scene;
        public int Size;

        public Dictionary<PlayerIndex, Vector3> Start = new Dictionary<PlayerIndex,Vector3>();

        public List<Character> Characters = new List<Character>();

        public Level()
        {
            // Create scene
            Scene = new Scene();
            Scene.Add(new CollisionPlane(new Plane(Vector3.Up, 0)));
            Scene.Add(new ConstantAcceleration(Vector3.Down * 98f));

            // Create camera
            Camera = new Camera();
        }

        public void AddCharacter(Character character)
        {
            Scene.Add(character);
            character.Part<PPosition>().Position = Start[character.Player];
            Characters.Add(character);
        }

        public void AddBomb(Bomb newBomb)
        {
            Scene.Add(newBomb);
            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].Part<CustomCollider>().OverrideCollisionWithItem.Add(newBomb);
            }
        }

        public void SetTopDownCamera()
        {
            Camera.Direction = Vector3.Down;
            Camera.UpDirection = Vector3.Forward;
            Camera.Distance = Size * 15;
            Camera.AspectRatio = (float)Bomberman.Device.Viewport.Width / (float)Bomberman.Device.Viewport.Height;
            Camera.NearPlane = 0.1f;
            Camera.FarPlane = 1000;
            Camera.FieldOfView = MathHelper.PiOver4;
            Camera.Target = new Vector3(1, 0, 1) * (float)Size / 2f * Bomberman.GridSize;
        }

        public void SetCimCityCamera()
        {
            Camera.Direction = Vector3.Forward * 4 + Vector3.Left + Vector3.Down * 8;
            Camera.UpDirection = Vector3.Up;
            Camera.Distance = Size * 150;
            Camera.AspectRatio = (float)Bomberman.Device.Viewport.Width / (float)Bomberman.Device.Viewport.Height;
            Camera.NearPlane = 1f;
            Camera.FarPlane = 10000;
            Camera.FieldOfView = MathHelper.PiOver4 * 0.1f;
            Camera.Target = new Vector3(1, 0, 1) * (float)Size / 2f * Bomberman.GridSize;
        }

    }
}
