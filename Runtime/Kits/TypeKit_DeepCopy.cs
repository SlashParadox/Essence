// Copyright (c) Craig Williams, SlashParadox

// Copyright (c) 2014 Burtsev Alexey
// Copyright (c) 2019 Jean-Paul Mikkers
// Based on https://github.com/jpmikkers/Baksteen.Extensions.DeepCopy, by Mikkers. License included.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SlashParadox.Essence.Kits
{
    public static partial class TypeKit
    {
        /// <summary>A set of primitive types with immutable value types within them. These don't need to be deep-copied, speeding up the copy process.</summary>
        private static readonly HashSet<Type> ImmutableTypes = new HashSet<Type>
                                                               {
                                                                   // Basic Types
                                                                   typeof(nint),
                                                                   typeof(nuint),
                                                                   typeof(bool),
                                                                   typeof(byte),
                                                                   typeof(sbyte),
                                                                   typeof(char),
                                                                   typeof(short),
                                                                   typeof(ushort),
                                                                   typeof(int),
                                                                   typeof(uint),
                                                                   typeof(long),
                                                                   typeof(ulong),
                                                                   typeof(float),
                                                                   typeof(double),
                                                                   typeof(decimal),
                                                                   typeof(string),

                                                                   // Special Numeric Types
                                                                   typeof(System.Numerics.BigInteger),
                                                                   typeof(System.Numerics.Complex),
                                                                   typeof(System.Numerics.Quaternion),
                                                                   typeof(System.Numerics.Vector2),
                                                                   typeof(System.Numerics.Vector3),
                                                                   typeof(System.Numerics.Vector4),
                                                                   typeof(System.Numerics.Plane),
                                                                   typeof(System.Numerics.Matrix3x2),
                                                                   typeof(System.Numerics.Matrix4x4),
                                                                   typeof(Guid),
                                                                   typeof(DateTime),
                                                                   typeof(TimeSpan),
                                                                   typeof(DateTimeOffset),
                                                                   typeof(Range),
                                                                   typeof(Index),

                                                                   // Unique System types
                                                                   typeof(DBNull),
                                                                   typeof(Version),
                                                                   typeof(Uri),
                                                                   typeof(Type),

                                                                   // Types in .NET versions not supported yet by Unity.
                                                                   // typeof(Half),
                                                                   // typeof(DateOnly),
                                                                   // typeof(TimeOnly),

                                                                   // Unity types
                                                                   typeof(UnityEngine.Vector2),
                                                                   typeof(UnityEngine.Vector2Int),
                                                                   typeof(UnityEngine.Vector3),
                                                                   typeof(UnityEngine.Vector3Int),
                                                                   typeof(UnityEngine.Vector4),
                                                                   typeof(UnityEngine.Quaternion),
                                                                   typeof(UnityEngine.Matrix4x4),
                                                                   typeof(UnityEngine.Plane),
                                                                   typeof(UnityEngine.FrustumPlanes),
                                                                   typeof(UnityEngine.Object),
                                                               };

        /// <summary>A copy of the function used to perform a memberwise clone.</summary>
        private static readonly Func<object, object> MemberwiseCloneFunc = (Func<object, object>)typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!
                                                                                                               .CreateDelegate(typeof(Func<object, object>));

        /// <summary>
        /// Performs a deep copy on an object.
        /// </summary>
        /// <param name="original">The original object to copy.</param>
        /// <typeparam name="T">The type of the <paramref name="original"/>.</typeparam>
        /// <returns>Returns the copied object, if doable.</returns>
        public static T? DeepCopy<T>(this T? original)
        {
            Dictionary<Type, FieldInfo[]> fieldCache = new Dictionary<Type, FieldInfo[]>();
            Dictionary<object, object> visitedObjs = new Dictionary<object, object>(new ReferenceEqualityComparer());
            return (T?)DeepCopyInternal(original, fieldCache, visitedObjs);
        }

        /// <summary>
        /// An internal constructor function for initializing <see cref="DeepCopy{T}"/> functionality.
        /// </summary>
        private static void InitializeDeepCopy()
        {
            // Append Nullable versions of the immutable types.
            List<Type> immutableValueTypes = ImmutableTypes.Where(type => type.IsValueType).ToList();
            foreach (Type type in immutableValueTypes)
            {
                ImmutableTypes.Add(typeof(Nullable<>).MakeGenericType(type));
            }
        }

        /// <summary>
        /// An internal, recursive function for deep copying an object.
        /// </summary>
        /// <param name="original">The original object to copy.</param>
        /// <param name="fieldCache">A cache of <see cref="FieldInfo"/>s that need deep copies, for each <see cref="Type"/> the <see cref="original"/> object references.</param>
        /// <param name="visitedObjs">An optional map of objects that have already been cloned.</param>
        /// <returns>Returns the cloned object.</returns>
        private static object? DeepCopyInternal(object? original, Dictionary<Type, FieldInfo[]> fieldCache, Dictionary<object, object>? visitedObjs)
        {
            // Bruh...
            if (original == null)
                return null;

            if (original is Type)
                return original;

            // Immutable objects should just return themselves.
            Type reflectedType = original.GetType();
            if (IsTypeImmutable(reflectedType))
                return original;

            // Some types have special returns.
            if (FindSpecialDeepCopyResult(original, out object? specialResult))
                return specialResult;

            // If there is an object cache, see if the object already exists.
            if (visitedObjs != null && visitedObjs.TryGetValue(original, out object visitedValue))
                return visitedValue;

            // Create the shallow clone and store it as visited.
            object shallowClone = MemberwiseCloneFunc.Invoke(original);
            visitedObjs?.Add(original, shallowClone);

            // Arrays need to be cared for in a special way to copy all objects. Otherwise, deep copy each field that needs it.
            if (reflectedType.IsArray)
            {
                Type? arrayType = reflectedType.GetElementType();

                if (arrayType != null && !IsTypeImmutable(arrayType))
                {
                    // An array of structs does not need to update the visited object cache, as they cannot have references. Otherwise, ref types must be.
                    bool bIsValueType = arrayType.IsValueType;
                    DeepCopyArray((Array)shallowClone, obj => DeepCopyInternal(obj, fieldCache, bIsValueType ? null : visitedObjs));
                }
            }
            else
            {
                FieldInfo[] foundFields = CacheFieldsForDeepCopy(reflectedType, fieldCache);
                foreach (FieldInfo field in foundFields)
                {
                    object fieldValue = field.GetValue(original);
                    object? clonedValue = DeepCopyInternal(fieldValue, fieldCache, field.FieldType.IsValueType ? null : visitedObjs);
                    field.SetValue(shallowClone, clonedValue);
                }
            }

            return shallowClone;
        }

        /// <summary>
        /// Checks if the given <see cref="Type"/> is immutable.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>Returns if the <paramref name="type"/> is immutable and thus can't be deep-copied.</returns>
        private static bool IsTypeImmutable(Type type)
        {
            return type.IsPrimitive || type.IsEnum || ImmutableTypes.Contains(type) || type.GetCustomAttribute<CopyImmutableAttribute>() != null;
        }

        /// <summary>
        /// Checks a given object and type, and sees if there is a special way to return a deep copy of it.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="outResult">The result to return as the deep copy.</param>
        /// <returns>Returns if there was a special value to use for the deep copy.</returns>
        private static bool FindSpecialDeepCopyResult(object obj, out object? outResult)
        {
            outResult = null;

            Type type = obj.GetType();

            if (typeof(Delegate).IsAssignableFrom(type))
                return true;

            if (typeof(System.Xml.Linq.XElement).IsAssignableFrom(type))
            {
                outResult = new System.Xml.Linq.XElement((System.Xml.Linq.XElement)obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deep-copies a given <see cref="Array"/>.
        /// </summary>
        /// <param name="inArray">The <see cref="Array"/> to copy.</param>
        /// <param name="elementCopyFunc">The function used to copy each element of the <paramref name="inArray"/>.</param>
        private static void DeepCopyArray(Array inArray, Func<object?, object?> elementCopyFunc)
        {
            // 1D arrays just need a quick loop. Multi-dimensional arrays need a recursive loop.
            if (inArray.Rank == 1)
            {
                for (int i = 0; i < inArray.Length; ++i)
                {
                    inArray.SetValue(elementCopyFunc(inArray.GetValue(i)), i);
                }
            }
            else
            {
                // Create size arrays for each dimension.
                int[] lengths = new int[inArray.Rank];
                int[] indices = new int[inArray.Rank];

                for (int i = 0; i < inArray.Rank; ++i)
                {
                    lengths[i] = inArray.GetLength(i);
                }

                DeepCopyArray(inArray, elementCopyFunc, 0, lengths, indices);
            }
        }

        /// <summary>
        /// Recursively deep-copies a given <see cref="Array"/>. Mainly for multi-dimensional arrays.
        /// </summary>
        /// <param name="inArray">The <see cref="Array"/> to copy.</param>
        /// <param name="elementCopyFunc">The function used to copy each element of the <paramref name="inArray"/>.</param>
        /// <param name="rank">The current rank (dimension) to copy.</param>
        /// <param name="lengths">An array of all lengths of each dimension of the array.</param>
        /// <param name="indices">An array of the currently iterated index of each dimension of the array.</param>
        private static void DeepCopyArray(Array inArray, Func<object?, object?> elementCopyFunc, int rank, int[] lengths, int[] indices)
        {
            int length = lengths[rank];

            // The last rank handles slightly differently as it does not require a recursion.
            if (rank < lengths.Length - 1)
                for (int i = 0; i < length; i++)
                {
                    indices[rank] = i;
                    DeepCopyArray(inArray, elementCopyFunc, rank + 1, lengths, indices);
                }
            else
                for (int i = 0; i < length; i++)
                {
                    indices[rank] = i;
                    inArray.SetValue(elementCopyFunc(inArray.GetValue(indices)), indices);
                }
        }

        /// <summary>
        /// Caches fields for deep copying a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to cache fields for.</param>
        /// <param name="fieldCache">The cache of fields to their matching <see cref="Type"/>.</param>
        /// <returns>Returns the array of <see cref="FieldInfo"/>s that need deep copies.</returns>
        private static FieldInfo[] CacheFieldsForDeepCopy(Type type, Dictionary<Type, FieldInfo[]> fieldCache)
        {
            if (!fieldCache.TryGetValue(type, out FieldInfo[] result))
            {
                result = EnumerateNonShallowFields(type).ToArray();
                fieldCache[type] = result;
            }

            return result;
        }

        /// <summary>
        /// A LINQ enumerator for finding <see cref="FieldInfo"/>s that need deep copies.
        /// </summary>
        /// <param name="inType">The <see cref="Type"/> to cache fields for.</param>
        /// <returns>Returns the enumerator for finding non-shallow fields.</returns>
        private static IEnumerable<FieldInfo> EnumerateNonShallowFields(Type inType)
        {
            while (inType.BaseType != null)
            {
                foreach (var fieldInfo in inType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (IsTypeImmutable(fieldInfo.FieldType))
                        continue;

                    if (fieldInfo.GetCustomAttribute<CopyImmutableAttribute>() != null)
                        continue;

                    yield return fieldInfo;
                }

                inType = inType.BaseType;
            }
        }
    }
}
