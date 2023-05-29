// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Text;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An <see cref="Exception"/> when a minimum and maximum value are in the wrong order.
    /// </summary>
    /// <typeparam name="T">The type being thrown for.</typeparam>
    public class MinMaxException<T> : Exception where T : IComparable<T>
    {
        /// <summary>
        /// The default constructor for a <see cref="MinMaxException{T}"/>.
        /// </summary>
        public MinMaxException()
            : base("The given minimum and maximum values were not valid. They may need to be swapped.") { }

        /// <summary>
        /// A constructor for a <see cref="MinMaxException{T}"/>, which will create a formatted
        /// string based on the values that were passed.
        /// </summary>
        /// <param name="min">The given minimum value.</param>
        /// <param name="max">The given maximum value.</param>
        /// <param name="equalityAllowed">If true, <paramref name="min"/> could equal <paramref name="max"/>.</param>
        public MinMaxException(T min, T max, bool equalityAllowed)
            : base(BuildMessage(min, max, equalityAllowed)) { }

        /// <summary>
        /// Throws a <see cref="MinMaxException{T}"/> if the given statement is false.
        /// </summary>
        /// <param name="min">The given minimum value.</param>
        /// <param name="max">The given maximum value.</param>
        /// <param name="equalityAllowed">If true, <paramref name="min"/> could equal <paramref name="max"/>.</param>
        /// <exception cref="MinMaxException{T}">Throws an exception if false.</exception>
        public static void CheckAndThrow(T min, T max, bool equalityAllowed)
        {
            int comparison = min.CompareTo(max);

            if (comparison > 0 || (!equalityAllowed && comparison == 0))
                throw new MinMaxException<T>(min, max, equalityAllowed);
        }

        /// <summary>
        /// Builds a message to a <see cref="MinMaxException{T}"/>.
        /// </summary>
        /// <param name="min">The given minimum value.</param>
        /// <param name="max">The given maximum value.</param>
        /// <param name="equalityAllowed">If true, <paramref name="min"/> could equal <paramref name="max"/>.</param>
        /// <returns>Returns a formatted message for a <see cref="MinMaxException{T}"/>.</returns>
        private static string BuildMessage(string min, string max, bool equalityAllowed)
        {
            // Create the string. Only append 'or equal to' if equality was allowed.
            StringBuilder builder = new StringBuilder("Given Minimum [");
            builder.Append(min).Append("] was greater than ");
            builder.Append(!equalityAllowed ? "or equal to " : string.Empty).Append("given Maximum [");
            builder.Append(max).Append("].");

            return builder.ToString(); // Return the finalized string.S
        }

        /// <summary>
        /// Builds a message to a <see cref="MinMaxException{T}"/>.
        /// </summary>
        /// <param name="min">The given minimum value.</param>
        /// <param name="max">The given maximum value.</param>
        /// <param name="equalityAllowed">If true, <paramref name="min"/> could equal <paramref name="max"/>.</param>
        /// <returns>Returns a formatted message for a <see cref="MinMaxException{T}"/>.</returns>
        private static string BuildMessage(T min, T max, bool equalityAllowed)
        {
            return BuildMessage(min.ToString(), max.ToString(), equalityAllowed);
        }
    }
}