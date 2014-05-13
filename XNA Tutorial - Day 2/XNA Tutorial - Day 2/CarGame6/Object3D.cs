using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Object3D : IBoundingBox
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
            set
            {
                boundingBox = value;
            }
        }
      
        // RENDER
        public void Render(Matrix worldMatrix, Camera realCamera, Camera sceneCamera)
        {
            Matrix world = normalMatrix * worldMatrix;

            foreach (ModelMesh mesh in model.Meshes)
            {
                // Calculate the bounding sphere of this mesh
                BoundingSphere sphere;
                sphere.Center = Vector3.Transform(mesh.BoundingSphere.Center, world);
                Vector3 sphereSide = Vector3.Transform(mesh.BoundingSphere.Center + Vector3.UnitX * mesh.BoundingSphere.Radius, world);
                float radius = (sphereSide - sphere.Center).Length();
                sphere.Radius = radius;

                // Only render if inside the frustum of the scene camera
                if (sceneCamera.Frustum.Intersects(sphere))
                {

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = world;
                        effect.View = realCamera.ViewMatrix;
                        effect.Projection = realCamera.ProjectionMatrix;
                    }

                    mesh.Draw();
#if DEBUG
                    Globals.MeshesRendered++;
#endif
                }
            }
        }
    }
}
