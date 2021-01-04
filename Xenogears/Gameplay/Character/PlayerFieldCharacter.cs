using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Xenogears.Database;
using Xenogears.Utilities;

namespace Xenogears.Gameplay
{
    public class PlayerFieldCharacter : BaseFieldCharacter
    {
        #region Fields

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructor

        #endregion Constructor

        #region Methods

        public override void Start()
        {
            base.Start();
        }

        public override void Update(InputComponent input, FieldCamera camera)
        {
            _movementComponent.RawDirection = input.Direction;
            _movementComponent.Direction = input.DirectionCameraRelative;
            _movementComponent.BeginJump = input.BeginJump;
            if (input.BeginJump)
                _movementComponent.Jumping = input.BeginJump;
            _movementComponent.Running = input.Run;
            base.Update(input, camera);
            input.Jumping = _movementComponent.Jumping;
            this.Direction = CalculateDirection(_movementComponent.Direction.X, _movementComponent.Direction.Z * -1,this.Direction);
        }

        protected override void DetermineSprite(FieldCamera camera)
        {
            var x = _movementComponent.RawDirection.X;
            var z = _movementComponent.RawDirection.Z * -1;

            EActionTypes newAnimationName;
            if (_movementComponent.Jumping) newAnimationName = EActionTypes.Jump;
            else if (z == 0 && x == 0) newAnimationName = EActionTypes.Idle;
            else if (_movementComponent.Running) newAnimationName = EActionTypes.Run;
            else newAnimationName = EActionTypes.Walk;

            var newDirection = _nextAnimation?.Direction ?? _previousAnimation.Direction;
            newDirection = CalculateDirection(x, z, newDirection);

            if (z == 0 && x == 0 && camera.CameraDirectionChanged)
                newDirection = camera.CameraDirection.Next() == camera.PreviousCameraDirection ? newDirection.Next() : newDirection.Previous();

            var actualAnimations = _animations[(newAnimationName, newDirection)];
            var nextFrame = _nextAnimation?.Frame ?? _previousAnimation.Frame;

            if (_currentTicks == 0) // Only advance to next frame if animation has ticked and reset
            {
                nextFrame += 1;
            }

            if (newAnimationName != _previousAnimation.Name) // Reset frame count if animation has changed
            {
                nextFrame = 0;
            }

            if (nextFrame >= actualAnimations.First().Count - 1) // Loop back to start. For jumping, this just ends the jump animation.
            {
                nextFrame = 0;
                _movementComponent.Jumping = false; // End jump at end of frames
            }
            _nextAnimation = actualAnimations[nextFrame];
        }

        private ECameraDirection CalculateDirection(float x, float z, ECameraDirection previousState)
        {
            var newDirection = previousState;
            if (z == 1 && x == 0) newDirection = ECameraDirection.North;
            if (z == 1 && x == 1) newDirection = ECameraDirection.NorthEast;
            if (z == 0 && x == 1) newDirection = ECameraDirection.East;
            if (z == -1 && x == 1) newDirection = ECameraDirection.SouthEast;
            if (z == -1 && x == 0) newDirection = ECameraDirection.South;
            if (z == -1 && x == -1) newDirection = ECameraDirection.SouthWest;
            if (z == 0 && x == -1) newDirection = ECameraDirection.West;
            if (z == 1 && x == -1) newDirection = ECameraDirection.NorthWest;
            return newDirection;

        }

        #endregion Methods
    }
}