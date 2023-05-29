// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Security.Cryptography;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for random number generations.
    /// </summary>
    public static class RandomKit
    {
        /// <summary>The max hi for making a <see cref="decimal"/> with uniform distribution.</summary>
        private static readonly int MaxDecimalHi = 542101086;
        
        /// <summary>The max scale for a <see cref="decimal"/>.</summary>
        private static readonly byte MaxDecimalScale = 28;
        
        /// <summary>The default generator to use.</summary>
        public static Random DefaultGenerator { get; private set; }

        /// <summary>The default cryptographic generator to use.</summary>
        public static RandomNumberGenerator DefaultCryptoGenerator { get; private set; }

        static RandomKit()
        {
            DefaultGenerator = new RejectionRandom();
            DefaultCryptoGenerator = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Sets the <see cref="DefaultGenerator"/> to a new one.
        /// </summary>
        /// <param name="generator">The new <see cref="Random"/> generator.</param>
        public static void SetDefaultGenerator(Random generator)
        {
            DefaultGenerator = generator;
        }

        /// <summary>
        /// Sets the <see cref="DefaultCryptoGenerator"/> to a new one.
        /// </summary>
        /// <param name="generator">The new <see cref="RandomNumberGenerator"/> generator.</param>
        public static void SetDefaultCryptoGenerator(RandomNumberGenerator generator)
        {
            DefaultCryptoGenerator = generator;
        }

        /// <summary>
        /// Gets a set of <see cref="byte"/>s from a random generator.
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <param name="count">The number of <see cref="byte"/>s to generate.</param>
        /// <param name="bytes">The array to store the result in.</param>
        public static void GetRandomBytes(this Random random, int count, out byte[] bytes)
        {
            bytes = new byte[count];
            random.NextBytes(bytes);
        }

        /// <summary>
        /// Gets a set of <see cref="byte"/>s from a random generator.
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <param name="count">The number of <see cref="byte"/>s to generate.</param>
        /// <returns>Returns the generated <see cref="byte"/>s.</returns>
        public static byte[] GetRandomBytes(this Random random, int count)
        {
            random.GetRandomBytes(count, out byte[] result);
            return result;
        }

        /// <summary>
        /// Gets a set of <see cref="byte"/>s from a random generator.
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <param name="count">The number of <see cref="byte"/>s to generate.</param>
        /// <param name="bytes">The array to store the result in.</param>
        public static void GetRandomBytes(this RandomNumberGenerator random, int count, out byte[] bytes)
        {
            bytes = new byte[count];
            random.GetBytes(bytes);
        }

        /// <summary>
        /// Gets a set of <see cref="byte"/>s from a random generator.
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <param name="count">The number of <see cref="byte"/>s to generate.</param>
        /// <returns>Returns the generated <see cref="byte"/>s.</returns>
        public static byte[] GetRandomBytes(this RandomNumberGenerator random, int count)
        {
            random.GetRandomBytes(count, out byte[] result);
            return result;
        }

        /// <summary>
        /// Gets a random <see cref="int"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static int GetRandomIntII(this Random random, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            MinMaxException<int>.CheckAndThrow(minValue, maxValue, true);
            
            // Get the difference between the max and min. The max difference is the max uint value.
            uint difference = (uint)(maxValue - minValue);

            // If the difference is for the full inclusive int range, return using the byte method.
            if (difference >= uint.MaxValue)
            {
                random.GetRandomBytes(sizeof(int), out byte[] bytes);
                return BitConverter.ToInt32(bytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // When maxValue is an int's max value, special care needs to be used by shifting down by
            // 1, then adding one after generating a value.
            if (maxValue == int.MaxValue)
                return random.Next(minValue - 1, (int)(minValue - 1 + difference)) + 1;

            // Otherwise, simply return on the given range. Add 1 due to [I, E) returns on Next.
            return random.Next(minValue, maxValue + 1);
        }

        /// <summary>
        /// Gets a random <see cref="int"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static int GetRandomIntII(this RandomNumberGenerator random, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            MinMaxException<int>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            // Get the difference between the min and max values.
            uint difference = (uint)maxValue - (uint)minValue;

            // If the difference is the full int range, merely return using the byte method.
            if (difference == uint.MaxValue)
            {
                random.GetRandomBytes(sizeof(int), out byte[] randomBytes);
                return BitConverter.ToInt32(randomBytes, 0);
            }

            difference += 1; // Increase difference by one, due to the new inclusivity of the maxValue.

            byte[] bytes = new byte[sizeof(uint)]; // Create a byte array.

            // Get an allowed maximum based on our range.
            uint maxAllowedRandomValue = uint.MaxValue - uint.MaxValue % difference;

            uint value; // The randomly generated value.

            // Generating with cryptographic safety is slow, as it requires this while loop.
            // Larger ranges are faster. The value must be less than the allowed maximum.
            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (int)(minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="int"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static int GetRandomIntIE(this RandomNumberGenerator random, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            MinMaxException<int>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            uint value; // The randomly generated value.

            // Get the difference between the min and max values.
            uint difference = (uint)maxValue - (uint)minValue;
            byte[] bytes = new byte[sizeof(uint)];

            // Get an allowed maximum based on our range.
            uint maxAllowedRandomValue = uint.MaxValue - uint.MaxValue % difference;

            // Generating with cryptographic safety is slow, as it requires this while loop.
            // Larger ranges are faster. The value must be less than the allowed maximum.
            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (int)(minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="uint"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static uint GetRandomUIntII(this Random random, uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            MinMaxException<uint>.CheckAndThrow(minValue, maxValue, true);
            
            // Get the difference between the max and min. The max difference is the max uint value.
            uint difference = maxValue - minValue;

            // If the difference is for the full inclusive uint range, return using the byte method.
            if (difference >= uint.MaxValue)
            {
                random.GetRandomBytes(sizeof(uint), out byte[] bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // When the difference is less than an int's max value, we can perform a standard Next.
            if (difference < int.MaxValue)
                return (uint)random.Next(0, (int)difference) + minValue;

            // Otherwise, special care is needed to handle wrapping around the normally negative values.
            // Get the wrapped values, and use it to create a wrap after the min value.
            long wrap = (long)int.MaxValue - difference;
            return (uint)(random.Next((int)wrap, int.MaxValue) - wrap + minValue);
        }

        /// <summary>
        /// Gets a random <see cref="uint"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static uint GetRandomUIntIE(this Random random, uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            MinMaxException<uint>.CheckAndThrow(minValue, maxValue, true);
            
            // Get the difference between the max and min. The max difference is the max uint value.
            uint difference = maxValue - minValue;

            // When the difference is less than an int's max value, we can perform a standard Next.
            if (difference < int.MaxValue)
                return (uint)random.Next(0, (int)difference) + minValue;

            // Otherwise, special care is needed to handle wrapping around the normally negative values.
            // Get the wrapped values, and use it to create a wrap after the min value.
            long wrap = (long)int.MaxValue - difference;
            return (uint)(random.Next((int)wrap, int.MaxValue) - wrap + minValue);
        }

        /// <summary>
        /// Gets a random <see cref="uint"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static uint GetRandomUIntII(this RandomNumberGenerator random, uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            MinMaxException<uint>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            // Get the difference between the min and max values.
            uint difference = maxValue - minValue;

            // If the difference is the full int range, merely return using the byte method.
            if (difference >= uint.MaxValue)
            {
                random.GetRandomBytes(sizeof(int), out byte[] randomBytes);
                return BitConverter.ToUInt32(randomBytes, 0);
            }

            difference += 1; // Increase difference by one, due to the new inclusivity of the maxValue.

            byte[] bytes = new byte[sizeof(uint)]; // Create a byte array.

            // Get an allowed maximum based on our range.
            uint maxAllowedRandomValue = uint.MaxValue - uint.MaxValue % difference;

            uint value; // The randomly generated value.

            // Generating with cryptographic safety is slow, as it requires this while loop.
            // Larger ranges are faster. The value must be less than the allowed maximum.
            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="uint"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static uint GetRandomUIntIE(this RandomNumberGenerator random, uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            MinMaxException<uint>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            uint value; // The randomly generated value.

            // Get the difference between the min and max values.
            uint difference = maxValue - minValue;
            byte[] bytes = new byte[sizeof(uint)]; // Create a byte array.

            // Get an allowed maximum based on our range.
            uint maxAllowedRandomValue = uint.MaxValue - uint.MaxValue % difference;

            // Generating with cryptographic safety is slow, as it requires this while loop.
            // Larger ranges are faster. The value must be less than the allowed maximum.
            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt32(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="long"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static long GetRandomLongII(this Random random, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            MinMaxException<long>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = (ulong)maxValue - (ulong)minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // If the difference is for the full inclusive long range, return using the byte method.
            if (difference == ulong.MaxValue)
            {
                random.GetRandomBytes(sizeof(long), out byte[] randomBytes);
                return BitConverter.ToInt64(randomBytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // Most random generators only generate up to an int, rather than a long. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.NextBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (long)((ulong)minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="long"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static long GetRandomLongIE(this Random random, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            MinMaxException<long>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = (ulong)maxValue - (ulong)minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // Most random generators only generate up to an int, rather than a long. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.NextBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (long)((ulong)minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="long"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static long GetRandomLongII(this RandomNumberGenerator random, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            MinMaxException<long>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = (ulong)maxValue - (ulong)minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // If the difference is for the full inclusive long range, return using the byte method.
            if (difference == ulong.MaxValue)
            {
                random.GetRandomBytes(sizeof(long), out byte[] randomBytes);
                return BitConverter.ToInt64(randomBytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // Most random generators only generate up to an int, rather than a long. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (long)((ulong)minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="long"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static long GetRandomLongIE(this RandomNumberGenerator random, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            MinMaxException<long>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = (ulong)maxValue - (ulong)minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // Most random generators only generate up to an int, rather than a long. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return (long)((ulong)minValue + value % difference);
        }

        /// <summary>
        /// Gets a random <see cref="ulong"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static ulong GetRandomULongII(this Random random, ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            MinMaxException<ulong>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = maxValue - minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // If the difference is for the full inclusive ulong range, return using the byte method.
            if (difference == ulong.MaxValue)
            {
                random.GetRandomBytes(sizeof(long), out byte[] randomBytes);
                return BitConverter.ToUInt64(randomBytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // Most random generators only generate up to an int, rather than a ulong. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.NextBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="ulong"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static ulong GetRandomULongIE(this Random random, ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            MinMaxException<ulong>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = maxValue - minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // Most random generators only generate up to an int, rather than a ulong. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.NextBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="ulong"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The INCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static ulong GetRandomULongII(this RandomNumberGenerator random, ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            MinMaxException<ulong>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = maxValue - minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // If the difference is for the full inclusive ulong range, return using the byte method.
            if (difference == ulong.MaxValue)
            {
                random.GetRandomBytes(sizeof(long), out byte[] randomBytes);
                return BitConverter.ToUInt64(randomBytes, 0);
            }

            difference += 1; // Add an extra point, as any Next function is inherently [I, E).

            // Most random generators only generate up to an int, rather than a ulong. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="ulong"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static ulong GetRandomULongIE(this RandomNumberGenerator random, ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            MinMaxException<ulong>.CheckAndThrow(minValue, maxValue, true);
            
            if (minValue == maxValue)
                return minValue;

            ulong value; // The randomly generated value.

            // Get the difference between the max and min. The max difference is the max uint value.
            ulong difference = maxValue - minValue;
            byte[] bytes = new byte[sizeof(ulong)]; // Create a byte array.

            // Most random generators only generate up to an int, rather than a ulong. Because of this
            // general limitation, we must use the remainder method.
            ulong maxAllowedRandomValue = ulong.MaxValue - ulong.MaxValue % difference;

            do
            {
                // Generate a random uint, to keep positive.
                random.GetBytes(bytes);
                value = BitConverter.ToUInt64(bytes, 0);
            }
            while (value >= maxAllowedRandomValue);

            // Modulo the value to fix the range and return it.
            return minValue + value % difference;
        }

        /// <summary>
        /// Gets a random <see cref="double"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static double GetRandomDoubleIE(this Random random, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            MinMaxException<double>.CheckAndThrow(minValue, maxValue, true);
            
            double value = random.NextDouble();
            return Math.Clamp(maxValue * value + minValue * (1d - value), minValue, maxValue);
        }

        /// <summary>
        /// Gets a random <see cref="double"/> on the range of [0.0, 1.0).
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <returns>Returns thea <see cref="double"/> on the range of [0.0, 1.0).</returns>
        public static double GetNextDouble(this RandomNumberGenerator random)
        {
            random.GetRandomBytes(sizeof(ulong), out byte[] bytes);
            ulong value = BitConverter.ToUInt64(bytes, 0);

            // We only have 53 bits to work with for generating a secure double.
            value &= (1ul << Literals.MaxSecureDoubleBits) - 1;

            // Return the value, divided by the bit mask.
            return (double)value / (1ul << Literals.MaxSecureDoubleBits);
        }
        
        /// <summary>
        /// Gets a random <see cref="double"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static double GetRandomDoubleIE(this RandomNumberGenerator random, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            MinMaxException<double>.CheckAndThrow(minValue, maxValue, true);
            
            double value = random.GetNextDouble();
            return Math.Clamp(maxValue * value + minValue * (1d - value), minValue, maxValue);
        }

        /// <summary>
        /// Gets a random <see cref="decimal"/> on the range of [0.0, 1.0).
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <returns>Returns thea <see cref="decimal"/> on the range of [0.0, 1.0).</returns>
        public static decimal GetNextDecimal(this Random random)
        {
            return new decimal(random.Next(), random.Next(), random.Next(0, MaxDecimalHi), false, MaxDecimalScale);
        }
        
        /// <summary>
        /// Gets a random <see cref="decimal"/> on the range of [0.0, 1.0).
        /// </summary>
        /// <param name="random">The generator to use.</param>
        /// <returns>Returns thea <see cref="decimal"/> on the range of [0.0, 1.0).</returns>
        public static decimal GetNextDecimal(this RandomNumberGenerator random)
        {
            return new decimal(random.GetRandomIntIE(), random.GetRandomIntIE(), random.GetRandomIntIE(0, MaxDecimalHi), false, MaxDecimalScale);
        }
        
        /// <summary>
        /// Gets a random <see cref="decimal"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static decimal GetRandomDecimalIE(this Random random, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            MinMaxException<decimal>.CheckAndThrow(minValue, maxValue, true);
            
            decimal value = random.GetNextDecimal();
            return (maxValue * value) + (minValue * (1m - value));
        }
        
        /// <summary>
        /// Gets a random <see cref="decimal"/> from a random generator.
        /// </summary>
        /// <param name="random">/The generator to use.</param>
        /// <param name="minValue">The INCLUSIVE minimum value.</param>
        /// <param name="maxValue">The EXCLUSIVE maximum value.</param>
        /// <returns>Returns the generated value.</returns>
        public static decimal GetRandomDecimalIE(this RandomNumberGenerator random, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            MinMaxException<decimal>.CheckAndThrow(minValue, maxValue, true);
            
            decimal value = random.GetNextDecimal();
            return (maxValue * value) + (minValue * (1m - value));
        }
    }
}