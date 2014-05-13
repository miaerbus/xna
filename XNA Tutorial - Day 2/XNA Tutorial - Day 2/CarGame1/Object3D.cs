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

        // RENDER
        public override void Render(Camera camera)
        {
            // 3D models are divided into meshes. Each mesh can have different material color
            // and textures defined so we need to render each individual mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                // Material and texture settings that the artist defined for this part of the
                // 3D model are already stored in an effect associated with this mesh. All we
                // have to do is set the world, view and projection matrices used to render
                // this mesh.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = NormalMatrix * WorldMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }
                // We let XNA do the rest.
                mesh.Draw();               
#if DEBUG
                // Increase for debug output
                Globals.MeshesRendered++;
#endif

            }

        }

    }
}
