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
        Dictionary<Car, ABTItem<Car>> carsABTMap = new Dictionary<Car, ABTItem<Car>>();

        // Sprites and models
        Object2D car;
        Object3D house;

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
            scene.OnItemAdd += sceneItemAdd;

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

        // This time we introduce an interesting feature, of seeing the scene from a different perspective
        // than the default scene camera. In this way, we can see how different objects get culled and such.
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
                car.Render(spriteBatch, ((ABTItem<Car>)visibleCars[i]).Tag.WorldMatrix, realCamera, sceneCamera);
            }

            spriteBatch.End();   
            
            // Render buildings
            List<IABTItem> visibleBuildings = buildingsABT.GetItems(sceneCamera.Frustum);
            for (int i = 0; i < visibleBuildings.Count; i++)
            {
                house.Render(((ABTItem<Building>)visibleBuildings[i]).Tag.WorldMatrix, realCamera, sceneCamera);
            }
        }

        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene<object>.SceneManipulationEventArgs<object> e)
        {
            // Update BuildingsABT
            Building b = e.Item as Building;
            if (b != null)
            {
                ABTItem<Building> i = new ABTItem<Building>();
                i.Position = b.Position;
                i.BoundingBox = house.BoundingBox;
                i.Tag = b;
                buildingsABT.Add(i);
            }

            // Update CarsABT
            Car c = e.Item as Car;
            if (c != null)
            {
                ABTItem<Car> i = new ABTItem<Car>();
                i.Position = c.Position;
                i.BoundingBox = car.BoundingBox;
                i.Tag = c;
                carsABT.Add(i);
                carsABTMap.Add(c, i);
                c.OnPositionChanged += CarPositionChanged;
            }

        }    

        // UPDATE MOVING CARS
        void CarPositionChanged(object sender, Car.PositionEventArgs e)
        {
            Car c = sender as Car;
            if (c != null)
            {
                carsABTMap[c].Position = e.Position;
            }
        }

    }
}
