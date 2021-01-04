using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace Xenogears.Core
{
    /// <summary>
    /// Result of the user clicking/tapping on the world
    /// </summary>
    public struct ClickResult
    {
        /// <summary>
        /// The world-space position of the click, where the raycast hits the collision body
        /// </summary>
        public Vector3 WorldPosition;

        /// <summary>
        /// The Entity containing the collision body which was hit
        /// </summary>
        public Entity ClickedEntity;

        /// <summary>
        /// The HitResult received from the physics simulation
        /// </summary>
        public HitResult HitResult;
    }
}