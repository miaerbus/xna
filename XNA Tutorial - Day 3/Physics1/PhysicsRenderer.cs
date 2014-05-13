using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    class PhysicsRenderer : GameComponent
    {
        // Graphics
        GraphicsDevice device;
        ContentManager content;
        PrimitiveBatch primitiveBatch;
        TextBatch textBatch;

        // Scene and ABTs
        Scene scene;
        
        AdaptiveBinaryTree sceneABT;
        public AdaptiveBinaryTree SceneABT
        {
            get
            {
                return sceneABT;
            }
        }
        Dictionary<object, IABTItem> ABTMap = new Dictionary<object, IABTItem>();
        List<ABTLinkedList> visibleItems = new List<ABTLinkedList>();

        // Simulator
        PhysicsSimulator simulator;

        // Render properties
        int numSettings = 4;
        string[] settingName = { "velocity", "acceleration", "gravity field", "particle radius" };

        Color positionColor = Color.Gold;
        float massFactor = 0.1f;

        bool renderVelocity = false;
        float velocityFactor = 0.5f;
        Color velocityColor = Color.White;

        bool renderAcceleration = false;
        float accelerationFactor = 10f;
        Color accelerationColor = Color.Yellow;

        bool renderGravityField = false;
        Color gravityFieldColor = Color.Red;

        bool renderParticleRadius = true;
        Color particleRadiusColor = Color.White;
        

        // CONSTRUCTOR
        public PhysicsRenderer(Game game, Scene scene, PhysicsSimulator simulator)
            : base(game)
        {
            // Add scene handlers
            this.scene = scene;
            scene.OnItemAdd += sceneItemAdd;
            scene.OnItemRemove += sceneItemRemoved;

            // Create ABTs
            sceneABT = new AdaptiveBinaryTree(32);

            // Get simulator for querying its properties
            this.simulator = simulator;
        }


        // INITIALIZE
        public override void Initialize()
        {
            // Graphics
            base.Initialize();
            content = new ContentManager(Game.Services);
            device = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            primitiveBatch = new PrimitiveBatch(device);
            textBatch = new TextBatch(Game);
            Game.Components.Add(textBatch);
        }


        // UPDATE
        MouseState mouseOld;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton==ButtonState.Pressed && mouseOld.LeftButton==ButtonState.Released && mouse.X > 10 && mouse.X < 20 && mouse.Y > 10 && mouse.Y < 10 + 10 * numSettings)
            {
                int setting = (mouse.Y - 10) / 10;
                switch (setting)
                {
                    case 0:
                        renderVelocity = !renderVelocity;
                        break;
                    case 1:
                        renderAcceleration = !renderAcceleration;
                        break;
                    case 2:
                        renderGravityField = !renderGravityField;
                        break;
                    case 3:
                        renderParticleRadius = !renderParticleRadius;
                        break;
                }
            }
            mouseOld = mouse;
        }

        // RENDER
        public void Render(Camera camera)
        {
            primitiveBatch.Begin(camera.ViewMatrix, camera.ProjectionMatrix);
            textBatch.Begin(camera.ViewMatrix, camera.ProjectionMatrix);

            // Render GUI
            StringBuilder s;
            float width = device.Viewport.Width;
            float height = device.Viewport.Height;
            float wf = 2/(float)device.Viewport.Width;
            float hf = -2/(float)device.Viewport.Height;
            for (int i = 0; i < numSettings; i++)
            {
                primitiveBatch.DrawRectangle(new Vector2(10 * wf - 1, (10 + i * 10) * hf + 1), new Vector2(18 * wf - 1, (18 + i * 10) * hf + 1), 0, 1, Color.White);
                s = textBatch.GetNextScreenText();
                s.Append(settingName[i]);
                textBatch.Write(new Vector2(22, 12 + i * 10), 1, Color.White, false);
                bool settingOn=false;
                switch (i)
                {
                    case 0:
                        settingOn = renderVelocity;
                        break;
                    case 1:
                        settingOn = renderAcceleration;
                        break;
                    case 2:
                        settingOn = renderGravityField;
                        break;
                    case 3:
                        settingOn = renderParticleRadius;
                        break;
                }
                if (settingOn)
                {
                    primitiveBatch.DrawLine(new Vector2(10 * wf - 1, (10 + i * 10) * hf + 1), new Vector2(18 * wf - 1, (18 + i * 10) * hf + 1), 0, 1, Color.White);
                    primitiveBatch.DrawLine(new Vector2(18 * wf - 1, (10 + i * 10) * hf + 1), new Vector2(10 * wf - 1, (18 + i * 10) * hf + 1), 0, 1, Color.White);
                }

            }


            // Render scale
            int scaleSize = 100;
            Vector4 position4D1 = Vector4.Transform(Vector3.Zero, camera.ViewMatrix * camera.ProjectionMatrix);
            Vector4 position4D2 = Vector4.Transform(Vector3.Right, camera.ViewMatrix * camera.ProjectionMatrix);
            position4D1 /= position4D1.W;
            position4D2 /= position4D2.W;
            float meter = (position4D1 - position4D2).Length() / 2f * width;
            primitiveBatch.DrawLine(new Vector2(10 * wf - 1, (height - 10) * hf + 1), new Vector2((10 + scaleSize) * wf - 1, (height - 10) * hf + 1), 0, 1, Color.White);
            float unit = meter;
            int factor = 1;
            while (unit < 10)
            {
                factor *= 10;
                unit = meter * factor;
            }
            int m = 0;
            while (m * unit < scaleSize)
            {
                primitiveBatch.DrawLine(new Vector2((10 + m * unit) * wf - 1, (height - 10) * hf + 1), new Vector2((10 + m * unit) * wf - 1, (height - 20) * hf + 1), 0, 1, Color.White);
                s = textBatch.GetNextScreenText();
                s.Append(m);
                textBatch.Write(new Vector2(10 + m * unit, height - 28), 1, Color.White, true);
                m++;
            }
            s = textBatch.GetNextScreenText();
            s.Append("* "); s.Append(factor); s.Append(" meters");
            textBatch.Write(new Vector2(10 + scaleSize, height - 28), 1, Color.White, false);


            // Render scene items
            Vector3 v1;

            LinkedListNode<IABTItem> node;
            sceneABT.GetItems(camera.Frustum, visibleItems);
            for (int i = 0; i < visibleItems.Count; i++)
            {
                node = visibleItems[i].First;
                while (node != null)
                {
                    object item = (node.Value as CustomBoundingBoxABTItem).PositionPart;
                    IPosition itemPosition = item as IPosition;
                    IMass itemMass = item as IMass;
                    IMovable itemMovable = item as IMovable;
                    IForceUser itemForceUser = item as IForceUser;
                    IParticleRadius itemParticleRadius = item as IParticleRadius;

                    // acceleration
                    if (renderAcceleration && itemForceUser != null)
                    {
                        v1 = itemForceUser.Acceleration;
                        float p = v1.LengthSquared();
                        if (p > 0)
                        {
                            v1.Normalize();
                            p = (float)Math.Pow(p, 0.25);
                            primitiveBatch.DrawLine(itemForceUser.Position, itemForceUser.Position + v1 * p * accelerationFactor, 1, accelerationColor);
                        }
                    }

                    // gravity field
                    if (renderGravityField && itemForceUser != null)
                    {
                        if (itemForceUser.Mass > simulator.MinMass)
                        {
                            float Radius = (float)Math.Sqrt(simulator.GravitationalConstant * itemForceUser.Mass / simulator.MinAcceleration);
                            primitiveBatch.DrawCircle(itemForceUser.Position, Vector3.Backward, Vector3.Right * Radius, 64, 1, gravityFieldColor);
                        }
                    }
                    
                    // velocity
                    if (renderVelocity && itemMovable != null)
                    {
                        primitiveBatch.DrawLine(itemMovable.Position, itemMovable.Position + itemMovable.Velocity * velocityFactor, 1, velocityColor);
                    }

                    // position & mass
                    if (itemPosition != null)
                    {
                        int size = 5;
                        if (itemMass != null)
                        {
                            size = (int)Math.Log(itemMass.Mass * massFactor);
                            if (size < 2) size = 2;
                            if (size > 10) size = 10;
                        }

                        primitiveBatch.DrawPoint(itemPosition.Position, size, positionColor);

                        // particle radius
                        if (renderParticleRadius && itemParticleRadius != null)
                        {
                            primitiveBatch.DrawCircle(itemPosition.Position, Vector3.Backward, Vector3.Right * itemParticleRadius.ParticleRadius, 16, 1, particleRadiusColor);
                        }
                    }
                   
                    node = node.Next;
                }
            }

            textBatch.End();
            primitiveBatch.End();
        }


        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            IPositionWithEvents itemWithPosition = e.Item as IPositionWithEvents;
            if (itemWithPosition != null)
            {
                CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                i.PositionPart = itemWithPosition;
                i.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                sceneABT.Add(i);
                ABTMap.Add(itemWithPosition, i);
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            IABTItem item = ABTMap[e.Item];
            if (item != null)
            {
                sceneABT.Remove(item);
            }
        }
    }
}
