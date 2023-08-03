// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base class for classes that represent <see cref="Enum"/>s.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Enum"/> class.</typeparam>
    public abstract class EnumType<T> : IComparable, IComparable<T> where T : EnumType<T>
    {
        /// <summary>The value of the <see cref="EnumType{T}"/>. Checked for equality.</summary>
        public int Value { get; }

        /// <summary>The name of the <see cref="EnumType{T}"/>. Not checked for equality.</summary>
        public string Name { get; }

        /// <summary>A list of registered values for this <see cref="EnumType{T}"/>.</summary>
        private static readonly List<T> EnumValues = new List<T>();

        protected EnumType(int value, string name)
        {
            Value = value;
            Name = name;
            
            EnumValues.Add((T)this);
        }

        /// <summary>
        /// Gets an <see cref="EnumType{T}"/> by the value. Note this only gets the first matching value!
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>Returns the <see cref="EnumType{T}"/>.</returns>
        public static T GetEnumByValue(int value)
        {
            foreach (T en in EnumValues)
            {
                if (en.Value == value)
                    return en;
            }

            return null;
        }

        /// <summary>
        /// Gets an <see cref="EnumType{T}"/> by the name. Note this only gets the first matching value!
        /// </summary>
        /// <param name="name">The name of the value to find.</param>
        /// <returns>Returns the <see cref="EnumType{T}"/>.</returns>
        public static T GetEnumByName(string name)
        {
            foreach (T en in EnumValues)
            {
                if (en.Name == name)
                    return en;
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is T otherValue)
                return otherValue.Value == Value;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (obj is T otherValue)
                return CompareTo(otherValue);

            return -1;
        }

        public int CompareTo(T other)
        {
            if (other == null)
                return -1;

            return Value.CompareTo(other.Value);
        }

        protected bool Equals(T other)
        {
            return other != null && Value == other.Value;
        }
    }
}