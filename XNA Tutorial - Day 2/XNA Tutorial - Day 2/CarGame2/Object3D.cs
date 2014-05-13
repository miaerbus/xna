using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Object3D : WorldObject
    {
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

        // Render
        public override void Render(Camera camera)
        {
            Matrix worldTransform = NormalMatrix * WorldMatrix;

            foreach (ModelMesh mesh in model.Meshes)
            {
                // Calculate the bounding sphere of this mesh in world space. Each mesh already has
                // bounding sphere information but in it's object space. We transform the center and
                // side of the sphere to world space and now we can calculate the new radius.                
                BoundingSphere sphere;
                sphere.Center = Vector3.Transform(mesh.BoundingSphere.Center, worldTransform);
                Vector3 sphereSide = Vector3.Transform(mesh.BoundingSphere.Center + Vector3.UnitX * mesh.BoundingSphere.Radius, worldTransform);
                float radius = (sphereSide - sphere.Center).Length();
                sphere.Radius = radius;

                // Only render if inside the frustum
                // Here we check if the bounding sphere is inside the view frustum
                if (camera.Frustum.Intersects(sphere))
                {

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldTransform;
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }

                    mesh.Draw();
#if DEBUG
                    // Increase for debug output
                    Globals.MeshesRendered++;
#endif
                }
            }

        }

    }
}
