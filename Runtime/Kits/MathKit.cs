// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using System.Numerics;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for mathematical operations.
    /// </summary>
    public static class MathKit
    {
        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The radian value.</param>
        /// <returns>Returns the degree equivalent of the given <see cref="radians"/>.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (Literals.SemiCircleDegrees / System.Math.PI);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The radian value.</param>
        /// <returns>Returns the radian equivalent of the given <see cref="degrees"/>.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (System.Math.PI / Literals.SemiCircleDegrees);
        }

        /// <summary>
        /// Clamps a <see cref="char"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static char Clamp(char value, char min, char max)
        {
            // Switch on the value.
            return value switch
            {
                _ when value < min => min,
                _ when value > max => max,
                _ => value
            };
        }

        /// <summary>
        /// Clamps a <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static BigInteger Clamp(BigInteger value, BigInteger min, BigInteger max)
        {
            // Switch on the value.
            return value switch
            {
                _ when value < min => min,
                _ when value > max => max,
                _ => value
            };
        }

        /// <summary>
        /// Clamps a <see cref="Vector2"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            value.X = System.Math.Clamp(value.X, min.X, max.X);
            value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Vector3"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            value.X = System.Math.Clamp(value.X, min.X, max.X);
            value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
            value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Vector4"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
        {
            value.X = System.Math.Clamp(value.X, min.X, max.X);
            value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
            value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
            value.W = System.Math.Clamp(value.W, min.W, max.W);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Quaternion"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Quaternion Clamp(Quaternion value, Quaternion min, Quaternion max)
        {
            value.X = System.Math.Clamp(value.X, min.X, max.X);
            value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
            value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
            value.W = System.Math.Clamp(value.W, min.W, max.W);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Complex"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Complex Clamp(Complex value, Complex min, Complex max)
        {
            double real = System.Math.Clamp(value.Real, min.Real, max.Real);
            double imaginary = System.Math.Clamp(value.Imaginary, min.Imaginary, max.Imaginary);
            return new Complex(real, imaginary);
        }

        /// <summary>
        /// Clamps a <see cref="Matrix3x2"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Matrix3x2 Clamp(Matrix3x2 value, Matrix3x2 min, Matrix3x2 max)
        {
            value.M11 = System.Math.Clamp(value.M11, min.M11, max.M11);
            value.M12 = System.Math.Clamp(value.M12, min.M12, max.M12);
            value.M21 = System.Math.Clamp(value.M21, min.M21, max.M21);
            value.M22 = System.Math.Clamp(value.M22, min.M22, max.M22);
            value.M31 = System.Math.Clamp(value.M31, min.M31, max.M31);
            value.M32 = System.Math.Clamp(value.M32, min.M32, max.M32);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Matrix4x4"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Matrix4x4 Clamp(Matrix4x4 value, Matrix4x4 min, Matrix4x4 max)
        {
            value.M11 = System.Math.Clamp(value.M11, min.M11, max.M11);
            value.M12 = System.Math.Clamp(value.M12, min.M12, max.M12);
            value.M13 = System.Math.Clamp(value.M13, min.M13, max.M13);
            value.M14 = System.Math.Clamp(value.M14, min.M14, max.M14);
            value.M21 = System.Math.Clamp(value.M21, min.M21, max.M21);
            value.M22 = System.Math.Clamp(value.M22, min.M22, max.M22);
            value.M23 = System.Math.Clamp(value.M23, min.M23, max.M23);
            value.M24 = System.Math.Clamp(value.M24, min.M24, max.M24);
            value.M31 = System.Math.Clamp(value.M31, min.M31, max.M31);
            value.M32 = System.Math.Clamp(value.M32, min.M32, max.M32);
            value.M33 = System.Math.Clamp(value.M33, min.M33, max.M33);
            value.M34 = System.Math.Clamp(value.M34, min.M34, max.M34);
            value.M41 = System.Math.Clamp(value.M41, min.M41, max.M41);
            value.M42 = System.Math.Clamp(value.M42, min.M42, max.M42);
            value.M43 = System.Math.Clamp(value.M43, min.M43, max.M43);
            value.M44 = System.Math.Clamp(value.M44, min.M44, max.M44);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="Plane"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static Plane Clamp(Plane value, Plane min, Plane max)
        {
            value.Normal = Clamp(value.Normal, min.Normal, max.Normal);
            value.D = System.Math.Clamp(value.D, min.D, max.D);
            return value;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(sbyte value, sbyte min, sbyte max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(byte value, byte min, byte max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(short value, short min, short max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(ushort value, ushort min, ushort max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(long value, long min, long max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(ulong value, ulong min, ulong max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(char value, char min, char max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(BigInteger value, BigInteger min, BigInteger max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(Matrix3x2 value, Matrix3x2 min, Matrix3x2 max)
        {
            return value.M11 >= min.M11 && value.M11 <= max.M11 &&
                   value.M12 >= min.M12 && value.M12 <= max.M12 &&
                   value.M21 >= min.M21 && value.M21 <= max.M21 &&
                   value.M22 >= min.M22 && value.M22 <= max.M22 &&
                   value.M31 >= min.M31 && value.M31 <= max.M31 &&
                   value.M32 >= min.M32 && value.M32 <= max.M32;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(Matrix4x4 value, Matrix4x4 min, Matrix4x4 max)
        {
            return value.M11 >= min.M11 && value.M11 <= max.M11 &&
                   value.M12 >= min.M12 && value.M12 <= max.M12 &&
                   value.M13 >= min.M13 && value.M13 <= max.M13 &&
                   value.M14 >= min.M14 && value.M14 <= max.M14 &&
                   value.M21 >= min.M21 && value.M21 <= max.M21 &&
                   value.M22 >= min.M22 && value.M22 <= max.M22 &&
                   value.M23 >= min.M23 && value.M23 <= max.M23 &&
                   value.M24 >= min.M24 && value.M24 <= max.M24 &&
                   value.M31 >= min.M31 && value.M31 <= max.M31 &&
                   value.M32 >= min.M32 && value.M32 <= max.M32 &&
                   value.M33 >= min.M33 && value.M33 <= max.M33 &&
                   value.M34 >= min.M34 && value.M34 <= max.M34 &&
                   value.M41 >= min.M41 && value.M41 <= max.M41 &&
                   value.M42 >= min.M42 && value.M42 <= max.M42 &&
                   value.M43 >= min.M43 && value.M43 <= max.M43 &&
                   value.M44 >= min.M44 && value.M44 <= max.M44;
        }
        
        /// <summary>
        /// Finds the minimum value within a collection.
        /// </summary>
        /// <param name="values">The values to check.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>Returns the minimum value.</returns>
        public static T Min<T>(params T[] values) where T : System.IComparable<T>
        {
            int count = values.Length;
            T min = values[0];

            // Iterate through all values.
            for (int i = 1; i < count; i++)
            {
                T value = values[i];

                // If the current value is greater than the current max, set it as the max.
                if (value.CompareTo(min) < 0)
                    min = value;
            }

            return min;
        }
        
        /// <summary>
        /// Finds the minimum value within a collection.
        /// </summary>
        /// <param name="values">The values to check.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>Returns the minimum value.</returns>
        public static T Min<T>(IList<T> values) where T : System.IComparable<T>
        {
            int count = values.Count;
            T min = values[0];

            // Iterate through all values.
            for (int i = 1; i < count; i++)
            {
                T value = values[i];

                // If the current value is greater than the current max, set it as the max.
                if (value.CompareTo(min) < 0)
                    min = value;
            }

            return min;
        }

        /// <summary>
        /// Finds the maximum value within a collection.
        /// </summary>
        /// <param name="values">The values to check.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>Returns the maximum value.</returns>
        public static T Max<T>(params T[] values) where T : System.IComparable<T>
        {
            int count = values.Length;
            T max = values[0];

            // Iterate through all values.
            for (int i = 1; i < count; i++)
            {
                T value = values[i];

                // If the current value is greater than the current max, set it as the max.
                if (value.CompareTo(max) > 0)
                    max = value;
            }

            return max;
        }
        
        /// <summary>
        /// Finds the maximum value within a collection.
        /// </summary>
        /// <param name="values">The values to check.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>Returns the maximum value.</returns>
        public static T Max<T>(IList<T> values) where T : System.IComparable<T>
        {
            int count = values.Count;
            T max = values[0];

            // Iterate through all values.
            for (int i = 1; i < count; i++)
            {
                T value = values[i];

                // If the current value is greater than the current max, set it as the max.
                if (value.CompareTo(max) > 0)
                    max = value;
            }

            return max;
        }

        /// <summary>
        /// Checks if two values are nearly equal to each other, within a tolerance range.
        /// </summary>
        /// <param name="a">The first value to check.</param>
        /// <param name="b">The second value to check.</param>
        /// <param name="tolerance">The amount of equality tolerance.</param>
        /// <returns>Returns if <paramref name="a"/> is nearly equal to <paramref name="b"/>.</returns>
        public static bool IsNearlyEqual(float a, float b, float tolerance = 0.0001f)
        {
            return System.Math.Abs(a - b) <= tolerance;
        }

        /// <summary>
        /// Checks if a value is infinity or not a number.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Returns if the <paramref name="value"/> is not a number.</returns>
        public static bool IsInvalid(this float value)
        {
            return float.IsInfinity(value) || float.IsNaN(value);
        }

        /// <summary>
        /// Checks if a value is infinity or not a number.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Returns if the <paramref name="value"/> is not a number.</returns>
        public static bool IsInvalid(this double value)
        {
            return double.IsInfinity(value) || double.IsNaN(value);
        }

#if UNITY_64
        /// <summary>
        /// Clamps a <see cref="UnityEngine.Vector2"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Vector2 Clamp(UnityEngine.Vector2 value, UnityEngine.Vector2 min, UnityEngine.Vector2 max)
        {
            value.x = System.Math.Clamp(value.x, min.x, max.x);
            value.y = System.Math.Clamp(value.y, min.y, max.y);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="UnityEngine.Vector3"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Vector3 Clamp(UnityEngine.Vector3 value, UnityEngine.Vector3 min, UnityEngine.Vector3 max)
        {
            value.x = System.Math.Clamp(value.x, min.x, max.x);
            value.y = System.Math.Clamp(value.y, min.y, max.y);
            value.z = System.Math.Clamp(value.z, min.z, max.z);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="UnityEngine.Vector4"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Vector4 Clamp(UnityEngine.Vector4 value, UnityEngine.Vector4 min, UnityEngine.Vector4 max)
        {
            value.x = System.Math.Clamp(value.x, min.x, max.x);
            value.y = System.Math.Clamp(value.y, min.y, max.y);
            value.z = System.Math.Clamp(value.z, min.z, max.z);
            value.w = System.Math.Clamp(value.w, min.w, max.w);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="UnityEngine.Quaternion"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Quaternion Clamp(UnityEngine.Quaternion value, UnityEngine.Quaternion min, UnityEngine.Quaternion max)
        {
            value.x = System.Math.Clamp(value.x, min.x, max.x);
            value.y = System.Math.Clamp(value.y, min.y, max.y);
            value.z = System.Math.Clamp(value.z, min.z, max.z);
            value.w = System.Math.Clamp(value.w, min.w, max.w);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="UnityEngine.Matrix4x4"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Matrix4x4 Clamp(UnityEngine.Matrix4x4 value, UnityEngine.Matrix4x4 min, UnityEngine.Matrix4x4 max)
        {
            value.m00 = System.Math.Clamp(value.m00, min.m00, max.m00);
            value.m01 = System.Math.Clamp(value.m01, min.m01, max.m01);
            value.m02 = System.Math.Clamp(value.m02, min.m02, max.m02);
            value.m03 = System.Math.Clamp(value.m03, min.m03, max.m03);
            value.m10 = System.Math.Clamp(value.m10, min.m10, max.m10);
            value.m11 = System.Math.Clamp(value.m11, min.m11, max.m11);
            value.m12 = System.Math.Clamp(value.m12, min.m12, max.m12);
            value.m13 = System.Math.Clamp(value.m13, min.m13, max.m13);
            value.m20 = System.Math.Clamp(value.m20, min.m20, max.m20);
            value.m21 = System.Math.Clamp(value.m21, min.m21, max.m21);
            value.m22 = System.Math.Clamp(value.m22, min.m22, max.m22);
            value.m23 = System.Math.Clamp(value.m23, min.m23, max.m23);
            value.m30 = System.Math.Clamp(value.m30, min.m30, max.m30);
            value.m31 = System.Math.Clamp(value.m31, min.m31, max.m31);
            value.m32 = System.Math.Clamp(value.m32, min.m32, max.m32);
            value.m33 = System.Math.Clamp(value.m33, min.m33, max.m33);
            return value;
        }

        /// <summary>
        /// Clamps a <see cref="UnityEngine.Plane"/> value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns a clamped <paramref name="value"/>.</returns>
        public static UnityEngine.Plane Clamp(UnityEngine.Plane value, UnityEngine.Plane min, UnityEngine.Plane max)
        {
            value.normal = Clamp(value.normal, min.normal, max.normal);
            value.distance = System.Math.Clamp(value.distance, min.distance, max.distance);
            return value;
        }

        /// <summary>
        /// Checks if a value is in between two extremes, inclusive.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum value, inclusive.</param>
        /// <param name="max">The maximum value, exclusive.</param>
        /// <returns>Returns if <paramref name="value"/> is within the range.</returns>
        public static bool InRange(UnityEngine.Matrix4x4 value, UnityEngine.Matrix4x4 min, UnityEngine.Matrix4x4 max)
        {
            return value.m00 >= min.m00 && value.m00 <= max.m00 &&
                   value.m01 >= min.m01 && value.m01 <= max.m01 &&
                   value.m02 >= min.m02 && value.m02 <= max.m02 &&
                   value.m03 >= min.m03 && value.m03 <= max.m03 &&
                   value.m10 >= min.m10 && value.m10 <= max.m10 &&
                   value.m11 >= min.m11 && value.m11 <= max.m11 &&
                   value.m12 >= min.m12 && value.m12 <= max.m12 &&
                   value.m13 >= min.m13 && value.m13 <= max.m13 &&
                   value.m20 >= min.m20 && value.m20 <= max.m20 &&
                   value.m21 >= min.m21 && value.m21 <= max.m21 &&
                   value.m22 >= min.m22 && value.m22 <= max.m22 &&
                   value.m23 >= min.m23 && value.m23 <= max.m23 &&
                   value.m30 >= min.m30 && value.m30 <= max.m30 &&
                   value.m31 >= min.m31 && value.m31 <= max.m31 &&
                   value.m32 >= min.m32 && value.m32 <= max.m32 &&
                   value.m33 >= min.m33 && value.m33 <= max.m33;
        }
#endif
    }
}