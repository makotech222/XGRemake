using Stride.Core;
using Stride.Engine;
using Stride.Core.Mathematics;
using Stride.Physics;
using Stride.Rendering.Sprites;
using Xenogears.Database;
using Xenogears.Utilities;
using System;

namespace Xenogears.Gameplay
{
    /// <summary>
    /// Script that handles all camera logic for the Field
    /// </summary>
    public class FieldCamera : StartupScript
    {
        #region Fields
        protected CameraComponent _camera;
        protected TransformComponent _cameraTarget;
        protected ECameraDirection _newDirection;
        protected int _newDirectionDegrees;
        protected int _currentDirectionDegrees;
        protected float _currentLerpAmount;
        protected Quaternion _currentQuaternion;
        #endregion Fields

        #region Properties
        [DataMemberIgnore]
        public ECameraDirection CameraDirection { get; set; }
        [DataMemberIgnore]
        public ECameraDirection PreviousCameraDirection { get; set; }

        [DataMemberIgnore]
        public TransformComponent CameraTarget { get { return _cameraTarget; } }

        /// <summary>
        /// Determines if camera direction has changed in last animation frame. Signals to animation to change direction.
        /// </summary>
        [DataMemberIgnore]
        public bool CameraDirectionChanged { get; set; }

        [DataMemberIgnore]
        public CameraComponent CameraComponent { get { return _camera; } }

        [DataMemberIgnore]
        public bool Initialized { get { return _camera != null; } }
        #endregion Properties

        #region Constructor

        #endregion Constructor

        #region Methods

        public override void Start()
        {
            base.Start();
            _camera = Entity.Get<CameraComponent>();
            _cameraTarget = Entity.GetParent().Transform;
            _currentQuaternion = _cameraTarget.Rotation;
        }

        public virtual void Update(InputComponent input)
        {
            if (!_camera.Enabled) // Scene change disabled camera temporarily.
                _camera.Enabled = true;

            CameraDirectionChanged = false;
            if (_currentDirectionDegrees == _newDirectionDegrees)
            {
                if (input.RotateCameraLeft)
                {
                    _newDirection = CameraDirection.Previous();
                    _newDirectionDegrees = _currentDirectionDegrees + 45;
                }
                else if (input.RotateCameraRight)
                {
                    _newDirection = CameraDirection.Next();
                    _newDirectionDegrees = _currentDirectionDegrees - 45;
                }
            }
            if (_currentDirectionDegrees != _newDirectionDegrees)
            {
                var deltaTime = Game.UpdateTime.Elapsed.TotalSeconds;
                var desiredQuaternion = Quaternion.RotationY(_newDirectionDegrees * (float)(Math.PI / 180));
                var amount = (float)Math.Clamp(_currentLerpAmount + (1.5 * deltaTime),0,1);
                _cameraTarget.Rotation = Quaternion.Lerp(_currentQuaternion, desiredQuaternion, amount);
                _currentLerpAmount = amount;
                if (amount >= 0.6f && CameraDirection != _newDirection)
                {
                    CameraDirection = _newDirection;
                    CameraDirectionChanged = true;
                }
                if (amount == 1.0f)
                {
                    _currentLerpAmount = 0;
                    _currentDirectionDegrees = _newDirectionDegrees;
                    _currentQuaternion = _cameraTarget.Rotation;
                    PreviousCameraDirection = CameraDirection;
                }
            }

        }

        #endregion Methods
    }

    public enum ECameraDirection
    {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7,
    }
}