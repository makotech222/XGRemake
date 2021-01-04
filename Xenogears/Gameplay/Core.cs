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
using Stride.Core.Diagnostics;
using Stride.Engine.Design;
using Stride.Core.Reflection;

namespace Xenogears.Gameplay
{
    /// <summary>
    /// The main game core class. Designed as a singleton that will exist on the root scene. Is responsible for loading game, changing scenes, and carrying references to all important things. Basically the main entry point to the game.
    /// </summary>
    public class Core : SyncScript
    {
        #region Fields
        private InputComponent _input;
        private Prefab _fieldPlayerPrefab;
        #endregion Fields

        #region Properties

        [DataMemberIgnore]
        public static Core Instance { get; set; }

        /// <summary>
        /// Reference to the main player character.
        /// </summary>
        [DataMemberIgnore]
        public PlayerFieldCharacter Player { get; set; }

        /// <summary>
        /// List of all characters, player and NPC in current scene.
        /// </summary>
        [DataMemberIgnore]
        public List<BaseFieldCharacter> CharactersInScene { get; set; }

        /// <summary>
        /// Reference to the main camera
        /// </summary>
        [DataMemberIgnore]
        public FieldCamera MainCamera { get; set; }

        /// <summary>
        /// All XGCharacter info from the database. Loaded at Start
        /// </summary>
        [DataMemberIgnore]
        public Dictionary<string, XGCharacter> XGCharacters { get; set; }

        [DataMemberIgnore]
        public Dictionary<string, XGGameState> XGGameStates { get; set; }

        [DataMemberIgnore]
        public bool Paused { get; set; }

        /// <summary>
        /// List of Scene Events in the current scene, which needs to be ticked each update. Includes scene transisions, position-based dialogues, etc.
        /// </summary>
        [DataMemberIgnore]
        public List<ScriptedEvent> SceneEvents { get; set; }

        /// <summary>
        /// Current game state.
        /// </summary>
        [DataMemberIgnore]
        public XGGameState GameState { get; set; }

        #endregion Properties

        #region Methods

        public override void Start()
        {
            base.Start();
            this.Paused = true;
            Instance = this;
            _input = new InputComponent();
            using (var db = new XenogearsDBContext())
            {
                this.XGCharacters = db.Characters.Include(x => x.RawAnimationsData).ToDictionary(x => x.Name, x => x);
                this.XGGameStates = db.GameStates.ToDictionary(x => x.Name, x => x);
            }
            this.GameState = this.XGGameStates["GameStart"];

            _fieldPlayerPrefab = Content.Load<Prefab>("Prefabs/FieldPlayer");

            //Load up initial debug scene on game start.
            UpdateScene(new SceneChangeArgs()
            {
                SceneName = "DebugScene",
                SceneChangeId = 1,
                PlayerFieldCharacter = new FieldCharacterSerialization() { XGCharacterName = "Fei" }
            });
        }

        /// <summary>
        /// Main Game Loop
        /// </summary>
        public override void Update()
        {
            _input.Update(this.Input, MainCamera, Player.CharacterComponent);
            if (MainCamera.Initialized)
                MainCamera.Update(_input);
            foreach (var character in CharactersInScene)
            {
                if (character.Initialized)
                    character.Update(_input, MainCamera);
            }
            foreach (var sceneEvent in this.SceneEvents)
            {
                if (sceneEvent.Initialized)
                    sceneEvent.Update(_input, this.Player);
            }
        }

        private void UpdateSceneReferences()
        {
            Player = Entity.EntityManager.First(x => x.Get<PlayerFieldCharacter>() != null).Get<PlayerFieldCharacter>();
            MainCamera = Player.Entity.GetComponentInChildren<CameraComponent>().Entity.Get<FieldCamera>();
            this.SceneEvents = Entity.EntityManager.Where(x => x.Get<ScriptedEvent>() != null).Select(x => x.Get<ScriptedEvent>()).ToList();
            this.CharactersInScene = Entity.EntityManager.Where(x => x.Get<BaseFieldCharacter>() != null).Select(x => x.Get<BaseFieldCharacter>()).ToList();
        }

        private void ClearSceneReferences()
        {
            Content.Unload($"Scenes/{Entity.Scene.Children[0].Name}");
            Entity.Scene.Entities.Remove(this.Player.Entity);
            this.Player.Entity.Scene = null;
            Entity.Scene.Children[0].Parent = null;

            foreach (var character in CharactersInScene)
                character.Reset();
            this.SceneEvents = null;
            this.CharactersInScene = null;
        }

        public void UpdateScene(SceneChangeArgs args)
        {
            //Clear out previous scene
            if (Entity.Scene.Children.Count > 0)
            {
                this.MainCamera.CameraComponent.Enabled = false; // Turn off camera while changing scene to avoid flickering.
                ClearSceneReferences();
            }
            Entity.Scene.Children.Clear();
            var scene = Content.Load<Scene>($"Scenes/{args.SceneName}");
            Entity.Scene.Children.Add(scene);
            scene.Name = args.SceneName;

            //Create player for first scene, or reset it in preparation for new scene.
            if (this.Player == null)
            {
                var playerInstance = _fieldPlayerPrefab.Instantiate()[0];
                playerInstance.Get<PlayerFieldCharacter>().Deserialize(args.PlayerFieldCharacter);
                this.Player = playerInstance.Get<PlayerFieldCharacter>();
            }
            scene.Entities.Add(this.Player.Entity);

            //Move character to proper entry point.
            var playerEntryPoint = scene.Entities.First(x => x?.Get<SceneChangeEvent>()?.SceneChangeId == args.SceneChangeId);
            var entryPointTransform = playerEntryPoint.Transform.LocalToWorld(playerEntryPoint.GetChild(0).Transform.Position);
            this.Player.Entity.Get<CharacterComponent>().Teleport(entryPointTransform);

            UpdateSceneReferences();
        }


        #endregion Methods
    }
}
