using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Physics
{
    public interface IParticleCollider : IPosition, IParticleRadius { }
    public class ParticleCollider : Part, IParticleCollider
    {
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.Require<PPosition>();
            parent.Require<PParticleRadius>();

            position = parent.As<PPosition>();
            particleRadius = parent.As<PParticleRadius>();

            forceUser = parent.As<ForceUser>();
            torqueUser = parent.As<TorqueUser>();
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
                return position.Position;
            }
            set
            {
                position.Position = value;
            }
        }

        PParticleRadius particleRadius;
        public float ParticleRadius
        {
            get
            {
                return particleRadius.ParticleRadius;
            }
            set
            {
                particleRadius.ParticleRadius = value;
            }
        }

        #endregion
    }

}
