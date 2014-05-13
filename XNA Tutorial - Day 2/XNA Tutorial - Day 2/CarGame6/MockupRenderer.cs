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

        // Scene and ABTs
        Scene<object> scene;
        AdaptiveBinaryTree buildingsABT;
        AdaptiveBinaryTree carsABT;

        // Sprites and models
        Object2D car;
        Object3D house;
        Dictionary<object, CompoundABTItem> ABTMap = new Dictionary<object, CompoundABTItem>();

        // ABT Properties for debug rendering
        public AdaptiveBinaryTree BuildingsABT
        {
            get
            {
                return buildingsABT;
            }
        }
        public AdaptiveBinaryTree CarsABT
        {
            get
            {
                return carsABT;
            }
        }

        // CONSTRUCTOR
        public MockupRenderer(Game game, Scene<object> scene)
            : base(game)
        {
            // Add scene handlers
            this.scene = scene;
            //scene.OnItemAdd += new EventHandler<Scene<object>.SceneManipulationEventArgs<object>>(sceneItemAdd);
            //scene.OnItemRemove += new EventHandler<Scene<object>.SceneManipulationEventArgs<object>>(sceneItemRemoved);
            scene.OnItemAdd += sceneItemAdd;
            scene.OnItemRemove += sceneItemRemoved;

            // Create ABTs
            buildingsABT = new AdaptiveBinaryTree(16);
            carsABT = new AdaptiveBinaryTree(16);

            // Create sprites and models
            car = new Object2D();
            car.NormalMatrix = Matrix.CreateScale(10f);
            car.BoundingBox = new BoundingBox(Vector3.One * -4, Vector3.One * 4);

            house = new Object3D();
            house.NormalMatrix = Matrix.CreateScale(0.125f);
            house.BoundingBox = new BoundingBox(new Vector3(-6, -6, 0), new Vector3(6, 6, 12));

        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();
            content = (ContentManager)Game.Services.GetService(typeof(ContentManager));
            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            device = spriteBatch.GraphicsDevice;

            // Load content
            car.Texture = content.Load<Texture2D>(@"Content\car");
            house.Model = content.Load<Model>(@"Content\3dhouse");
        }

        // RENDER
        public void Render(Camera camera)
        {
            Render(camera, camera);
        }

        public void Render(Camera realCamera, Camera sceneCamera)
        {

            // Render sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.DepthBufferWriteEnable = true;

            // Render cars
            List<IABTItem> visibleCars = carsABT.GetItems(sceneCamera.Frustum);
            for (int i = 0; i < visibleCars.Count; i++)
            {
                car.Render(spriteBatch, ((Car)(((CompoundABTItem)visibleCars[i]).PositionPart)).WorldMatrix, realCamera, sceneCamera);
            }

            spriteBatch.End();   
            
            // Render buildings
            List<IABTItem> visibleBuildings = buildingsABT.GetItems(sceneCamera.Frustum);
            for (int i = 0; i < visibleBuildings.Count; i++)
            {
                house.Render(((Building)(((CompoundABTItem)visibleBuildings[i]).PositionPart)).WorldMatrix, realCamera, sceneCamera);
            }
        }

        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene<object>.SceneManipulationEventArgs<object> e)
        {
            // Update BuildingsABT
            Building b = e.Item as Building;
            if (b != null)
            {
                CompoundABTItem i = new CompoundABTItem();
                i.PositionPart = (IPositionWithEvents)b;
                i.SizePart = (IBoundingBox)house;
                buildingsABT.Add(i);
                ABTMap.Add(b, i);
            }

            // Update CarsABT
            Car c = e.Item as Car;
            if (c != null)
            {
                CompoundABTItem i = new CompoundABTItem();
                i.PositionPart = (IPositionWithEvents)c;
                i.SizePart = (IBoundingBox)car;
                carsABT.Add(i);
                ABTMap.Add(c, i);
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene<object>.SceneManipulationEventArgs<object> e)
        {
            // Update BuildingsABT
            Building b = e.Item as Building;
            if (b != null)
            {
                CompoundABTItem i = ABTMap[b];
                buildingsABT.Remove(i);                
            }

            // Update CarsABT
            Car c = e.Item as Car;
            if (c != null)
            {
                CompoundABTItem i = ABTMap[c];
                carsABT.Remove(i);
            }
        }    
    }
}
