using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class CharacterRenderInfo : Item
    {
        float turnSpeed = 5f;
        AnimatedModel model;
        public AnimatedModel Model
        {
            get
            {
                return model;
            }
        }
        Character character;

        PAngularPosition targetAngularPosition;
        PAngularPosition angularPosition;
        PPosition position;
        Vector3 oldPosition;


        public CharacterRenderInfo(Character character, AnimatedModel model)
        {
            this.model = new AnimatedModel();
            this.model.Model = model.Model;
            this.model.NormalMatrix = model.NormalMatrix;
            this.model.Animation = model.Animation;

            this.character = character;
            angularPosition = Require<PAngularPosition>();
            targetAngularPosition = character.Part<PAngularPosition>();
            angularPosition.AngularPosition = targetAngularPosition.AngularPosition;
            position = character.Part<PPosition>();            
            oldPosition = position.Position;
        }

        float speed;
        public float Speed
        {
            get
            {
                return speed;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (character.IsDead)
            {
                //if (model.Animation != "Die")
                //{
                //    model.Animation = "Die";
                //    model.LoopAnimation = false;
                //}
            }
            else if (character.HasWon)
            {
                if (model.Animation != "Idle") model.Animation = "Idle";
            }
            else
            {
                angularPosition.AngularPosition = Quaternion.Lerp(angularPosition.AngularPosition, targetAngularPosition.AngularPosition, turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                speed = ((oldPosition - position.Position) * new Vector3(1, 0, 1)).Length() / (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (float.IsNaN(speed)) speed = 0;
                if (speed > 4)
                {
                    if (model.Animation != "Walk") model.Animation = "Walk";
                }
                else
                {
                    if (model.Animation != "Idle") model.Animation = "Idle";
                }
                oldPosition = position.Position;
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
