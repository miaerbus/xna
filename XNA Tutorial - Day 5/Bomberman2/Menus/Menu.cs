using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.GUI;
using Artificial.XNATutorial.Physics;
using FontRenderer;

namespace Artificial.XNATutorial.Bomberman
{
    public class Menu : Part
    {
        // Gameplay component
        public Scene Scene;
        public MousePointer Mouse;

        // Renderer
        MenuRenderer renderer;
        public Font Font
        {
            get
            {
                return renderer.Font;
            }
        }

        // Simulator
        Simulator simulator;
        PhysicsRenderer physicsRenderer;

        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);

            parent.Require<State>();

            // Prepare simulation engine
            simulator = new Simulator();
            Scene = new Scene();
            simulator.Scene = Scene;
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleConvexCollisions(1));

            // Create main renderer
            renderer = new MenuRenderer(Bomberman.Game, Scene);
            renderer.DrawOrder = 0;
            Bomberman.Game.Components.Add(renderer);

            Camera menuCamera = new Camera();
            menuCamera.Direction = Vector3.Down;
            menuCamera.UpDirection = Vector3.Forward;
            menuCamera.Distance = 300;
            menuCamera.AspectRatio = (float)Bomberman.Device.Viewport.Width / (float)Bomberman.Device.Viewport.Height;
            menuCamera.NearPlane = 0.1f;
            menuCamera.FarPlane = 1000;
            menuCamera.FieldOfView = MathHelper.PiOver2;
            menuCamera.Target = new Vector3(400, 0, 300);

            physicsRenderer = new PhysicsRenderer(Bomberman.Game, Scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = menuCamera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            Bomberman.Game.Components.Add(physicsRenderer);

            float aspectRatio = (float)Bomberman.Device.Viewport.Width / (float)Bomberman.Device.Viewport.Height;
            float scale = 600f / (float)Bomberman.Device.Viewport.Height;
            float offsetX = Bomberman.Device.Viewport.Width - (float)Bomberman.Device.Viewport.Height * 1.33333f;

            // Prepare scene
            Mouse = new MousePointer(scale, new Vector2(-offsetX / 2, 0));
            Scene.Add(Mouse);
        }

        public override void Uninstall()
        {
            Bomberman.Game.Components.Remove(renderer);
            Bomberman.Game.Components.Remove(physicsRenderer);
        }

        public void Update(float dt)
        {
            simulator.Simulate(dt);
        }
        
    }
}
