using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class StaticModel
    {
        // Normal matrix
        private Matrix normalMatrix = Matrix.Identity;
        public Matrix NormalMatrix
        {
            get
            {
                return normalMatrix;
            }
            set
            {
                normalMatrix = value;
            }
        }

        // 3D Model
        private Model model;
        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                boundingBox = BoundingBox.CreateFromSphere(model.Meshes[0].BoundingSphere);
                for (int j = 1; j < model.Meshes.Count; j++)
                {
                    BoundingBox newBox = BoundingBox.CreateFromSphere(model.Meshes[j].BoundingSphere);
                    Vector3.Min(ref boundingBox.Min, ref newBox.Min, out boundingBox.Min);
                    Vector3.Max(ref boundingBox.Max, ref newBox.Max, out boundingBox.Max);
                }
            }
        }

        // Bounding box
        private BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }
      
        // FRUSTUM CULLING
        public void FrustumCulling(Matrix[] worldMatrices, Camera camera, List<Matrix> visibleModels)
        {
            GraphicsDevice device = model.Meshes[0].MeshParts[0].Effect.GraphicsDevice;
            BoundingFrustum frustum = camera.Frustum;

            // Determine visible items
            visibleModels.Clear();
            for (int k = 0; k < worldMatrices.Length; k++)
            {
                bool visible = true;
                Matrix worldTransform = normalMatrix * worldMatrices[k];
                for (int j = 0; j < model.Meshes.Count; j++)
                {
                    mesh = model.Meshes[j];
                    // Calculate the bounding sphere of this mesh
                    BoundingSphere sphere;
                    sphere.Center = Vector3.Transform(mesh.BoundingSphere.Center, worldTransform);
                    Vector3 sphereSide = Vector3.Transform(mesh.BoundingSphere.Center + Vector3.UnitX * mesh.BoundingSphere.Radius, worldTransform);
                    float radius = (sphereSide - sphere.Center).Length();
                    sphere.Radius = radius;

                    // Only render if inside the frustum
                    bool intersects;
                    frustum.Intersects(ref sphere, out intersects);
                    if (!intersects) visible = false;
                }
                if (visible) visibleModels.Add(worldMatrices[k]);
            }
        }

        // RENDER
        static IEnumerator<EffectPass> passEnumerator;
        static Model renderModel;
        static ModelMesh mesh;
        static ModelMeshPart part;
        static Texture2D texture;

        public void Render(Matrix worldMatrix, BasicEffect effect)
        {
            GraphicsDevice device = effect.GraphicsDevice;
            passEnumerator = effect.CurrentTechnique.Passes.GetEnumerator();
            effect.World = normalMatrix * worldMatrix;

            effect.Texture = ((BasicEffect)model.Meshes[0].Effects[0]).Texture;
            effect.Begin();

            // Iterate through effect passes
            passEnumerator.Reset();
            while (passEnumerator.MoveNext())
            {
                passEnumerator.Current.Begin();


                    // Draw all meshes in model
                    for (int j = 0; j < model.Meshes.Count; j++)
                    {
                        mesh = model.Meshes[j];

                        // Draw all mesh parts in mesh
                        for (int i = 0; i < mesh.MeshParts.Count; i++)
                        {
                            part = mesh.MeshParts[i];
                            device.VertexDeclaration = part.VertexDeclaration;
                            device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                            device.Indices = mesh.IndexBuffer;
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                    }

                passEnumerator.Current.End();
            }

            effect.End();
        }

        public static void Render(List<Texture2D> textures, Dictionary<Texture2D, List<StaticModel>> models, Dictionary<Texture2D,Dictionary<StaticModel, List<Matrix>>> worldMatrices, BasicEffect effect)
        {
            GraphicsDevice device = effect.GraphicsDevice;
            EffectParameter world = effect.Parameters["World"];
            List<StaticModel> currentModels;
            List<Matrix> currentWorldMatrices;
            passEnumerator = effect.CurrentTechnique.Passes.GetEnumerator();

            // Calculate world matrices
            for (int m = 0; m < textures.Count; m++)
            {
                texture = textures[m];
                currentModels = models[texture];

                for (int l = 0; l < currentModels.Count; l++)
                {
                    currentWorldMatrices = worldMatrices[texture][currentModels[l]];
                    for (int k = 0; k < currentWorldMatrices.Count; k++)
                    {
                        currentWorldMatrices[k] = currentModels[l].normalMatrix * currentWorldMatrices[k];
                    }
                }
            }


            // Iterate through all textures
            for (int m = 0; m < textures.Count; m++)
            {
                texture = textures[m];
                currentModels = models[texture];

                effect.Texture = texture;
                effect.Begin();

                // Iterate through effect passes
                passEnumerator.Reset();
                while (passEnumerator.MoveNext())
                {
                    passEnumerator.Current.Begin();

                    // In each pass, draw all static models
                    for (int l = 0; l < currentModels.Count; l++)
                    {
                        renderModel = currentModels[l].model;
                        currentWorldMatrices = worldMatrices[texture][currentModels[l]];

                        // Draw all meshes in model
                        for (int j = 0; j < renderModel.Meshes.Count; j++)
                        {
                            mesh = renderModel.Meshes[j];

                            // Draw all mesh parts in mesh
                            for (int i = 0; i < mesh.MeshParts.Count; i++)
                            {
                                part = mesh.MeshParts[i];
                                device.VertexDeclaration = part.VertexDeclaration;
                                device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                                device.Indices = mesh.IndexBuffer;

                                // draw the mesh part for each model in list
                                for (int k = 0; k < currentWorldMatrices.Count; k++)
                                {
                                    world.SetValue(currentWorldMatrices[k]);
                                    effect.CommitChanges();
                                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                                }
                            }
                        }
                    }

                    passEnumerator.Current.End();
                }

                effect.End();
            }
        }
    }
}
