// Copyright (c) Craig Williams, SlashParadox

using System;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for dealing with <see cref="Type"/>s.
    /// </summary>
    public static class TypeKit
    {
        /// <summary>
        /// Checks if a <see cref="Type"/> matches or is a subclass of another <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <param name="otherType">The <see cref="Type"/> to check against.</param>
        /// <returns>Returns if <paramref name="type"/> is or is a subclass of <paramref name="otherType"/>.</returns>
        public static bool IsOrIsSubclassOf(this Type type, Type otherType)
        {
            // IsSubclassOf normally fails if the two are the same type.
            return type == otherType || type.IsSubclassOf(otherType);
        }

        /// <summary>
        /// Checks if a generic <see cref="Type"/> is, or is a subclass of, another generic <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <param name="otherType">The <see cref="Type"/> to check against.</param>
        /// <returns>Returns if <paramref name="type"/> is or is a subclass of <paramref name="otherType"/>.</returns>
        public static bool IsGenericSubclassOf(this Type type, Type otherType)
        {
            if (!otherType.IsGenericType)
                return false;

            Type current = type;

            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == otherType.GetGenericTypeDefinition())
                    return true;

                current = current.BaseType;
            }

            return false;
        }

        public static bool ImplementsInterface(this Type type, Type otherType)
        {
            if (!otherType.IsInterface)
                return false;

            Type[] interfaces = type.GetInterfaces();
            foreach (Type i in interfaces)
            {
                if (i == otherType)
                    return true;
            }

            return false;
        }

        public static bool IsSubclassOrImplements(this Type type, Type otherType)
        {
            if (IsOrIsSubclassOf(type, otherType))
                return true;

            if (IsGenericSubclassOf(type, otherType))
                return true;

            return ImplementsInterface(type, otherType);
        }
    }
}