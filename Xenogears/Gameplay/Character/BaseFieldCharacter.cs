using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Graphics;
using Stride.Physics;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using Xenogears.Database;
using Xenogears.Utilities;

namespace Xenogears.Gameplay
{
    public abstract class BaseFieldCharacter : StartupScript
    {
        #region Fields
        protected SpriteComponent _spriteComponent;
        protected CharacterComponent _characterComponent;
        protected MovementComponent _movementComponent = new MovementComponent();
        /// <summary>
        /// Dictionary of all field animations, keyed by animation name + direction
        /// </summary>
        protected Dictionary<(EActionTypes, ECameraDirection), List<IndividualAnimation>> _animations;
        protected IndividualAnimation _previousAnimation;
        protected IndividualAnimation _nextAnimation;
        protected double _currentTicks;
        protected readonly float _walkSpeed = 2.0f;
        protected readonly float _runSpeed = 4.0f;
        protected readonly string _spritesPath = @"../../../../XenoRip";

        /// <summary>
        /// Used to keep the sprite transform a small distance closer to the camera each frame, to avoid clipping with ground
        /// </summary>
        private Vector3 _unmodifiedPosition;
        #endregion Fields

        #region Properties

        /// <summary>
        /// When set, loads the corresponding XGCharacter from the database.
        /// </summary>
        public string XGCharacterName { get; set; }

        [DataMemberIgnore]
        public XGCharacter XGCharacter { get; set; }

        [DataMemberIgnore]
        public CharacterComponent CharacterComponent { get { return _characterComponent; } }

        [DataMemberIgnore]
        public bool Initialized { get { return XGCharacter != null; } }

        public ECameraDirection Direction { get; protected set; }
        #endregion Properties

        #region Constructor

        #endregion Constructor

        #region Events
        #endregion

        #region Methods

        public override void Start()
        {
            if (this.Initialized)
                return;
            base.Start();
            this.XGCharacter = Core.Instance.XGCharacters[this.XGCharacterName];
            this.Direction = ECameraDirection.North;

            _spriteComponent = Entity.GetComponentInChildren<SpriteComponent>();
            _characterComponent = Entity.Get<CharacterComponent>();
            _animations = new Dictionary<(EActionTypes, ECameraDirection), List<IndividualAnimation>>();

            _spriteComponent.SpriteProvider = BuildSpriteSheet();
            _spriteComponent.SpriteType = SpriteType.Sprite;
            _spriteComponent.IgnoreDepth = false;
            _spriteComponent.Entity.Transform.Scale.Y = 2.5f;
            _characterComponent.FallSpeed = 50f;
            _characterComponent.JumpSpeed = 5f;
            _characterComponent.Gravity = new Vector3(0, -12f, 0);
            _unmodifiedPosition = _spriteComponent.Entity.Transform.Position;
            _characterComponent.UpdatePhysicsTransformation();
        }

        public virtual void Update(InputComponent input, FieldCamera camera)
        {
            _spriteComponent.Entity.Transform.Position = _unmodifiedPosition;
            MoveCharacter();
            DetermineSprite(camera);
            _unmodifiedPosition = _spriteComponent.Entity.Transform.Position;
            _currentTicks += Game.UpdateTime.Elapsed.TotalMilliseconds;
            if (_currentTicks >= 125) // Each animation takes 0.125 seconds to complete. Only change sprite after this time elapsed
            {
                var spriteProvider = _spriteComponent.SpriteProvider as SpriteFromSheet;
                spriteProvider.CurrentFrame = _nextAnimation.SpriteSheetId;
                _previousAnimation = _nextAnimation;
                _nextAnimation = null;
                _currentTicks = 0;

                
            }
            this.Direction = _previousAnimation.Direction;

            _spriteComponent.Entity.Transform.Rotation = camera.CameraTarget.Rotation; // Keep sprite rotated towards camera. TODO: Update Sprite's Y Value to scale properly with camera angle.
            var cameraEntity = Core.Instance.MainCamera.Entity.Transform;
            var direction = (Entity.Transform.Position - cameraEntity.LocalToWorld(cameraEntity.Position));
            direction.Normalize();

            _spriteComponent.Entity.Transform.Position -= 0.3f * direction;
        }

