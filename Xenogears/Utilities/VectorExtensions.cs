using System;
using Stride.Core.Mathematics;

namespace Xenogears.Utilities
{
    public static class VectorExtensions
    {
        private const double DegToRad = Math.PI / 180;

        public static Vector2 Rotate(this Vector2 v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector2 RotateRadians(this Vector2 v, double radians)
        {
            var ca = Convert.ToSingle(Math.Cos(radians));
            var sa = Convert.ToSingle(Math.Sin(radians));
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public static bool RoughlyEquals(this Vector3 v, Vector3 other)
        {
            float diff = 0.1f;
            if (Math.Abs(v.X - other.X) >= diff)
                return false;
            if (Math.Abs(v.Y - other.Y) >= diff)
                return false;
            if (Math.Abs(v.Z - other.Z) >= diff)
                return false;

            return true;
        }

        public static Vector3 Forward(this Vector3 rotationInDegrees)
        {

            float X = (float)(Math.Sin(rotationInDegrees.Y) * Math.Cos(rotationInDegrees.X));
            float Y = (float)Math.Sin(-rotationInDegrees.X);
            float Z = (float)(Math.Cos(rotationInDegrees.X) * Math.Cos(rotationInDegrees.Y));
            return new Vector3(X, Y, Z);
        }

        public static Vector3 Multiply(this Quaternion rotation, Vector3 point)
        {
            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3 vector3;
            vector3.X = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.X + ((double)num7 - (double)num12) * (double)point.Y + ((double)num8 + (double)num11) * (double)point.Z);
            vector3.Y = (float)(((double)num7 + (double)num12) * (double)point.X + (1.0 - ((double)num4 + (double)num6)) * (double)point.Y + ((double)num9 - (double)num10) * (double)point.Z);
            vector3.Z = (float)(((double)num8 - (double)num11) * (double)point.X + ((double)num9 + (double)num10) * (double)point.Y + (1.0 - ((double)num4 + (double)num5)) * (double)point.Z);
            return vector3;
        }
    }
}