// Copyright (c) Craig Williams, SlashParadox

using System;
using SlashParadox.Essence.Kits;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A random number generating class. This is based on the Rejection Method detailed in
    /// 'Numerical Recipes for C [Second Edition] (1992)', the same function that .NET's
    /// <see cref="Random"/> is based on. This version comes with a key improvement.
    /// A mistyped '21' in the original class is now a proper '31'. This is not fixed
    /// in the original class due to compatibility issues.
    /// </summary>
    public class RejectionRandom : Random
    {
        /// <summary>A giant addition value. This can be any number, according to D.E. Knuth.</summary>
        private static readonly int MBIG = int.MaxValue;

        /// <summary>The starting seed value. This must be smaller than <see cref="MBIG"/>.</summary>
        private static readonly int MSEED = 161803398;

        /// <summary>The min value of a seed before it is added onto.</summary>
        private static readonly int MINVALUE = 0;

        /// <summary>The size of the buffer, specified by D.E. Knuth.</summary>
        private static readonly int KnuthsSize = 56;

        /// <summary>The starting value of <see cref="_inextp"/>, specified by D.E. Knuth.</summary>
        private static readonly int KnuthsConstant = 31;

        /// <summary>A multiplier to turn an randomly generated integer into a floating point.</summary>
        private static readonly double FloatingPointMultiplier = 4.6566128752457969E-10;

        /// <summary>The buffer for the seeded values.</summary>
        private readonly int[] _seedArray = new int[KnuthsSize];

        /// <summary>The lower accessor variable into the <see cref="_seedArray"/> buffer.</summary>
        private int _inext;

        /// <summary>The higher accessor variable into the <see cref="_seedArray"/> buffer.</summary>
        private int _inextp = KnuthsConstant;

        /// <summary>
        /// A default constructor for a <see cref="RejectionRandom"/> generator. This will create
        /// a generator with the current <see cref="System.Environment.TickCount"/> as a seed.
        /// </summary>
        public RejectionRandom() : this(Environment.TickCount) { }

        /// <summary>
        /// A constructor for a <see cref="RejectionRandom"/> generator This will create a generator
        /// with the given <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">The seed to use with this generator.</param>
        public RejectionRandom(int seed)
        {
            int bufferSize = KnuthsSize - 1; // Get the buffer size.

            int value0 = MSEED - Math.Abs(seed); // Get a starting seed value.
            _seedArray[bufferSize] = value0; // Insert as the last element in the buffer.

            int value1 = 1; // Get another seed value.

            for (int i = 1; i < bufferSize; i++)
            {
                int index = 21 * i % bufferSize; // Get an index into the buffer.
                _seedArray[index] = value1; // Set the previous value into the buffer.

                // Update the second value. If negative, add a large number on to it.
                value1 = value0 - value1;
                if (value1 < MINVALUE)
                    value1 += MBIG;

                // Add more random seeds to the buffer.
                for (int j = 1; j < 5; j++)
                {
                    for (int k = 1; k < KnuthsSize; k++)
                    {
                        int value2 = _seedArray[k] - _seedArray[1 + (k + 30) % bufferSize];
                        if (value2 < MINVALUE)
                            value2 += MBIG;

                        _seedArray[k] = value2;
                    }
                }

                // Safety set. Can be removed.
                _inext = 0;
                _inextp = KnuthsConstant;
            }
        }

        /// <summary>
        /// Fills a given array of bytes with random values.
        /// </summary>
        /// <param name="buffer">The array to place values into. This must be sized to the number of
        /// bytes to generate.</param>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer.IsEmptyOrNull())
                return;

            int length = buffer.Length;
            for (int i = 0; i < length; i++)
            {
                buffer[i] = (byte)(GetIntSample() % (byte.MaxValue + 1));
            }
        }

        /// <summary>
        /// Gets the next non-negative integer in the seed, in a range of
        /// [0, <see cref="int.MaxValue"/>).
        /// </summary>
        /// <returns>Returns a pseudo-random, non-negative integer from the current seed.</returns>
        public override int Next()
        {
            return GetIntSample();
        }

        /// <summary>
        /// Gets a random integer in a range of
        /// [<paramref name="minValue"/>, <paramref name="maxValue"/>).
        /// </summary>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <returns>Returns a random integer within the range.</returns>
        public override int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new MinMaxException<int>(minValue, maxValue, true);

            long difference = (long)maxValue - minValue;
            if (difference <= int.MaxValue)
                return (int)(Sample() * difference) + minValue;

            return (int)((long)(GetLargeDoubleSample() * difference) + minValue);
        }

        /// <summary>
        /// Gets the next double in the seed, in a range of [0.0, 1.0).
        /// </summary>
        /// <returns>Returns a pseudo-random double, valued from 0.0 to 1.0.</returns>
        public override double NextDouble()
        {
            return Sample(); // Return a double sample.
        }

        /// <summary>
        /// Gets a double sample value randomly between 0.0 and 1.0.
        /// </summary>
        /// <returns>Returns a random double value between 0.0 and 1.0.</returns>
        protected override double Sample()
        {
            // Get an int sample, and turn it into a double.
            return GetIntSample() * FloatingPointMultiplier;
        }

        /// <summary>
        /// Gets an <see cref="int"/> sample value randomly.
        /// </summary>
        /// <returns>Returns a random integer value.</returns>
        protected virtual int GetIntSample()
        {
            // Increment the inext and inextp indexes, wrapping back to one at max index.
            if (++_inext >= KnuthsSize)
                _inext = 1;
            if (++_inextp >= KnuthsSize)
                _inextp = 1;

            // Generate a new value via subtraction. Fix it to make sure it stays on range.
            int value = _seedArray[_inext] - _seedArray[_inextp];
            if (value < MINVALUE)
                value += MBIG;

            // Return the value to the buffer.
            _seedArray[_inext] = value;
            return value;
        }

        /// <summary>
        /// Gets a random sample value when there is a large range of values.
        /// </summary>
        /// <returns>Returns a random, larger double value.</returns>
        protected virtual double GetLargeDoubleSample()
        {
            double value = GetIntSample(); // Get a default sample.

            // If the next sample is divisible by 2, it's best to negate the value.
            if (GetIntSample() % 2 == 0)
                value = -value;

            // Extra math is required for getting a valid double sample.
            value += int.MaxValue - 1;
            return value / ((long)int.MaxValue * 2 + 1);
        }
    }
}