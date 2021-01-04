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

namespace Xenogears.Gameplay
{
    /// <summary>
    /// Event that changes scene when interacted with, i.e moving through doors, clicking buttons, etc.
    /// </summary>
    public class SceneChangeEvent : ScriptedEvent
    {
        #region Fields

        #endregion Fields

        #region Properties
        /// <summary>
        /// Id of this scene change event
        /// </summary>
        public int SceneChangeId { get; set; }
        /// <summary>
        /// Name of the scene that will be used.
        /// </summary>
        public string NextSceneName { get; set; }
        /// <summary>
        /// Matches to a scene change event in the next scene where character will be positioned
        /// </summary>
        public int NextSceneChangeId { get; set; }

        #endregion Properties

        #region Events

        #endregion Events

        #region Methods

        public override void Start()
        {
            base.Start();
        }

        public override void Update(InputComponent input, PlayerFieldCharacter character)
        {
            base.Update(input, character);
        }

        protected override void Execute(InputComponent input, PlayerFieldCharacter character)
        {
            base.Execute(input, character);
            if (this.NextSceneName.NullIfEmpty() == null)
                return;
            Core.Instance.UpdateScene(new SceneChangeArgs()

            {
                SceneName = NextSceneName,
                SceneChangeId = NextSceneChangeId,
                PlayerFieldCharacter = character.Serialize()
            });
        }

        #endregion Methods

    }

    public class SceneChangeArgs
    {
        public string SceneName { get; set; }
        public int SceneChangeId { get; set; }
        public FieldCharacterSerialization PlayerFieldCharacter { get; set; }


    }
}