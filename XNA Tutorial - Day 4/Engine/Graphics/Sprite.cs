using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial
{
    public class Sprite
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

        // Sprite image
        private Texture2D texture;
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                origin = new Vector2(texture.Width, texture.Height) / 2f;
            }
        }

        // Source rectangle
        private Rectangle sourceRectangle;
        public Rectangle SourceRectangle
        {
            get
            {
                return sourceRectangle;
            }
            set
            {
                sourceRectangle = value;
            }
        }

        // Origin
        private Vector2 origin;
        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        // Color
        private Color color = Color.White;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        // RENDER
        public void Render(SpriteBatch spriteBatch, Matrix worldMatrix, Camera camera, Color color)
        {
            GraphicsDevice device = spriteBatch.GraphicsDevice;

            // Create the transformation using the given camera variables and object matrices
            Matrix screen = Matrix.CreateTranslation(1, -1, 0) * Matrix.CreateScale(device.Viewport.Width / 2, -device.Viewport.Height / 2, 1);
            Matrix worldTransform = normalMatrix * worldMatrix;
            Matrix viewTransform = worldTransform * camera.ViewMatrix;
            Matrix screenTransform = camera.ProjectionMatrix * screen;

            // Calculate bounding sphere
            Vector3 sphereCenter = Vector3.Transform(Vector3.Zero, worldTransform);
            Vector3 sphereSide = Vector3.Transform(Vector3.One, worldTransform);
            float radius = (sphereSide - sphereCenter).Length();
            BoundingSphere spriteBoundingSphere = new BoundingSphere(sphereCenter, radius);

            // Frustum culling using the scene camera
            if (!camera.Frustum.Intersects(spriteBoundingSphere)) return;
        
            // Calculate the screen position and depth
            Vector3 viewposition = Vector3.Transform(Vector3.Zero, viewTransform);
            Vector4 position4D = Vector4.Transform(viewposition, screenTransform);
            position4D /= position4D.W;
            Vector2 position = new Vector2(position4D.X, position4D.Y);
            float depth = position4D.Z;

            // Calculate the scale
            Vector3 viewposition2 = Vector3.Transform(Vector3.UnitX, viewTransform);
            float size = (viewposition2 - viewposition).Length();
            position4D = Vector4.Transform(viewposition + Vector3.Right * size, screenTransform);
            position4D /= position4D.W;
            Vector2 position2 = new Vector2(position4D.X, position4D.Y);

            Vector3 viewposition3 = Vector3.Transform(Vector3.UnitY, viewTransform);
            size = (viewposition3 - viewposition).Length();
            position4D = Vector4.Transform(viewposition + Vector3.Up * size, screenTransform);
            position4D /= position4D.W;
            Vector2 position3 = new Vector2(position4D.X, position4D.Y);

            Vector2 scale = new Vector2((position2 - position).Length() / sourceRectangle.Width, (position3 - position).Length() / sourceRectangle.Height);

            // Calculate the rotation
            Vector3 normal = Vector3.TransformNormal(Vector3.UnitX, viewTransform);
            float rotation = -(float)Math.Atan2(normal.Y, normal.X);

            // Render using sprite batch
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None, depth);
        }

        // Overloads
        public void Render(SpriteBatch spriteBatch, Matrix worldMatrix, Camera camera)
        {
            Render(spriteBatch, worldMatrix, camera, color);
        }


    }
}