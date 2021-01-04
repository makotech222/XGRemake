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
    /// Dialogues that occur when moving around the world. Some NPCs will talk to you, but you can still move around.
    /// </summary>
    public class MinorDialogueEvent : ScriptedEvent
    {
        #region Fields

        #endregion Fields

        #region Properties
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

        #endregion Methods
    }
}