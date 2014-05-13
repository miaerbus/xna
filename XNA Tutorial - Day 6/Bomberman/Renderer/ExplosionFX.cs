using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class ExplosionFX : Item
    {
        PAngularPosition angularPosition;
        PPosition position;

        Sprite sprite;
        float time = 0;
        float FPS = 0.05f;
        public bool Ended;
        public float Intensity
        {
            get
            {
                return 1 - (float)Math.Pow((time / FPS) / 8 - 1, 2);
            }
        }

        public ExplosionFX(Sprite sprite, Vector3 position)
        {
            this.sprite = sprite;

            Require<PPosition>().Position = position;
            Vector3 axis = new Vector3((float)Bomberman.Random.NextDouble(), (float)Bomberman.Random.NextDouble(), (float)Bomberman.Random.NextDouble());
            float rotation = (float)Bomberman.Random.NextDouble()*5;
            Require<PAngularPosition>().AngularPosition = Quaternion.CreateFromAxisAngle(axis, rotation);

            angularPosition = Part<PAngularPosition>();
            this.position = Part<PPosition>();
        }

        public void Render(GameTime gameTime, SpriteBatch spriteBatch)
        {
            time += (float)gameTime.ElapsedRealTime.TotalSeconds;
            int frame = (int)(time / FPS);
            if (frame > 15)
            {
                Ended = true;
            }
            else
            {
                sprite.SourceRectangle = new Rectangle(128 * (frame / 4), 128 * (frame % 4), 128, 128);
                sprite.Render(spriteBatch, WorldMatrix, Bomberman.Level.Camera, new Color(255,255,255,150));
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                return Matrix.CreateFromQuaternion(angularPosition.AngularPosition) * Matrix.CreateTranslation(position.Position);
            }
        }
    }
}
