using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class CharacterThumb : Item
    {
        float angle = 0;

        AnimatedModel model;
        public AnimatedModel Model
        {
            get
            {
                return model;
            }
        }

        PAngularPosition angularPosition;

        public CharacterThumb(AnimatedModel model)
        {
            this.model = new AnimatedModel();
            this.model.Model = model.Model;
            this.model.NormalMatrix = model.NormalMatrix;
            this.model.Animation = "Walk";

            angularPosition = Require<PAngularPosition>();
        }

        public void Update(float dt, AnimatedModel model)
        {
            angle += dt;
            angularPosition.AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Forward, angle);
            if (this.model.Model != model.Model)
            {
                this.model.Model = model.Model;
                this.model.NormalMatrix = model.NormalMatrix;
                this.model.Animation = "Walk";
            }
        }
    }
}
