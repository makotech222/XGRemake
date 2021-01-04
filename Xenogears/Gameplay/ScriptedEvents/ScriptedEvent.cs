using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xenogears.Utilities;
using Stride.Core;
using Stride.Engine;
using Xenogears.Database;
using SharpFont.MultipleMasters;
using Stride.Core.Mathematics;
using Stride.Physics;
using System;

namespace Xenogears.Gameplay
{
    public class ScriptedEvent : StartupScript
    {
        #region Fields

        protected StaticColliderComponent _collider;

        /// <summary>
        /// Minimum distance of interaction before triggering.
        /// </summary>
        protected readonly float _minimumDistance = 0.6f;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Requires player to press 'A' to initiate event
        /// </summary>
        public bool RequiresInteraction { get; set; }

        [DataMemberIgnore]
        public bool Initialized { get { return _collider != null; } }

        #endregion Properties

        #region Events

        #endregion Events

        #region Methods

        public override void Start()
        {
            base.Start();
            _collider = Entity.Get<StaticColliderComponent>();
        }

        public virtual void Update(InputComponent input, PlayerFieldCharacter character)
        {
            var characterCollider = character.CharacterComponent;

            if (input.Interact && RequiresInteraction)
            {
                var startPos = character.Entity.Transform.Position + new Vector3(0, 0.5f, 0);
                var forwardvec = startPos + (character.GetForwardVector() * 2.0f);
                var hitresult = this.GetSimulation().Raycast(startPos, forwardvec, CollisionFilterGroups.SensorTrigger, CollisionFilterGroupFlags.SensorTrigger);
                if (hitresult.Collider == _collider && (this.Entity.Transform.Position - character.Entity.Transform.Position).LengthSquared() < _minimumDistance)
                {
                    //Ensure character and eventbox are facing each other when interacting.
                    forwardvec.Normalize();
                    var eventNormal = Entity.Transform.RotationEulerXYZ.Forward();
                    var characterNormal = forwardvec;
                    var isFacing = Vector3.Dot(eventNormal, characterNormal).RoughlyEquals(-1.0f);
                    if (isFacing)
                        Execute(input, character);
                }
            }
            else
            {
                foreach (var collision in _collider.Collisions)
                {
                    if ((collision.ColliderA == characterCollider || collision.ColliderB == characterCollider) && !RequiresInteraction)
                    {
                        Execute(input, character);
                    }
                }
            }
        }

        protected virtual void Execute(InputComponent input, PlayerFieldCharacter character)
        {
        }

        #endregion Methods
    }
}