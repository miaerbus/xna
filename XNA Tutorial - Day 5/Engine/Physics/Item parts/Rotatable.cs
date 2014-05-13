using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public interface IRotatable : IRotationMatrix, IAngularPosition, IAngularVelocity { }
    public class Rotatable : Part, IRotatable
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PRotationMatrix>();
            parent.Require<PAngularPosition>();
            parent.Require<PAngularVelocity>();

            rotation = parent.As<PRotationMatrix>();
            angularPosition = parent.As<PAngularPosition>();
            angularVelocity = parent.As<PAngularVelocity>();
        }

        #region Properties

        PRotationMatrix rotation;
        public Matrix Rotation
        {
            get
            {
                return rotation.Rotation;
            }
            set
            {
                rotation.Rotation = value;
            }
        }

        PAngularPosition angularPosition;
        public Quaternion AngularPosition
        {
            get
            {
                return angularPosition.AngularPosition;
            }
            set
            {
                angularPosition.AngularPosition = value;
                rotation.Rotation = Matrix.CreateFromQuaternion(Quaternion.Normalize(value));
            }
        }

        PAngularVelocity angularVelocity;
        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity.AngularVelocity;
            }
            set
            {
                angularVelocity.AngularVelocity = value;
            }
        }

        #endregion
    }
}
