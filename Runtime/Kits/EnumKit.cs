// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for dealing with <see cref="Enum"/>s
    /// </summary>
    public static class EnumKit
    {
        /// <summary>
        /// Gets the number of values in an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Enum"/>.</typeparam>
        /// <returns>Returns the value count.</returns>
        public static int GetValueCount<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        /// <summary>
        /// Gets the number of values in an <see cref="Enum"/>.
        /// </summary>
        /// <param name="enumType">The <see cref="Type"/> of the <see cref="Enum"/>.</param>
        /// <returns>Returns the value count.</returns>
        public static int GetValueCount(Type enumType)
        {
            return enumType.IsEnum ? Enum.GetValues(enumType).Length : Literals.InvalidIndex;
        }

        /// <summary>
        /// Gets a collection of the values of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Enum"/>.</typeparam>
        /// <returns>Returns a collection of the values.</returns>
        public static T[] GetValueArray<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Gets a collection of the values of an <see cref="Enum"/> type.
        /// </summary>
        /// <param name="enumType">The <see cref="Type"/> of the <see cref="Enum"/>.</param>
        /// <returns>Returns a collection of the values.</returns>
        public static Array GetValueArray(Type enumType)
        {
            // If the type is an enum, return its value array. Otherwise, return null.
            return enumType.IsEnum ? Enum.GetValues(enumType) : null;
        }

        /// <summary>
        /// Gets a collection of the values of an <see cref="Enum"/> type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Enum"/>.</typeparam>
        /// <returns>Returns a collection of the values.</returns>
        public static List<T> GetValueList<T>() where T : Enum
        {
            T[] array = GetValueArray<T>(); // Get the array of values.

            // Create a list sized to the array length. We avoid using LINQ due to speed concerns.
            int count = array.Length;
            List<T> list = new List<T>(count);

            // Add all enum values to the list.
            for (int i = 0; i < count; i++)
            {
                list.Add(array[i]);
            }

            return list; // Return the final list.
        }
    }
}