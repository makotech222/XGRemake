using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.Core.Mathematics;
using System.Linq;
using Xenogears.Utilities;
using Stride.Core;
using Xenogears.Database;

namespace Xenogears.Gameplay
{
    public class NPCFieldCharacter : BaseFieldCharacter
    {
        #region Fields

        private NPCAction _currentAction;

        #endregion Fields

        #region Properties

        public List<NPCAction> ActionList { get; set; } = new List<NPCAction>();
        public bool RepeatActions { get; set; }

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
            ExecuteAction();
            base.Update(input, camera);

            bool isCollidingWithPlayer = _characterComponent.Collisions.FirstOrDefault(x => x.ColliderB.Entity == Core.Instance.Player.Entity) != null;
            if (_currentAction != null && _currentTicks == 0 && !isCollidingWithPlayer)
            {
                _currentAction.CurrentFrame++;
                if (_currentAction.CurrentFrame > _currentAction.Frames - 1)
                {
                    _currentAction = null;
                }
            }
            this.Direction = _previousAnimation.Direction;
        }

        protected void ExecuteAction()
        {
            if (ActionList.Count == 0 && _currentAction == null)
            {
                ActionList.Add(new NPCAction() { Action = EActionTypes.Idle, Direction = _previousAnimation.Direction, Frames = 1 }) ;
            }
            if (_currentAction == null)
            {
                _currentAction = ActionList.First();
                if (_currentAction.CurrentFrame < _currentAction.InitialFrame) _currentAction.CurrentFrame = _currentAction.InitialFrame;
                ActionList.RemoveAt(0);
                if (RepeatActions)
                    ActionList.Add(new NPCAction(_currentAction));
            }
            //Handle movement if any
            _movementComponent.Direction = new Vector3(0, 0, 0);
            if (_currentAction.Action == EActionTypes.Walk ||
                _currentAction.Action == EActionTypes.Run)
            {
                var direction = new Vector3();
                if (_currentAction.Direction == ECameraDirection.North) direction.Z = -1;
                if (_currentAction.Direction == ECameraDirection.NorthEast) direction.Z = -1;
                if (_currentAction.Direction == ECameraDirection.East) direction.Z = 0;
                if (_currentAction.Direction == ECameraDirection.SouthEast) direction.Z = 1;
                if (_currentAction.Direction == ECameraDirection.South) direction.Z = 1;
                if (_currentAction.Direction == ECameraDirection.SouthWest) direction.Z = 1;
                if (_currentAction.Direction == ECameraDirection.West) direction.Z = 0;
                if (_currentAction.Direction == ECameraDirection.SouthWest) direction.Z = -1;

                if (_currentAction.Direction == ECameraDirection.North) direction.X = 0;
                if (_currentAction.Direction == ECameraDirection.NorthEast) direction.X = 1;
                if (_currentAction.Direction == ECameraDirection.East) direction.X = 1;
                if (_currentAction.Direction == ECameraDirection.SouthEast) direction.X = 1;
                if (_currentAction.Direction == ECameraDirection.South) direction.X = 0;
                if (_currentAction.Direction == ECameraDirection.SouthWest) direction.X = -1;
                if (_currentAction.Direction == ECameraDirection.West) direction.X = -1;
                if (_currentAction.Direction == ECameraDirection.SouthWest) direction.X = -1;
                _movementComponent.Direction = direction;
                _movementComponent.Running = _currentAction.Action == EActionTypes.Run;
                _movementComponent.BeginJump = _currentAction.Action == EActionTypes.Jump && !_movementComponent.Jumping;
            }
        }

        protected override void DetermineSprite(FieldCamera camera)
        {
            ECameraDirection direction = (ECameraDirection)(((0).LoopAdd(-(int)camera.CameraDirection,7,0).LoopAdd((int)_currentAction.Direction,7,0)));

            bool isCollidingWithPlayer = _characterComponent.Collisions.FirstOrDefault(x => x.ColliderB.Entity == Core.Instance.Player.Entity || x.ColliderA.Entity == Core.Instance.Player.Entity) != null;
            var actualAnimations = _animations[(isCollidingWithPlayer ? EActionTypes.Idle : _currentAction.Action, direction)];

            var nextFrame = _currentAction.FreezeFrame ? _currentAction.InitialFrame :_currentAction.CurrentFrame;
            if (nextFrame >= actualAnimations.First().Count) // Loop back to start. For jumping, this just ends the jump animation.
            {
                nextFrame = nextFrame % actualAnimations.First().Count;
                _movementComponent.Jumping = false; // End jump at end of frames
            }
            _nextAnimation = actualAnimations[nextFrame];
        }

        #endregion Methods

        #region Classes

        [DataContract]
        public class NPCAction
        {
            public EActionTypes Action { get; set; }
            public ECameraDirection Direction { get; set; }

            /// <summary>
            /// How many frames the action should be done for. Each frame is 1/8 of a second.
            /// </summary>
            public int Frames { get; set; }

            public int InitialFrame { get; set; }

            /// <summary>
            /// Zero-based. Current frame that action is on.
            /// </summary>
            public int CurrentFrame { get; set; }

            /// <summary>
            /// If true, will freeze animation to the current frame and not advance each tick through sprite sheet. Will still advance to next action, though.
            /// </summary>
            public bool FreezeFrame { get; set; }

            public NPCAction()
            {
            }

            public NPCAction(NPCAction copy)
            {
                this.Action = copy.Action;
                this.Direction = copy.Direction;
                this.Frames = copy.Frames;
                this.CurrentFrame = copy.CurrentFrame;
            }
        }

        #endregion Classes
    }
}