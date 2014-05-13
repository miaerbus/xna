using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Artificial.XNATutorial.CarGame
{
    class ABTRenderer : GameComponent
    {
        // Graphics
        PrimitiveBatch primitiveBatch;
        GraphicsDevice device;

        // Colors
        Color culledColor = Color.Black;
        Color visibleColor = Color.Gold;

        // ABT
        AdaptiveBinaryTree abt;

        // PROPERTIES
        Vector2 treeTop = new Vector2(0.65f, 0.9f);
        public Vector2 TreeTop
        {
            get
            {
                return treeTop;
            }
            set
            {
                treeTop = value;
            }
        }

        float treeWidth = 0.5f;
        public float TreeWidth
        {
            get
            {
                return treeWidth;
            }
            set
            {
                treeWidth = value;
            }
        }

        float levelHeight = 0.1f;
        public float LevelHeight
        {
            get
            {
                return levelHeight;
            }
            set
            {
                levelHeight = value;
            }
        }

        bool renderVolumes = false;
        public bool RenderVolumes
        {
            get
            {
                return renderVolumes;
            }
            set
            {
                renderVolumes = value;
            }
        }


        // CONSTRUCTOR
        public ABTRenderer(Game game, AdaptiveBinaryTree abt)
            : base(game)
        {
            this.abt = abt;
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();
            primitiveBatch = (PrimitiveBatch)Game.Services.GetService(typeof(PrimitiveBatch));
            device = primitiveBatch.GraphicsDevice;
        }

        // RENDER
        public void Render(Camera camera)
        {
            Render(camera, camera);
        }
        
        public void Render(Camera realCamera, Camera sceneCamera)
        {
            ABTNode root = abt.GetCopy();

            primitiveBatch.Begin(realCamera.ViewMatrix, realCamera.ProjectionMatrix);
            // Render the graphical representation of the tree and bounding volumes
            RenderBranch(root, treeTop, new Vector2(TreeWidth / 4f, 0), new Vector2(0, -levelHeight), primitiveBatch, sceneCamera,255);
            // Draw scene camera's frustum
            primitiveBatch.DrawBoxWireframe(sceneCamera.Frustum.GetCorners(), 0, Color.Red);
            primitiveBatch.End();
        }

        // RENDER BRANCH
        void RenderBranch(ABTNode root, Vector2 top, Vector2 width, Vector2 height, PrimitiveBatch primitiveBatch, Camera camera, float grayShade)
        {
            height.Y = Math.Min(height.Y, -0.01f);

            // Calculate if this node is visible for proper coloring
            bool visible = root.volume.Intersects(camera.Frustum);
            Color c = visible ? visibleColor : culledColor;

            primitiveBatch.DrawPoint(top, 0, 5, c);

            if (root.isLeaf)
            {
                LinkedListNode<IABTItem> item = root.items.First;
                for (int i = 0; i < root.items.Count; i++)
                {
                    // Draw points on the graphical representation
                    primitiveBatch.DrawPoint(top - Vector2.UnitY * i * 0.01f, 0, 1, c);
                    // Render bounding volums of items
                    if (renderVolumes && visible)
                    {
                        primitiveBatch.DrawBoxWireframe(item.Value.Position, item.Value.BoundingBox.GetCorners(), 1, Color.Black);
                    }
                    item = item.Next;
                }
            }
            else
            {
                // Draw lines to the children on the graphical representation
                Vector2 left = top - width + height;
                Vector2 right = top + width + height;
                Color cl = root.child[0].volume.Intersects(camera.Frustum) ? visibleColor : culledColor;
                Color cr = root.child[1].volume.Intersects(camera.Frustum) ? visibleColor : culledColor;
                primitiveBatch.DrawLine(top, left,0, 1, cl);
                primitiveBatch.DrawLine(top, right,0, 1, cr);

                // Render both child branches
                RenderBranch(root.child[0], left, width / 2f, height * 0.8f, primitiveBatch, camera, grayShade * 0.7f);
                RenderBranch(root.child[1], right, width / 2f, height * 0.8f, primitiveBatch, camera, grayShade * 0.7f);
            }

            // Render bounding volume of the current node
            if (renderVolumes)
            {
                byte b = (byte)grayShade;
                primitiveBatch.DrawBoxWireframe(root.volume.GetCorners(), 1, new Color(255,255,255,b));
            }

        }
    }
}
