using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public class ConvexCollider : Part
    {
        public override void Install(Item parent)
        {
            base.Install(parent);

            parent.Require<PConvex>();

            forceUser = parent.As<ForceUser>();
            torqueUser = parent.As<TorqueUser>();
            convex = parent.As<PConvex>();

            position = parent.As<PPosition>();
      
            angularPosition = parent.As<PAngularPosition>();
       
            rotation = parent.As<PRotationMatrix>();

            boundingSphere = parent.As<PBoundingSphere>();
        }

        #region Properties

        ForceUser forceUser;
        public ForceUser ForceUser
        {
            get
            {
                return forceUser;
            }
        }

        TorqueUser torqueUser;
        public TorqueUser TorqueUser
        {
            get
            {
                return torqueUser;
            }
        }

        PPosition position;
        public Vector3 Position
        {
            get
            {
                if (position != null)
                {
                    return position.Position;
                }
                return Vector3.Zero;
            }
            set
            {
                if (position != null)
                {

                    position.Position = value;
                }
            }
        }


        PRotationMatrix rotation;
        public Matrix Rotation
        {
            get
            {
                if (rotation != null)
                {
                    return rotation.Rotation;
                }
                return Matrix.Identity;
            }
            set
            {
                if (rotation != null)
                {
                    rotation.Rotation = value;
                }
            }
        }
        
        PAngularPosition angularPosition;
        public Quaternion AngularPosition
        {
            get
            {
                if (angularPosition != null)
                {
                    return angularPosition.AngularPosition;
                }
                return Quaternion.Identity;
            }
            set
            {
                if (angularPosition != null)
                {
                    angularPosition.AngularPosition = value;
                }
            }
        }

        PConvex convex;
        public Convex Convex
        {
            get
            {
                return convex.Convex;
            }
            set
            {
                convex.Convex = value;
            }
        }

        static BoundingSphere infiniteSphere = new BoundingSphere(Vector3.Zero, float.PositiveInfinity);
        PBoundingSphere boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get
            {
                if (boundingSphere != null)
                {
                    return boundingSphere.BoundingSphere;
                }
                return infiniteSphere;
            }
            set
            {
                if (boundingSphere != null)
                {

                    boundingSphere.BoundingSphere = value;
                }
            }
        }


        #endregion
    }



}
