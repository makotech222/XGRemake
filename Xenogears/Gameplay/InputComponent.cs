using System;
using System.Collections.Generic;
using System.Text;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.Physics;
using Xenogears.Utilities;

namespace Xenogears.Gameplay
{
    /// <summary>
    /// Contains current state of game's input and can be passed to required scripts.
    /// </summary>
    public class InputComponent
    {
        private float DeadZone = 0.2f;

        public Vector3 Direction { get; set; } = new Vector3(0, 0, 0);
        public Vector3 DirectionCameraRelative { get; set; } = new Vector3(0, 0, 0);
        public bool Interact { get; set; }
        public bool Cancel { get; set; }
        public bool OpenMenu { get; set; }
        public bool Run { get; set; }
        public bool BeginJump { get; set; }
        public bool Jumping { get; set; }
        public bool RotateCameraLeft { get; set; }
        public bool RotateCameraRight { get; set; }

        public void Update(InputManager input, FieldCamera camera, CharacterComponent _character)
        {
            var gamepad = input.DefaultGamePad;
            if (gamepad == null)
                return;
            Vector2 leftStick = gamepad.State.LeftThumb;
            leftStick.X = Math.Abs(leftStick.X) < DeadZone ? 0 : leftStick.X;
            leftStick.Y = Math.Abs(leftStick.Y) < DeadZone ? 0 : leftStick.Y;
            if (leftStick.X < 0) leftStick.X = -1;
            if (leftStick.X > 0) leftStick.X = 1;
            if (leftStick.Y < 0) leftStick.Y = -1;
            if (leftStick.Y > 0) leftStick.Y = 1;
            this.Direction = new Vector3(leftStick.X, 0, -leftStick.Y);

            leftStick.Y = leftStick.Y * -1;
            var rotatedDirection = leftStick.Rotate((double)camera.CameraDirection * 45);
            this.DirectionCameraRelative = new Vector3((float)Math.Round(rotatedDirection.X), 0, (float)Math.Round(rotatedDirection.Y));

            this.Cancel = gamepad.IsButtonPressed(GamePadButton.B);
            if (!this.Cancel)
                this.Interact = gamepad.IsButtonPressed(GamePadButton.A);
            this.Run = gamepad.IsButtonDown(GamePadButton.B);
            this.BeginJump = gamepad.IsButtonPressed(GamePadButton.Y) && !Jumping && _character.IsGrounded;
            if (this.BeginJump)
                this.Jumping = true;
            this.OpenMenu = gamepad.IsButtonPressed(GamePadButton.Y);
            this.RotateCameraLeft = gamepad.IsButtonPressed(GamePadButton.LeftShoulder);
            if (!this.RotateCameraLeft)
                this.RotateCameraRight = gamepad.IsButtonPressed(GamePadButton.RightShoulder);
        }
    }
}
