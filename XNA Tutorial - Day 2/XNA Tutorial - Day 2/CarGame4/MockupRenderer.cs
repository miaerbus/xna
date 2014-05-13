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
        Scene<object> scene; // This is the scene we are rendering. One renderer instance can handle
                             // only one scene, since it needs to prepare the ABT's in advance
        
        AdaptiveBinaryTree buildingsABT; // ABT for buildings, which are static objects
        
        AdaptiveBinaryTree carsABT; // ABT for cars, which always move and dictate different ABT settings.

        Dictionary<Car, ABTItem<Car>> carsABTMap = new Dictionary<Car, ABTItem<Car>>(); // Mapping of cars to
                                                                                        // their ABT objects so
                                                                                        // we can update them later
        // Sprites and models
        Object2D car;
        Object3D house;

        // CONSTRUCTOR
        public MockupRenderer(Game game, Scene<object> scene)
            : base(game)
        {
            // Add scene handlers
            this.scene = scene;
            scene.OnItemAdd += sceneItemAdd;

            // Create ABTs
            buildingsABT = new AdaptiveBinaryTree(16);
            carsABT = new AdaptiveBinaryTree(64);

            // Create sprites and models
            // We need to move this into the constructor since the bounding boxes need to be known
            // when items get inserted into the scene.
            car = new Object2D();
            car.NormalMatrix = Matrix.CreateScale(10f);
            car.BoundingBox = new BoundingBox(Vector3.One * -10, Vector3.One * 10);

            house = new Object3D();
            house.NormalMatrix = Matrix.CreateScale(0.125f);
            house.BoundingBox = new BoundingBox(Vector3.One * -10, Vector3.One * 10);

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

            // Render sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            device.RenderState.DepthBufferEnable = true;
            device.RenderState.DepthBufferWriteEnable = true;
           
            // Render cars
            List<IABTItem> visibleCars = carsABT.GetItems(camera.Frustum);
            for (int i = 0; i < visibleCars.Count; i++)
            {
                car.Render(spriteBatch, ((ABTItem<Car>)visibleCars[i]).Tag.WorldMatrix, camera);
            }

            spriteBatch.End();   
            
            // Render buildings
            List<IABTItem> visibleBuildings = buildingsABT.GetItems(camera.Frustum);
            for (int i = 0; i < visibleBuildings.Count; i++)
            {
                house.Render(((ABTItem<Building>)visibleBuildings[i]).Tag.WorldMatrix, camera);
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
            // A car's position has changed. Update the position of the according ABTitem
            // that represents this car. This will in turn inform the ABT of the change
            Car c = sender as Car;
            if (c != null)
            {
                carsABTMap[c].Position = e.Position;
            }
        }

    }
}
