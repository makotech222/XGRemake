using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xenogears.Database
{
    /// <summary>
    /// Contains the possible states of the game. Used to determine how far character has progressed, and which NPC dialogue to load when being talked with. Also important for save/load
    /// Naming scheme will generally be "{Location}_{Action}" To help human readability
    /// </summary>
    [Table("GameState")]
    public class XGGameState
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User friendly name for game state.
        /// </summary>
        public string Name { get; set; }
    }
}