        protected virtual SpriteFromSheet BuildSpriteSheet()
        {
            var ss = new SpriteSheet();
            //Each action has 8 directions * actionCount sprites
            int spriteSheetId = 0;
            foreach (var animation in XGCharacter.RawAnimationsData)
            {
                FileInfo f = new FileInfo(Path.Combine(_spritesPath, animation.StartSprite));
                for (int dir = 0; dir < 5; dir++)
                {
                    int skip = animation.SkipBetweenFiles * dir;
                    for (int num = 0; num < animation.Count; num++)
                    {
                        var spritePath = f.DirectoryName + "\\" + (Convert.ToInt32(Path.GetFileNameWithoutExtension(f.Name)) + ((dir * animation.Count) + num + skip)).ToString().PadLeft(4, '0') + ".png";

                        if (File.Exists(spritePath)) // Some sprites missing?
                        {
                            var bytes = File.ReadAllBytes(spritePath);
                            var image = Stride.Graphics.Image.Load(bytes);
                            var texture = Texture.New(this.GraphicsDevice, image, TextureFlags.ShaderResource);
                            var sprite = new Sprite($"{animation.Name}_{dir}_{num}", texture);
                            ss.Sprites.Add(sprite);
                            var animationKey = (animation.Name, (ECameraDirection)dir);
                            if (!this._animations.ContainsKey(animationKey))
                                this._animations.Add(animationKey, new List<IndividualAnimation>());
                            _animations[animationKey].Add(new IndividualAnimation()
                            {
                                Name = animation.Name,
                                Count = animation.Count,
                                Direction = (ECameraDirection)dir,
                                Frame = num,
                                SpriteSheetId = spriteSheetId++
                            });
                            if (dir > 0 && dir < 4) // Get non-north south sprites. Will use for mirror
                            {
                                var bitmap = new Bitmap(spritePath);
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                bytes = BitmapToByteArray(bitmap);
                                image = Stride.Graphics.Image.Load(bytes);
                                texture = Texture.New(this.GraphicsDevice, image, TextureFlags.ShaderResource);
                                sprite = new Sprite($"{animation.Name}_{dir}_{num}", texture);
                                ss.Sprites.Add(sprite);
                                animationKey = (animation.Name, (ECameraDirection)(8 - dir));
                                if (!this._animations.ContainsKey(animationKey))
                                    this._animations.Add(animationKey, new List<IndividualAnimation>());
                                _animations[animationKey].Add(new IndividualAnimation()
                                {
                                    Name = animation.Name,
                                    Count = animation.Count,
                                    Direction = (ECameraDirection)(8 - dir),
                                    Frame = num,
                                    SpriteSheetId = spriteSheetId++
                                });
                            }
                        }
                    }
                }
            }
            _previousAnimation = _animations[(EActionTypes.Idle, ECameraDirection.North)][0];
            return SpriteFromSheet.Create(ss, "Idle_0_0");
        }

        protected abstract void DetermineSprite(FieldCamera camera);

        protected virtual void MoveCharacter()
        {
            var velocity = new Vector3(_movementComponent.Direction.X, _movementComponent.Direction.Y, _movementComponent.Direction.Z);
            velocity.Normalize();
            if (_movementComponent.BeginJump)
                _characterComponent.Jump();
            _characterComponent.SetVelocity(velocity * (float)(_movementComponent.Running ? _runSpeed : _walkSpeed));
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        public FieldCharacterSerialization Serialize()
        {
            return new FieldCharacterSerialization()
            {
                XGCharacterName = this.XGCharacterName
            };
        }
        /// <summary>
        /// Properly loads the character upon scene change. Make sure to call before adding entity to scene, so Start() works properly.
        /// </summary>
        /// <param name="s"></param>
        public void Deserialize(FieldCharacterSerialization s)
        {
            this.XGCharacterName = s.XGCharacterName;
        }

        /// <summary>
        /// Prepare character for moving to new scene. Probably temp code
        /// </summary>
        public void Reset()
        {
            this.XGCharacter = null;
            if (_spriteComponent != null)
                _spriteComponent.Entity.Transform.Position = _unmodifiedPosition;
        }

        public Vector3 GetForwardVector()
        {
            Vector3 forward = new Vector3();
            //North is -z direction. East is +x direction
            if (Direction == ECameraDirection.North)
                forward = new Vector3(0, 0, -1);
            if (Direction == ECameraDirection.NorthEast)
                forward = new Vector3(1, 0, -1);
            if (Direction == ECameraDirection.East)
                forward = new Vector3(1, 0, 0);
            if (Direction == ECameraDirection.SouthEast)
                forward = new Vector3(1, 0, 1);
            if (Direction == ECameraDirection.South)
                forward = new Vector3(0, 0, 1);
            if (Direction == ECameraDirection.SouthWest)
                forward = new Vector3(-1, 0, 1);
            if (Direction == ECameraDirection.West)
                forward = new Vector3(-1, 0, 0);
            if (Direction == ECameraDirection.NorthWest)
                forward = new Vector3(-1, 0, -1);
            forward.Normalize();
            return forward;
        }
        #endregion Methods

        #region Classes
        [DataContract]
        protected class IndividualAnimation
        {
            public EActionTypes Name { get; set; }
            public ECameraDirection Direction { get; set; }
            public int Frame { get; set; }
            public int Count { get; set; }
            public int SpriteSheetId { get; set; }
        }

        [DataContract]
        public class MovementComponent
        {
            /// <summary>
            /// Raw input direction from player. Used only for player character
            /// </summary>
            public Vector3 RawDirection { get; set; }
            public Vector3 Direction { get; set; }
            public bool BeginJump { get; set; }
            public bool Jumping { get; set; }
            public bool Running { get; set; }
        }
        #endregion
    }


    /// <summary>
    /// Can be used to load a character properly
    /// </summary>
    public class FieldCharacterSerialization
    {
        public string XGCharacterName { get; set; }
    }
}