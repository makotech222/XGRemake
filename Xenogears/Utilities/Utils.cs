using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;
using Xenogears.Core;

namespace Xenogears.Utilities
{
    public static class Utils
    {
        public static Vector3 LogicDirectionToWorldDirection(Vector2 logicDirection, CameraComponent camera, Vector3 upVector)
        {
            var inverseView = Matrix.Invert(camera.ViewMatrix);

            var forward = Vector3.Cross(upVector, inverseView.Right);
            forward.Normalize();

            var right = Vector3.Cross(forward, upVector);
            var worldDirection = forward * logicDirection.Y + right * logicDirection.X;
            worldDirection.Normalize();
            return worldDirection;
        }

        public static bool ScreenPositionToWorldPositionRaycast(Vector2 screenPos, CameraComponent camera, Simulation simulation, out ClickResult clickResult)
        {
            Matrix invViewProj = Matrix.Invert(camera.ViewProjectionMatrix);

            Vector3 sPos;
            sPos.X = screenPos.X * 2f - 1f;
            sPos.Y = 1f - screenPos.Y * 2f;

            sPos.Z = 0f;
            var vectorNear = Vector3.Transform(sPos, invViewProj);
            vectorNear /= vectorNear.W;

            sPos.Z = 1f;
            var vectorFar = Vector3.Transform(sPos, invViewProj);
            vectorFar /= vectorFar.W;

            clickResult.ClickedEntity = null;
            clickResult.WorldPosition = Vector3.Zero;
            clickResult.HitResult = new HitResult();

            var minDistance = float.PositiveInfinity;

            List<HitResult> result = new List<HitResult>();
            simulation.RaycastPenetrating(vectorNear.XYZ(), vectorFar.XYZ(), result);
            foreach (var hitResult in result)
            {
                var staticBody = hitResult.Collider;
                if (staticBody != null)
                {
                    var distance = (vectorNear.XYZ() - hitResult.Point).LengthSquared();
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        clickResult.HitResult = hitResult;
                        clickResult.WorldPosition = hitResult.Point;
                        clickResult.ClickedEntity = hitResult.Collider.Entity;
                    }
                }
            }

            return (clickResult.ClickedEntity != null);
        }
    }
}