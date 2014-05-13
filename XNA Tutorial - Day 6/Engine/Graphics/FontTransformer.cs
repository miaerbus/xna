using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontRenderer;

namespace Artificial.XNATutorial
{
    public static class FontTransformer
    {
        // RENDER
        public static void Write(String text, Font font, SpriteBatch spriteBatch, float textSize, Matrix worldMatrix, Camera camera, Color color, Vector2 shadow)
        {
            GraphicsDevice device = spriteBatch.GraphicsDevice;

            // Create the transformation using the given camera variables and object matrices
            Matrix screen = Matrix.CreateTranslation(1, -1, 0) * Matrix.CreateScale(device.Viewport.Width / 2, -device.Viewport.Height / 2, 1);
            Matrix worldTransform = worldMatrix;
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

            // Calculate the rotation
            Vector3 normal = Vector3.TransformNormal(Vector3.UnitX, viewTransform);
            float rotation = -(float)Math.Atan2(normal.Y, normal.X);

            // Render using sprite batch
            if (shadow != Vector2.Zero) font.DrawCentered(text, position + shadow, textSize, Color.Black, spriteBatch);
            font.DrawCentered(text, position, textSize, color, spriteBatch);
        }
    }
}
