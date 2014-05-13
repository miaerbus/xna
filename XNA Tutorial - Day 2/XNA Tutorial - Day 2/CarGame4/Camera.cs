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
        private float fieldOfView=1;
        public float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
                updateProjectionMatrix();
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
                updateProjectionMatrix();
            }
        }

        // Near plane
        private float nearPlane=0.1f;
        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
                updateProjectionMatrix();
            }
        }

        // Far plane
        private float farPlane=1000;
        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
                updateProjectionMatrix();
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
                updateViewMatrix();
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
                updateViewMatrix();
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
                updateTargetVector();
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
                updateViewMatrix();
            }
        }

        // Distance
        private float distance=1;
        public float Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
                updateViewMatrix();
            }
        }        

        // View matrix
        Matrix viewMatrix;
        public Matrix ViewMatrix
        {
            get
            {
                updateTargetVector();
                return viewMatrix;
            }
        }

        // Projection matrix
        Matrix projectionMatrix;
        public Matrix ProjectionMatrix
        {
            get
            {
                return projectionMatrix;
            }
        }

        // Bounding frustum
        BoundingFrustum frustum;
        public BoundingFrustum Frustum
        {
            get
            {
                updateTargetVector();
                return frustum;
            }
        }

        // UPDATING
        private void updateTargetVector()
        {
            if (targetThing==null) return;
            if (target != targetThing.Position)
            {
                Target = targetThing.Position;
            }
        }

        private void updateViewMatrix()
        {
            viewMatrix = Matrix.CreateLookAt(Target - Direction * Distance, Target, upDirection);
            updateFrustum();
        }

        private void updateProjectionMatrix()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
            updateFrustum();
        }

        private void updateFrustum()
        {
            frustum = new BoundingFrustum(viewMatrix * projectionMatrix);
        }
    }
}
