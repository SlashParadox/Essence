// Copyright (c) Craig Williams, SlashParadox

#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SlashParadox.Essence.Kits
{
    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    /// <summary>
    /// A helper class for dealing with <see cref="Type"/>s.
    /// </summary>
    public static partial class TypeKit
    {
        static TypeKit()
        {
            InitializeDeepCopy();
        }

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

        public static Type GetTypeSafe<T>(this T obj)
        {
            return obj == null ? null! : obj.GetType();
        }

        public static bool HasDefaultConstructor(this Type? type)
        {
            return type != null && !type.IsValueType && type.GetConstructor(ReflectionKit.DefaultFlags, null, Type.EmptyTypes, null) != null;
        }
    }
}
