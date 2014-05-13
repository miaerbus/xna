using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    class Object2D : WorldObject
    {
        // Graphics internals
        SpriteBatch spriteBatch;
        GraphicsDevice device;

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
            }
        }
      
        // CONSTRUCTOR
        public Object2D(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            device = spriteBatch.GraphicsDevice;
        }

        // RENDER
        public override void Render(Camera camera)
        {
            // Create the transformation using the given camera variables and object matrices
            Matrix screen = Matrix.CreateTranslation(1, -1, 0) * Matrix.CreateScale(device.Viewport.Width / 2, -device.Viewport.Height / 2, 1);
            Matrix viewTransform = NormalMatrix * WorldMatrix * camera.ViewMatrix;
            Matrix screenTransform = camera.ProjectionMatrix * screen;

            // Calculate the screen position and depth
            Vector3 viewposition = Vector3.Transform(Vector3.Zero, viewTransform);
            Vector4 position4D = Vector4.Transform(viewposition, screenTransform);
            position4D /= position4D.W;
            Vector2 position = new Vector2(position4D.X, position4D.Y);
            float depth = position4D.Z;

            // Calculate the scale
            Vector3 viewposition2 = Vector3.Transform(Vector3.UnitX, viewTransform);
            float size = (viewposition2 - viewposition).Length();
            position4D = Vector4.Transform(viewposition + Vector3.Up * size, screenTransform);
            position4D /= position4D.W;
            Vector2 position2 = new Vector2(position4D.X, position4D.Y);            
            float scale = (position2-position).Length()/texture.Height;

            // Calculate the rotation
            Vector3 normal = Vector3.TransformNormal(Vector3.UnitX, viewTransform);
            float rotation = -(float)Math.Atan2(normal.Y, normal.X);

            // Render using sprite batch
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            // Use depth buffer for correct sorting
            device.RenderState.DepthBufferEnable = true; 
            device.RenderState.DepthBufferWriteEnable = true;
            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, rotation, new Vector2(texture.Width, texture.Height)/2f, scale, SpriteEffects.None, depth);
            spriteBatch.End();

#if DEBUG
            // Increase for debug output
            Globals.SpritesRendered++;
#endif
        }

    }
}
