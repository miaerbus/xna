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
    public class PhysicsRenderer : DrawableGameComponent
    {
        // Graphics
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
        Dictionary<Item, IABTItem> ABTMap = new Dictionary<Item, IABTItem>();
        List<ABTLinkedList> visibleItems = new List<ABTLinkedList>();
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();

        // Simulator
        Simulator simulator;

        // Render properties
        int numSettings = 8;
        string[] settingName = { "velocity", "acceleration", "gravity field", "particle radius", "constraints & springs" , "convex hull" , "bounding sphere", "separating axis"};

        public Color positionColor = Color.Gold;
        public float massFactor = 0.1f;

        public bool renderVelocity = false;
        public float velocityFactor = 0.5f;
        public Color velocityColor = Color.White;

        public bool renderAcceleration = false;
        public float accelerationFactor = 0.5f;
        public Color accelerationColor = Color.Yellow;

        public bool renderGravityField = false;
        public Color gravityFieldColor = Color.Red;

        public bool renderParticleRadius = true;
        public Color particleRadiusColor = Color.White;

        public bool renderConstraintsSprings = true;
        public Color constraintColor = Color.LightCyan;
        public Color springColor = Color.Yellow;

        public bool renderConvexHull = true;
        public Color normalColor = Color.Red;
        public float normalLength = 0.2f;
        public Color polyEdgeColor = Color.White;

        public bool renderBoundingSphere = false;
        public Color boundingSphereColor = Color.Red;

        public bool renderSeparatingAxis = false;
        public Color axisCollideColor = Color.Lime;
        public Color axisDontCollideColor = Color.Green;


        // Camera
        private Camera camera;
        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }


        // CONSTRUCTOR
        public PhysicsRenderer(Game game, Scene scene, Simulator simulator)
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

            // Handle existing scene items
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                itemAdded(scene[indices[i]]);
            }
        }


        // INITIALIZE
        public override void Initialize()
        {
            // Graphics
            base.Initialize();
            content = new ContentManager(Game.Services);
            primitiveBatch = new PrimitiveBatch(GraphicsDevice);
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
                    case 4:
                        renderConstraintsSprings = !renderConstraintsSprings;
                        break;
                    case 5:
                        renderConvexHull = !renderConvexHull;
                        break;
                    case 6:
                        renderBoundingSphere = !renderBoundingSphere;
                        break;
                    case 7:
                        renderSeparatingAxis = !renderSeparatingAxis;
                        break;
                }
            }
            mouseOld = mouse;
        }

        // DRAW
        public override void  Draw(GameTime gameTime)
        {
 	        base.Draw(gameTime);

            primitiveBatch.Begin(camera.ViewMatrix, camera.ProjectionMatrix);
            textBatch.Begin(camera.ViewMatrix, camera.ProjectionMatrix);

            // Render GUI
            StringBuilder s;
            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;
            float wf = 2 / (float)GraphicsDevice.Viewport.Width;
            float hf = -2 / (float)GraphicsDevice.Viewport.Height;
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
                    case 4:
                        settingOn = renderConstraintsSprings;
                        break;
                    case 5:
                        settingOn = renderConvexHull;
                        break;
                    case 6:
                        settingOn = renderBoundingSphere;
                        break;
                    case 7:
                        settingOn = renderSeparatingAxis;
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
            float factor = 1;
            while (unit < 10)
            {
                factor *= 10;
                unit = meter * factor;
            }
            while (unit > 100)
            {
                factor /= 10;
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
            Vector3 v1, v2;
            Matrix m1 = Matrix.Identity;
           
            // Items we need to render always
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                object item = scene[indices[i]];
                Spring itemSpring = item as Spring;
                PositionConstraint itemConstraint = item as PositionConstraint;

                // constraints & springs
                if (renderConstraintsSprings)
                {
                    if (itemSpring != null)
                    {
                        v1 = itemSpring.Item1.Position;
                        v2 = itemSpring.Item2.Position;
                        primitiveBatch.DrawLine(v1, v2, 1, springColor);
                    }
                    if (itemConstraint != null)
                    {
                        v1 = itemConstraint.Item1.Position;
                        v2 = itemConstraint.Item2.Position;
                        primitiveBatch.DrawLine(v1, v2, 1, constraintColor);
                    }
                }
            }
 
            // Items optimized by ABT
            LinkedListNode<IABTItem> node;
            sceneABT.GetItems(camera.Frustum, visibleItems);
            for (int i = 0; i < visibleItems.Count; i++)
            {
                node = visibleItems[i].First;
                while (node != null)
                {
                    Item item = (node.Value as CustomBoundingBoxABTItem).PositionPart;
                    PPosition itemPosition = null;
                    PMass itemMass = null;
                    Movable itemMovable = null;
                    ForceUser itemForceUser = null;
                    PParticleRadius itemParticleRadius = null;
                    PConvex itemConvex = null;
                    Rotatable itemRotatable = null;
                    PBoundingSphere itemBoundingSphere = null;
                    ParticleCollider itemParticleCollider = null;
                    ConvexCollider itemRigidCollider = null;

                    if (item != null)
                    {
                        itemPosition = item.As<PPosition>();
                        itemMass = item.As<PMass>();
                        itemMovable = item.As<Movable>();
                        itemForceUser = item.As<ForceUser>();
                        itemParticleRadius = item.As<PParticleRadius>();
                        itemConvex = item.As<PConvex>();
                        itemRotatable = item.As<Rotatable>();
                        itemBoundingSphere = item.As<PBoundingSphere>();
                        itemParticleCollider = item.As<ParticleCollider>();
                        itemRigidCollider = item.As<ConvexCollider>();
                    }

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
                            primitiveBatch.DrawCircle(itemPosition.Position, Vector3.Up, Vector3.Right * itemParticleRadius.ParticleRadius, 16, 1, particleRadiusColor);
                            primitiveBatch.DrawCircle(itemPosition.Position, Vector3.Right, Vector3.Up * itemParticleRadius.ParticleRadius, 16, 1, particleRadiusColor);
                        }
                    }

                    // convex polygon
                    if (renderConvexHull && itemConvex != null && itemPosition != null)
                    {                       
                        ConvexPolygon poly = itemConvex.Convex as ConvexPolygon;
                        if (poly != null)
                        {
                            for (int k = 0; k < poly.Points.Count; k++)
                            {
                                int j = (k + 1) % poly.Points.Count;
                                v1 = poly.Points[j];
                                v2 = poly.Points[k];
                                if (itemRotatable != null)
                                {
                                    v1 = Vector3.Transform(v1, itemRotatable.Rotation);
                                    v2 = Vector3.Transform(v2, itemRotatable.Rotation);
                                }
                                v1 += itemPosition.Position;
                                v2 += itemPosition.Position;
                                primitiveBatch.DrawLine(v1, v2, 1, polyEdgeColor);
                            }
                        }

                        for (int k = 0; k < itemConvex.Convex.Planes.Count; k++)
                        {
                            Plane p = itemConvex.Convex.Planes[k];
                            v1 = p.Normal * p.D;
                            v2 = v1 + p.Normal * normalLength;
                            if (itemRotatable != null)
                            {
                                v1 = Vector3.Transform(v1, itemRotatable.Rotation);
                                v2 = Vector3.Transform(v2, itemRotatable.Rotation);
                            }
                            v1 += itemPosition.Position;
                            v2 += itemPosition.Position;
                            primitiveBatch.DrawLine(v1, v2, 1, normalColor);
                        }
                    }

                    // bounding sphere
                    if (renderBoundingSphere && itemBoundingSphere != null && itemPosition !=null)
                    {
                        primitiveBatch.DrawCircle(itemPosition.Position + itemBoundingSphere.BoundingSphere.Center, Vector3.Backward, Vector3.Right * itemBoundingSphere.BoundingSphere.Radius, 16, 1, boundingSphereColor);
                        primitiveBatch.DrawCircle(itemPosition.Position + itemBoundingSphere.BoundingSphere.Center, Vector3.Up, Vector3.Right * itemBoundingSphere.BoundingSphere.Radius, 16, 1, boundingSphereColor);
                        primitiveBatch.DrawCircle(itemPosition.Position + itemBoundingSphere.BoundingSphere.Center, Vector3.Right, Vector3.Up * itemBoundingSphere.BoundingSphere.Radius, 16, 1, boundingSphereColor);
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
            Item item = e.Item as Item;
            if (item != null)
            {
                itemAdded(item);
            }
        }

        void itemAdded(Item item)
        {
            PPositionWithEvents itemPosition = item.As<PPositionWithEvents>();
            PParticleRadius itemParticleRadius = item.As<PParticleRadius>();
            PBoundingSphere itemBoundingSphere = item.As<PBoundingSphere>();
            PBoundingBox itemBoundingBox = item.As<PBoundingBox>();

            if (itemPosition != null)
            {
                CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                i.PositionPart = item;
                if (itemParticleRadius != null)
                {
                    i.BoundingBox = new BoundingBox(itemParticleRadius.ParticleRadius * -Vector3.One, itemParticleRadius.ParticleRadius * Vector3.One);
                }
                else if (itemBoundingSphere != null)
                {
                    i.BoundingBox = BoundingBox.CreateFromSphere(itemBoundingSphere.BoundingSphere);
                }
                else if (itemBoundingBox != null)
                {
                    i.BoundingBox = itemBoundingBox.BoundingBox;
                }
                else
                {
                    i.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                }
                sceneABT.Add(i);
                ABTMap.Add(item, i);
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene.SceneManipulationEventArgs e)
        {
            if (ABTMap.ContainsKey(e.Item))
            {
                // Update sceneABT
                IABTItem item = ABTMap[e.Item];
                CustomBoundingBoxABTItem i = item as CustomBoundingBoxABTItem;
                if (item != null)
                {
                    sceneABT.Remove(item);
                    if (i != null)
                    {
                        i.PositionPart = null;
                    }
                }
            }
        }
    }
}
