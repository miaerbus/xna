using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Artificial.XNATutorial.CarGame
{
    [Serializable()]
    class Building : IPositionWithEvents, ISerializable
    {
        // Position
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                updateWorldMatrix();
                RaiseOnPositionChanged();
            }
        }

        // OnPositionChanged
        public event EventHandler OnPositionChanged;
        private void RaiseOnPositionChanged()
        {
            EventHandler e = OnPositionChanged;
            if (e != null) e(this, null);
        }

        // Rotation
        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                updateWorldMatrix();
            }
        }

        // World matrix
        private Matrix worldMatrix = Matrix.Identity;
        public Matrix WorldMatrix
        {
            get
            {
                return worldMatrix;
            }
            set
            {
                worldMatrix = value;
            }
        }

        // Update world matrix
        private void updateWorldMatrix()
        {
            worldMatrix = Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }

        public Building()
        {
        }

        // SERIALIZATION
        protected Building(SerializationInfo info, StreamingContext context)
        {
            position = (Vector3)info.GetValue("position",typeof(Vector3));
            rotation = (float)info.GetValue("rotation",typeof(float));
            worldMatrix = (Matrix)info.GetValue("worldMatrix",typeof(Matrix));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("position", position);
            info.AddValue("rotation", rotation);
            info.AddValue("worldMatrix", worldMatrix);
        }
    }
}
