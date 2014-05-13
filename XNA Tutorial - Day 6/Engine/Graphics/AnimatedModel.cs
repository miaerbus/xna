using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace Artificial.XNATutorial
{
    public class AnimatedModel
    {
        // Normal matrix
        Matrix normalMatrix = Matrix.Identity;
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

        // Skinned 3D Model
        Model model;
        SkinningData skinningData;
        AnimationPlayer animationPlayer;
        AnimationClip clip;
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
                skinningData = model.Tag as SkinningData;
                animationPlayer = new AnimationPlayer(skinningData);
            }
        }        

        // Bounding box
        BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

        private string animation;
        public string Animation
        {
            get
            {
                return animation;
            }
            set
            {
                clip = skinningData.AnimationClips[value];
                animation = value;
                animationPlayer.StartClip(clip);
                animationEnded = false;
            }
        }

        private bool loopAnimation = true;
        public bool LoopAnimation
        {
            get
            {
                return loopAnimation;
            }
            set
            {
                loopAnimation = value;
            }
        }

        private bool animationEnded = false;
        public bool AnimationEnded
        {
            get
            {
                return animationEnded;
            }
        }

        // RENDER
        static IEnumerator<EffectPass> passEnumerator;
        static ModelMesh mesh;
        static ModelMeshPart part;

        public void Render(TimeSpan elapsedSpan, Matrix worldMatrix, Effect effect)
        {
            if (!loopAnimation && animationPlayer.CurrentTime.Add(elapsedSpan) >= clip.Duration)
            {
                animationEnded = true;
            }
            if (!animationEnded)
            {
                animationPlayer.Update(elapsedSpan, true, normalMatrix * worldMatrix);
            }

            GraphicsDevice device = effect.GraphicsDevice;
            passEnumerator = effect.CurrentTechnique.Passes.GetEnumerator();
            
            Matrix[] bones = animationPlayer.GetSkinTransforms();
            effect.Parameters["Bones"].SetValue(bones);
            
            effect.Parameters["Texture"].SetValue(model.Meshes[0].Effects[0].Parameters["Texture"].GetValueTexture2D());
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
    }
}
