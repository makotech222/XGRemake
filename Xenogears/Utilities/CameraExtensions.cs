using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace Xenogears.Utilities
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Converts the world position to clip space coordinates relative to camera.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <returns>
        /// The position in clip space.
        /// </returns>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static Vector3 WorldToClip(this CameraComponent cameraComponent, Vector3 position)
        {
            cameraComponent.WorldToClip(ref position, out var result);
            return result;
        }

        /// <summary>
        /// Converts the world position to clip space coordinates relative to camera.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <param name="result">The position in clip space.</param>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static void WorldToClip(this CameraComponent cameraComponent, ref Vector3 position, out Vector3 result)
        {
            if (cameraComponent == null)
            {
                throw new ArgumentNullException(nameof(cameraComponent));
            }

            Vector3.TransformCoordinate(ref position, ref cameraComponent.ViewProjectionMatrix, out result);
        }

        /// <summary>
        /// Converts the world position to screen space coordinates relative to camera.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <returns>
        /// The screen position in normalized X, Y coordinates. Top-left is (0,0), bottom-right is (1,1). Z is in world units from near camera plane.
        /// </returns>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static Vector3 WorldToScreenPoint(this CameraComponent cameraComponent, Vector3 position)
        {
            cameraComponent.WorldToScreenPoint(ref position, out var result);
            return result;
        }

        /// <summary>
        /// Converts the world position to screen space coordinates relative to camera.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <param name="result">The screen position in normalized X, Y coordinates. Top-left is (0,0), bottom-right is (1,1). Z is in world units from near camera plane.</param>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static void WorldToScreenPoint(this CameraComponent cameraComponent, ref Vector3 position, out Vector3 result)
        {
            cameraComponent.WorldToClip(ref position, out var clipSpace);

            Vector3.TransformCoordinate(ref position, ref cameraComponent.ViewMatrix, out var viewSpace);

            result = new Vector3
            {
                X = (clipSpace.X + 1f) / 2f,
                Y = 1f - (clipSpace.Y + 1f) / 2f,
                Z = viewSpace.Z + cameraComponent.NearClipPlane,
            };
        }

        /// <summary>
        /// Converts the screen position to a <see cref="RaySegment"/> in world coordinates.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <returns><see cref="RaySegment"/>, starting at near plain and ending at the far plain.</returns>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static RaySegment ScreenToWorldRaySegment(this CameraComponent cameraComponent, Vector2 position)
        {
            cameraComponent.ScreenToWorldRaySegment(ref position, out var result);

            return result;
        }

        /// <summary>
        /// Converts the screen position to a <see cref="RaySegment"/> in world coordinates.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position"></param>
        /// <param name="result"><see cref="RaySegment"/>, starting at near plain and ending at the far plain.</param>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static void ScreenToWorldRaySegment(this CameraComponent cameraComponent, ref Vector2 position, out RaySegment result)
        {
            if (cameraComponent == null)
            {
                throw new ArgumentNullException(nameof(cameraComponent));
            }

            Matrix.Invert(ref cameraComponent.ViewProjectionMatrix, out var inverseViewProjection);

            ScreenToClipSpace(ref position, out var clipSpace);

            Vector3.TransformCoordinate(ref clipSpace, ref inverseViewProjection, out var near);

            clipSpace.Z = 1f;
            Vector3.TransformCoordinate(ref clipSpace, ref inverseViewProjection, out var far);

            result = new RaySegment(near, far);
        }

        private static void ScreenToClipSpace(ref Vector2 position, out Vector3 clipSpace)
        {
            clipSpace = new Vector3
            {
                X = position.X * 2f - 1f,
                Y = 1f - position.Y * 2f,
                Z = 0f
            };
        }

        private static Vector3 ScreenToClipSpace(Vector2 position)
        {
            ScreenToClipSpace(ref position, out var result);
            return result;
        }

        /// <summary>
        /// Converts the screen position to a point in world coordinates.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position">The screen position in normalized X, Y coordinates. Top-left is (0,0), bottom-right is (1,1). Z is in world units from near camera plane.</param>
        /// <returns>Position in world coordinates.</returns>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static Vector3 ScreenToWorldPoint(this CameraComponent cameraComponent, Vector3 position)
        {
            cameraComponent.ScreenToWorldPoint(ref position, out var result);

            return result;
        }

        /// <summary>
        /// Converts the screen position to a point in world coordinates.
        /// </summary>
        /// <param name="cameraComponent"></param>
        /// <param name="position">The screen position in normalized X, Y coordinates. Top-left is (0,0), bottom-right is (1,1). Z is in world units from near camera plane.</param>
        /// <param name="result">Position in world coordinates.</param>
        /// <exception cref="ArgumentNullException">If the cameraComponent argument is <see langword="null"/>.</exception>
        /// <remarks>
        /// This method does not update the <see cref="CameraComponent.ViewMatrix"/> or <see cref="CameraComponent.ProjectionMatrix"/> before performing the transformation.
        /// If the <see cref="CameraComponent"/> or it's containing <see cref="Entity"/> <see cref="TransformComponent"/>has been modified since the last frame you may need to call the <see cref="CameraComponent.Update()"/> method first.
        /// </remarks>
        public static void ScreenToWorldPoint(this CameraComponent cameraComponent, ref Vector3 position, out Vector3 result)
        {
            if (cameraComponent == null)
            {
                throw new ArgumentNullException(nameof(cameraComponent));
            }
            var position2D = position.XY();
            //Matrix.Invert(ref cameraComponent.ProjectionMatrix, out var inverseProjection);
            //Matrix.Invert(ref cameraComponent.ViewMatrix, out var inverseView);

            //ScreenToClipSpace(ref position2D, out var clipSpace);
            //Vector3.TransformCoordinate(ref clipSpace, ref inverseProjection, out var near);

            //near.Z = -position.Z;

            //Vector3.TransformCoordinate(ref near, ref inverseView, out result);

            cameraComponent.ScreenToWorldRaySegment(ref position2D, out var ray);

            var direction = ray.End - ray.Start;
            direction.Normalize();

            Vector3.TransformNormal(ref direction, ref cameraComponent.ViewMatrix, out var viewSpaceDir);

            float rayDistance = (position.Z / viewSpaceDir.Z);

            result = ray.Start + (direction * rayDistance);
        }
    }

    /// <summary>
    /// Represents a three dimensional line based on a 2 points in space.
    /// </summary>
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RaySegment : IEquatable<RaySegment>, IFormattable
    {
        private const string ToStringFormat = "Start:{0} End:{1}";

        /// <summary>
        /// The position in three dimensional space where the ray starts.
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// The position in three dimensional space where the ray ends.
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// Initializes a new instance of the <see cref="XenkoToolkit.Mathematics.RaySegment"/> struct.
        /// </summary>
        /// <param name="start">The position in three dimensional space where the ray starts.</param>
        /// <param name="end">The position in three dimensional space where the ray ends.</param>
        public RaySegment(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Length of RaySegment
        /// </summary>
        public float Length => Vector3.Distance(Start, End);

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(RaySegment left, RaySegment right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(RaySegment left, RaySegment right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, Start.ToString(), End.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            return string.Format(CultureInfo.CurrentCulture, ToStringFormat, Start.ToString(format, CultureInfo.CurrentCulture),
                End.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, ToStringFormat, Start.ToString(), End.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, ToStringFormat, Start.ToString(format, formatProvider),
                End.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Stride.Core.Mathematics.Vector4"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Stride.Core.Mathematics.Vector4"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Stride.Core.Mathematics.Vector4"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RaySegment value)
        {
            return Start == value.Start && End == value.End;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (value == null)
                return false;
            if (value.GetType() != GetType())
                return false;
            return Equals((RaySegment)value);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="RaySegment"/> to <see cref="Ray"/>.
        /// </summary>
        /// <param name="raySegment">The <see cref="RaySegment"/> to convert</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Ray(RaySegment raySegment)
        {
            var result = new Ray(raySegment.Start, Vector3.Normalize(raySegment.End - raySegment.Start));

            return result;
        }
    }
}