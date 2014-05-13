using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Artificial.XNATutorial.CarGame
{
    class MockupRenderer : GameComponent
    {
        // Graphics
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        ContentManager content;

        // Sprites and models
        Object2D car;
        Object3D house;

        // CONSTRUCTOR
        // Here we get access to the game object and its services
        public MockupRenderer(Game Game)
            : base(Game)
        {
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            // Get shared graphics objects
            content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            device = spriteBatch.GraphicsDevice;

            // In our new architecture, the renderer controls how logical game objects get
            // represented on the screen. He is like a painter with certain knowledge of
            // drawing things. In our case, he will know how to render a car and a building,
            // using mockup textures and 3D models.

            // Create sprites and models
            car = new Object2D();
            car.NormalMatrix = Matrix.CreateScale(10f);
            car.Texture = content.Load<Texture2D>(@"Content\car");

            house = new Object3D();
            house.Model = content.Load<Model>(@"Content\3dhouse");
            house.NormalMatrix = Matrix.CreateScale(0.125f);
        }

        // RENDER
        public void Render(List<object> Scene, Camera Camera)
        {
            // Render sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.DepthBufferWriteEnable = true;

            // Render cars
            for (int i = 0; i < Scene.Count; i++)
            {
                Car c = Scene[i] as Car;
                if (c != null)
                {                    
                    car.Render(spriteBatch, c.WorldMatrix, Camera);
                }
            }

            spriteBatch.End();   
            
            // Render buildings
            for (int i = 0; i < Scene.Count; i++)
            {
                Building b = Scene[i] as Building;
                if (b != null)
                {
                    house.Render(b.WorldMatrix, Camera);
                }
            }

        }
    }
}
