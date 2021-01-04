using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xenogears.Database
{
    /// <summary>
    /// Main Character root table.
    /// </summary>
    [Table("Characters")]
    public class XGCharacter
    {
        [Key]
        public string Name { get; set; }

        public virtual List<XGCharacterAnimation> RawAnimationsData { get; set; }
    }
}
