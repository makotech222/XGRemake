using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Xenogears.Database
{
    /// <summary>
    /// Describes the sprites associated with an XGCharacter.
    /// </summary>
    [Table("CharacterAnimations")]
    public class XGCharacterAnimation
    {
        public int Id { get; set; }
        public string CharacterName { get; set; }
        public EActionTypes Name { get; set; }

        /// <summary>
        /// Number of frames of this specific action
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// First image file for a specific action, always looking north.
        /// </summary>
        public string StartSprite { get; set; }

        /// <summary>
        /// Sometimes directional animations are spread apart by images inbetween. Skip this many images to get next direction
        /// </summary>
        public int SkipBetweenFiles { get; set; }
    }

    public enum EActionTypes
    {
        Idle,
        Walk,
        Run,
        Jump,
        Climb,
        Action1, // Characters tend to have random different actions, not generalizable between different characters. Things like pressing buttons, crouching, etc.
        Action2,
        Action3,
        Action4,
        Action5
    }
}