using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class PrimitiveBatch
    {
        // Graphics
        GraphicsDevice device;
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return device;
            }
        }

        BasicEffect effect;
        VertexDeclaration vertexDeclaration;
        VertexBuffer vertexBuffer;
        int vertexBufferSize = 10000;
   
        // CACHE
        // an array of vertex lists, where the index is a list of different primitives
        const int numPools = 4;
        // 0 = points
        // 1 = transformed points
        // 2 = lines
        // 3 = transformed lines
        Dictionary<int, List<VertexPositionColor>>[] cache = new Dictionary<int, List<VertexPositionColor>>[numPools];

        // CONSTRUCTOR
        public PrimitiveBatch(GraphicsDevice device)
        {
            this.device = device;
            // Create the effect for rendering
            effect = new BasicEffect(device, null);
            // Instruct to use vertext color information
            effect.VertexColorEnabled = true;
            // Create caches for storing drawing requests
            for (int i = 0; i < numPools; i++)
            {
                cache[i] = new Dictionary<int, List<VertexPositionColor>>();
            }
            // Create vertex declaration for later use
            vertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
            // Initialize the vertex buffer on the graphics card
            vertexBuffer = new VertexBuffer(device, vertexBufferSize * VertexPositionColor.SizeInBytes, ResourceUsage.Points);
        }

        // BEGIN
        public void Begin(Matrix view, Matrix projection)
        {
            // Set the transformations for primitives in 3D space
            effect.View = view;
            effect.Projection = projection;
        }

        // DRAW A POINT

        // In world space
        public void DrawPoint(Vector3 position, int size, Color color)
        {
            if (size <= 1)
            {
                AddToCache(0, position, size, color);
            }
        }

        // In projection space
        public void DrawPoint(Vector2 position, float depth, int size, Color color)
        {
            AddToCache(1, new Vector3(position, depth), size, color);
        }

        // DRAW A LINE

        // In world space
        public void DrawLine(Vector3 start, Vector3 end, int width, Color color)
        {
            if (width <= 1)
            {
                AddToCache(2, start, 1, color);
                AddToCache(2, end, 1, color);
            }
        }

        // In projection space
        public void DrawLine(Vector2 start, Vector2 end, float depth, int width, Color color)
        {
            if (width <= 1)
            {
                AddToCache(3, new Vector3(start, depth), 1, color);
                AddToCache(3, new Vector3(end, depth), 1, color);
            }
        }

        // DRAW A WIREFRAME OF A BOX

        // In absolute world space
        public void DrawBoxWireframe(Vector3[] corners, int lineWidth, Color color)
        {
            DrawBoxWireframe(Vector3.Zero, corners, lineWidth, color);
        }

        // In relative world space with given offset
        public void DrawBoxWireframe(Vector3 offset, Vector3[] corners, int lineWidth, Color color)
        {
            for (int i = 0; i < 4; i++)
            {
                addBoxLine(offset, corners, lineWidth, color, i, (i + 1) % 4);
                addBoxLine(offset, corners, lineWidth, color, 4 + i, 4 + (i + 1) % 4);
                addBoxLine(offset, corners, lineWidth, color, i, i + 4);
            }
        }
        private void addBoxLine(Vector3 offset, Vector3[] corners, int lineWidth, Color color, int start, int end)
        {
            DrawLine(offset + corners[start], offset + corners[end], lineWidth, color);
        }

        // ADD TO CACHE
        void AddToCache(int pool, Vector3 position, int size, Color color)
        {
            List<VertexPositionColor> l;
            if (!cache[pool].ContainsKey(size))
            {
                l = new List<VertexPositionColor>();
                cache[pool].Add(size, l);
            }
            else
            {
                l = cache[pool][size];
            }
            l.Add(new VertexPositionColor(position, color));
        }


        // END
        public void End()
        {
            // Here we actually draw everything

            // Setup the device
            device.VertexDeclaration = vertexDeclaration;
            device.RenderState.DepthBufferEnable = false;

            // Draw primitives in world space
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            drawCache(0);
            drawCache(2);
          
            effect.CurrentTechnique.Passes[0].End();
            effect.End();

            // Vertices in projection space are already transformed
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.Identity;
            
            // Draw primitives in projection space
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            drawCache(1);
            drawCache(3);

            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }

        // DRAW PRIMITIVES IN CACHE
        private void drawCache(int pool)
        {
            // Loop thru all point sizes
            foreach (KeyValuePair<int, List<VertexPositionColor>> kvp in cache[pool])
            {
                // Load points into vertex buffer
                vertexBuffer.SetData<VertexPositionColor>(kvp.Value.ToArray());

                // Set the size of poits
                device.RenderState.PointSize = kvp.Key;

                // Set the vertex buffer to graphics device
                device.Vertices[0].SetSource(
                    vertexBuffer, 0,
                    VertexPositionColor.SizeInBytes);

                switch (pool)
                {
                    case 0:
                    case 1:
                        device.DrawPrimitives(PrimitiveType.PointList, 0, kvp.Value.Count);
                        break;
                    case 2:
                    case 3:
                        device.DrawPrimitives(PrimitiveType.LineList, 0, kvp.Value.Count / 2);
                        break;
                }
            }
            // Clear for next frame
            cache[pool].Clear();
        }
    }
}
