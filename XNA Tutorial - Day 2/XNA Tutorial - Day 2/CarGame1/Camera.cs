using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    public class Camera
    {
        // PROPERTIES

        // Field of view
        private float fieldOfView;
        public float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
            }
        }

        // Aspect ratio
        private float aspectRatio = 4f/3f;
        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
            }
        }

        // Near plane
        private float nearPlane;
        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
            }
        }

        // Far plane
        private float farPlane;
        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
            }
        }

        // Up direction
        private Vector3 upDirection = Vector3.Up;
        public Vector3 UpDirection
        {
            get
            {
                return upDirection;
            }
            set
            {
                upDirection = value;
            }
        }

        // Target vector
        private Vector3 target;
        public Vector3 Target
        {
            get
            {
                if (TargetThing == null)
                    return target;
                else
                    return TargetThing.Position;
            }
            set
            {
                target = value;
            }
        }
       
        // Target thing
        private IPosition targetThing;
        public IPosition TargetThing
        {
            get
            {
                return targetThing;
            }
            set
            {
                targetThing = value;
            }
        }

        // Camera direction
        private Vector3 direction = Vector3.Forward;
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                direction.Normalize();
            }
        }

        // Distance
        private float distance;
        public float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }



        // RENDERING

        // View Matrix
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(Target - Direction * Distance, Target, upDirection);
            }
        }

        // Projection matrix
        public Matrix ProjectionMatrix
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
            }
        }
    }
}
