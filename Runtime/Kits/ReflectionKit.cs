// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for using the C# Reflection System.
    /// </summary>
    public static class ReflectionKit
    {
        /// <summary> The default <see cref="BindingFlags"/> that are used for getting members.
        /// These are 'Public, NonPublic, Instance, Static, and FlattenHierarchy'.</summary>
        public static readonly BindingFlags DefaultFlags = BindingFlags.Public
                                                           | BindingFlags.NonPublic | BindingFlags.Instance
                                                           | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        /// <summary> The character typically used to separate pieces of a full member path.</summary>
        public static readonly char DefaultPathSeparator = '.';

        /// <summary> The keyword declaring that a path is accessing an <see cref="IList"/>.</summary>
        private static readonly string ArrayKeyword = "Array";

        /// <summary> The keyword before the index of a <see cref="IList"/> element path.</summary>
        private static readonly string DataKeyword = "data[";

        /// <summary>
        /// Concatenates a group of paths together into one <see cref="string"/>, with a
        /// separator in between all of them.
        /// </summary>
        /// <param name="paths">The separate paths to concatenate.</param>
        /// <returns>Returns the full concatenated <see cref="string"/>.</returns>
        public static string ConcatenatePaths(params string[] paths)
        {
            return ConcatenatePaths(DefaultPathSeparator, paths);
        }

        /// <summary>
        /// Concatenates a group of paths together into one <see cref="string"/>, with a
        /// separator in between all of them.
        /// </summary>
        /// <param name="separator">The character to place between each path.</param>
        /// <param name="paths">The separate paths to concatenate.</param>
        /// <returns>Returns the full concatenated <see cref="string"/>.</returns>
        public static string ConcatenatePaths(char separator, params string[] paths)
        {
            // Return an empty string if there are no paths.
            if (!paths.IsNotEmptyOrNull())
                return string.Empty;

            // Create a string builder, and get the number of pacts.
            StringBuilder fullPath = new StringBuilder();
            int count = paths.Length - 1;

            // Add all paths, except the last one, with a separator.
            for (int i = 0; i < count; i++)
            {
                fullPath.Append(paths[i]).Append(separator);
            }

            // Add the final path without a separator, and return the string.
            fullPath.Append(paths[count]);
            return fullPath.ToString();
        }
        
        /// <summary>
        /// Breaks a given path into multiple <see cref="string"/>s, broken by a separating char.
        /// </summary>
        /// <param name="path">The path to break up.</param>
        /// <returns>Returns the broken up <see cref="path"/>.</returns>
        public static string[] BreakReflectionPath(string path)
        {
            return BreakReflectionPath(DefaultPathSeparator, path);
        }

        /// <summary>
        /// Breaks a given path into multiple <see cref="string"/>s, broken by a separating char.
        /// </summary>
        /// <param name="separator">The character to break paths up by.</param>
        /// <param name="path">The path to break up.</param>
        /// <returns>Returns the broken up <see cref="path"/>.</returns>
        public static string[] BreakReflectionPath(char separator, string path)
        {
            return path?.Split(separator);
        }

        /// <summary>
        /// Creates a path to an <see cref="IList"/> index. This is formatted as
        /// 'Array.data[<paramref name="index"/>]'.
        /// </summary>
        /// <param name="index">The index to place into the path.</param>
        /// <returns>Returns the <see cref="IList"/> path.</returns>
        public static string CreateCollectionIndexPath(int index)
        {
            // Append the keywords, with the index in the middle.
            return new StringBuilder(ArrayKeyword).Append(DefaultPathSeparator).Append(DataKeyword)
                                                  .Append(index).Append(']').ToString();
        }

        /// <summary>
        /// Parses out an index from a data path to an <see cref="IList"/>.
        /// </summary>
        /// <param name="dataPath">The path of the element index.</param>
        /// <returns>Returns the index stored within the <paramref name="dataPath"/>.</returns>
        public static int ParseCollectionIndex(string dataPath)
        {
            Regex regex = new Regex(@"((?!\[)([\d]{1,})(?=\]){1})(?!.*((?!\[)([\d]{1,})(?=\]){1}))(?=]{1}$)");
            return int.Parse(regex.Match(dataPath).Value);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the variable <see cref="Type"/> from.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetFieldType(object obj, params string[] path)
        {
            return GetFieldType(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the variable <see cref="Type"/> from.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetFieldType(object obj, BindingFlags flags, params string[] path)
        {
            FieldInfo info = GetFieldInfo(obj, flags, path);
            return info?.FieldType;
        }
        
        /// <summary>
        /// Gets a given type's <see cref="FieldInfo"/>, given a <paramref name="propertyName"/>. This variant is for static variables.
        /// </summary>
        /// <param name="propertyName">The name of the field being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <typeparam name="T">The type being searched.</typeparam>
        /// <returns>Returns the found <see cref="FieldInfo"/>, if available.</returns>
        public static FieldInfo GetFieldInfo<T>(string propertyName, BindingFlags flags)
        {
            flags |= BindingFlags.Static; // Ensure there's a static flag, otherwise this is pointless.
            Type type = typeof(T);
            return type.GetField(propertyName, flags);
        }

        /// <summary>
        /// Gets a given type's <see cref="FieldInfo"/>, given a <paramref name="propertyName"/>. This variant is for static variables.
        /// </summary>
        /// <param name="type">The type being searched.</param>
        /// <param name="propertyName">The name of the field being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <returns>Returns the found <see cref="FieldInfo"/>, if available.</returns>
        public static FieldInfo GetFieldInfo(Type type, string propertyName, BindingFlags flags)
        {
            flags |= BindingFlags.Static; // Ensure there's a static flag, otherwise this is pointless.

            return type?.GetField(propertyName, flags);
        }

        /// <summary>
        /// Gets an object's <see cref="FieldInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="FieldInfo"/> from.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="FieldInfo"/> at the end of the paths.</returns>
        public static FieldInfo GetFieldInfo(object obj, params string[] path)
        {
            return GetFieldInfo(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="FieldInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="FieldInfo"/> from.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="FieldInfo"/> at the end of the paths.</returns>
        public static FieldInfo GetFieldInfo(object obj, BindingFlags flags, params string[] path)
        {
            object current = obj; // Make a separate reference to the object.
            return GetFieldInfo(ref current, out object _, flags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="FieldInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="current">The current object being looked into. This will end as the final
        /// value in the <see cref="FieldInfo"/>.</param>
        /// <param name="previous">The previous object looked into.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="FieldInfo"/> at the end of the paths.</returns>
        public static FieldInfo GetFieldInfo(ref object current, out object previous, BindingFlags flags, params string[] path)
        {
            FieldInfo field = null; // The FieldInfo of the field.
            previous = current; // Set the previous object to null to start.

            try
            {
                // Go through all the individual paths.
                for (int i = 0; i < path.Length; ++i)
                {
                    // Switch on current's type. This also allows for extra types later if required.
                    switch (current)
                    {
                        // If the current object is an IList.
                        case IList iList:
                        {
                            // Check if the current path matches the ArrayKeyword.
                            if (path[i] == ArrayKeyword)
                            {
                                ++i; // The next path is guaranteed to be the element path.
                                int index = ParseCollectionIndex(path[i]); // Parse out the index.
                                previous = current; // Set the previous object.

                                // If the index is valid, return that object. Else, return null.
                                current = iList.IsValidIndexNG(index) ? iList[index] : null;
                            }

                            break;
                        }
                        default:
                        {
                            // Get the field of the current object's type, using the given flags.
                            field = current?.GetType().GetField(path[i], flags);
                            previous = current; // Update the previous object reference.
                            current = field?.GetValue(current); // Update the current object reference.
                            break;
                        }
                    }
                }
            }
            catch
            {
                // In the event of an error, return null for everything.
                current = null;
                previous = null;
                return null;
            }

            return field; // Return the found FieldInfo.
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the variable <see cref="Type"/> from.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetPropertyType(object obj, params string[] path)
        {
            return GetPropertyType(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the variable <see cref="Type"/> from.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetPropertyType(object obj, BindingFlags flags, params string[] path)
        {
            PropertyInfo info = GetPropertyInfo(obj, flags, path);
            return info?.PropertyType;
        }
        
        /// <summary>
        /// Gets a given type's <see cref="PropertyInfo"/>, given a <paramref name="propertyName"/>. This variant is for static variables.
        /// </summary>
        /// <param name="propertyName">The name of the property being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <typeparam name="T">The type being searched.</typeparam>
        /// <returns>Returns the found <see cref="PropertyInfo"/>, if available.</returns>
        public static PropertyInfo GetPropertyInfo<T>(string propertyName, BindingFlags flags)
        {
            flags |= BindingFlags.Static; // Ensure there's a static flag, otherwise this is pointless.
            Type type = typeof(T);
            return type.GetProperty(propertyName, flags);
        }

        /// <summary>
        /// Gets a given type's <see cref="PropertyInfo"/>, given a <paramref name="propertyName"/>. This variant is for static variables.
        /// </summary>
        /// <param name="type">The type being searched.</param>
        /// <param name="propertyName">The name of the property being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <returns>Returns the found <see cref="PropertyInfo"/>, if available.</returns>
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName, BindingFlags flags)
        {
            flags |= BindingFlags.Static; // Ensure there's a static flag, otherwise this is pointless.
            return type?.GetProperty(propertyName, flags);
        }

        /// <summary>
        /// Gets an object's <see cref="PropertyInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="PropertyInfo"/> at the end of the paths.</returns>
        public static PropertyInfo GetPropertyInfo(object obj, params string[] path)
        {
            return GetPropertyInfo(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="PropertyInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="PropertyInfo"/> from.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="PropertyInfo"/> at the end of the paths.</returns>
        public static PropertyInfo GetPropertyInfo(object obj, BindingFlags flags, params string[] path)
        {
            object current = obj; // Make a separate reference to the object.
            return GetPropertyInfo(ref current, out object _, flags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="PropertyInfo"/>, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="current">The current object being looked into. This will end as the final
        /// value in the <see cref="PropertyInfo"/>.</param>
        /// <param name="previous">The previous object looked into.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="PropertyInfo"/> at the end of the paths.</returns>
        public static PropertyInfo GetPropertyInfo(ref object current, out object previous, BindingFlags flags, params string[] path)
        {
            PropertyInfo property = null; // The PropertyInfo of the field.
            previous = current; // Set the previous object to null to start.
            try
            {
                // Go through all the individual paths.
                for (int i = 0; i < path.Length; ++i)
                {
                    // Switch on current's type. This also allows for extra types later if required.
                    switch (current)
                    {
                        // If the current object is an IList.
                        case IList iList:
                        {
                            // Check if the current path matches the ArrayKeyword.
                            if (path[i] == ArrayKeyword)
                            {
                                i++; // The next path is guaranteed to be the element path.
                                int index = ParseCollectionIndex(path[i]); // Parse out the index.
                                previous = current; // Set the previous object.

                                // If the index is valid, return that object. Else, return null.
                                current = iList.IsValidIndexNG(index) ? iList[index] : null;
                            }

                            break; 
                        }
                        default:
                        {
                            // Get the field of the current object's type, using the given flags.
                            property = current?.GetType().GetProperty(path[i], flags);
                            previous = current; // Update the previous object reference.
                            current = property?.GetValue(current); // Update the current object reference.
                            break; 
                        } 
                    }
                }
            }
            catch
            {
                // In the event of an error, return null for everything.
                current = null;
                previous = null;
                return null;
            }

            return property; // Return the found FieldInfo.
        }
        
        /// <summary>
        /// Gets an object's <see cref="MemberInfo"/>, given a <paramref name="path"/>. This is specifically for variables.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="MemberInfo"/> from.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="MemberInfo"/> at the end of the paths.</returns>
        public static MemberInfo GetVariableInfo(object obj, params string[] path)
        {
            return GetVariableInfo(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="MemberInfo"/>, given a <paramref name="path"/>. This is specifically for variables.
        /// </summary>
        /// <param name="obj">The starting object to get the <see cref="MemberInfo"/> from.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="MemberInfo"/> at the end of the paths.</returns>
        public static MemberInfo GetVariableInfo(object obj, BindingFlags flags, params string[] path)
        {
            object current = obj; // Make a separate reference to the object.
            return GetVariableInfo(ref current, out object _, flags, path);
        }

        /// <summary>
        /// Gets an object's <see cref="MemberInfo"/>, given a <paramref name="path"/>. This is specifically for variables.
        /// </summary>
        /// <param name="current">The current object being looked into. This will end as the final
        /// value in the <see cref="PropertyInfo"/>.</param>
        /// <param name="previous">The previous object looked into.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="path">The array of path pieces to the wanted value.</param>
        /// <returns>Returns the <see cref="MemberInfo"/> at the end of the paths.</returns>
        public static MemberInfo GetVariableInfo(ref object current, out object previous, BindingFlags flags, params string[] path)
        {
            object startObj = current;
            
            FieldInfo fieldInfo = GetFieldInfo(ref current, out previous, flags, path);
            if (fieldInfo != null)
                return fieldInfo;

            current = startObj;
            return GetPropertyInfo(ref current, out previous, flags, path);
        }

        /// <summary>
        /// Gets a given type's <see cref="MethodInfo"/>, given a <paramref name="methodName"/>.
        /// </summary>
        /// <param name="methodName">The name of the method being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <typeparam name="T">The type being searched.</typeparam>
        /// <returns>Returns the found <see cref="MethodInfo"/>, if available.</returns>
        public static MethodInfo GetMethodInfo<T>(string methodName, BindingFlags flags)
        {
            Type type = typeof(T);
            return type.GetMethod(methodName, flags);
        }

        /// <summary>
        /// Gets a given type's <see cref="MethodInfo"/>, given a <paramref name="methodName"/>.
        /// </summary>
        /// <param name="type">The type being searched.</param>
        /// <param name="methodName">The name of the method being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <returns>Returns the found <see cref="MethodInfo"/>, if available.</returns>
        public static MethodInfo GetMethodInfo(Type type, string methodName, BindingFlags flags)
        {
            return type?.GetMethod(methodName, flags);
        }

        /// <summary>
        /// Gets a value of a field of a given type. This variant is for static variables.
        /// </summary>
        /// <param name="fieldName">The name of the field being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="value">The found value.</param>
        /// <typeparam name="TType">The type being searched.</typeparam>
        /// <typeparam name="TValue">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetFieldValue<TType, TValue>(string fieldName, BindingFlags flags, out TValue value)
        {
            return GetFieldValue(typeof(TType), fieldName, flags, out value);
        }

        /// <summary>
        /// Gets a value of a field of a given type. This variant is for static variables.
        /// </summary>
        /// <param name="type">The type being searched.</param>
        /// <param name="fieldName">The name of the field being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="value">The found value.</param>
        /// <typeparam name="TValue">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetFieldValue<TValue>(Type type, string fieldName, BindingFlags flags, out TValue value)
        {
            value = default;
            FieldInfo fieldInfo = GetFieldInfo(type, fieldName, flags);

            if (fieldInfo == null || fieldInfo.GetValue(null) is not TValue foundValue)
                return false;
            
            value = foundValue;
            return true;
        }

        /// <summary>
        /// Gets a value from a Field after a series of paths.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetFieldValue<T>(object obj, out T value, params string[] path)
        {
            return GetFieldValue(obj, DefaultFlags, out value, path);
        }

        /// <summary>
        /// Gets a value from a Field after a series of paths.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetFieldValue<T>(object obj, BindingFlags flags, out T value, params string[] path)
        {
            object current = obj; // The current object. This is a constantly changing reference.
            value = default;

            // Get the FieldInfo at the end of the series of paths.
            FieldInfo field = GetFieldInfo(ref current, out _, flags, path);

            // In the case of an error, return the default value.
            if (field == null || current is not T foundValue) 
                return false;
            
            // If the value is not null, and is assignable from T, return the value cast to T.
            value = foundValue;
            return true;
        }

        /// <summary>
        /// Sets a value to a Field after a series of paths. The <see cref="DefaultFlags"/> are used.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was properly set or not.</returns>
        public static bool SetFieldValue<T>(object obj, T value, params string[] path)
        {
            return SetFieldValue(obj, value, DefaultFlags, path);
        }

        /// <summary>
        /// Sets a value to a Field after a series of paths.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was properly set or not.</returns>
        public static bool SetFieldValue<T>(object obj, T value, BindingFlags flags,
                                            params string[] path)
        {
            object current = obj; // The current object. This is a constantly changing reference.

            // Get the FieldInfo at the end of the series of paths. We will need the previous object.
            FieldInfo field = GetFieldInfo(ref current, out object previous, flags, path);

            // Make sure the field is not null.
            if (field == null)
                return false;

            // Switch on previous type. This also allows for extra types later if required.
            switch (previous)
            {
                // If the previous object is an IList, and the last path points to an array element.
                case IList iList when path.LastElement().Contains(DataKeyword):
                {
                    int index = ParseCollectionIndex(path.LastElement()); // Parse the index out.

                    // Check that the collection is not null, and the requested index is valid.
                    if (!iList.IsValidIndexNG(index))
                        return false; // The value was not set.

                    iList[index] = value; // Set the value at the requested index.
                    return true; // The value was successfully set.
                }
                case IList<T> iList when path.LastElement().Contains(DataKeyword):
                {
                    int index = ParseCollectionIndex(path.LastElement()); // Parse the index out.

                    // Check that the collection is not null, and the requested index is valid.
                    if (!iList.IsValidIndex(index))
                        return false;

                    iList[index] = value; // Set the value at the requested index.
                    return true; // The value was successfully set.
                }
                default:
                {
                    field.SetValue(previous, value);
                    return true; // Return if the value was correctly set.
                }
            }
        }
        
        /// <summary>
        /// Gets a value of a property of a given type. This variant is for static variables.
        /// </summary>
        /// <param name="propertyName">The name of the property being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="value">The found value.</param>
        /// <typeparam name="TType">The type being searched.</typeparam>
        /// <typeparam name="TValue">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetPropertyValue<TType, TValue>(string propertyName, BindingFlags flags, out TValue value)
        {
            return GetPropertyValue(typeof(TType), propertyName, flags, out value);
        }

        /// <summary>
        /// Gets a value of a property of a given type. This variant is for static variables.
        /// </summary>
        /// <param name="type">The type being searched.</param>
        /// <param name="propertyName">The name of the property being searched for.</param>
        /// <param name="flags">The <see cref="BindingFlags"/> to determine what is accessible.</param>
        /// <param name="value">The found value.</param>
        /// <typeparam name="TValue">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetPropertyValue<TValue>(Type type, string propertyName, BindingFlags flags, out TValue value)
        {
            value = default;
            PropertyInfo propertyInfo = GetPropertyInfo(type, propertyName, flags);

            if (propertyInfo == null || propertyInfo.GetValue(null, null) is not TValue foundValue)
                return false;
            
            value = foundValue;
            return true;
        }

        /// <summary>
        /// Gets a value from a Property after a series of paths. The <see cref="DefaultFlags"/> are used.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetPropertyValue<T>(object obj, out T value, params string[] path)
        {
            return GetPropertyValue(obj, DefaultFlags, out value, path);
        }

        /// <summary>
        /// A function to get a value from a PropertyField after a series of paths.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetPropertyValue<T>(object obj, BindingFlags flags, out T value, params string[] path)
        {
            object current = obj; // The current object. This is a constantly changing reference.
            value = default;

            // Get the PropertyInfo at the end of the series of paths.
            PropertyInfo property = GetPropertyInfo(ref current, out object _, flags, path);

            // In the case of an error, return the default value.
            if (property == null || current is not T foundValue) 
                return false;
            
            // If the value is not null, and is assignable from T, return the value cast to T.
            value = foundValue;
            return true;
        }

        /// <summary>
        /// A function to set a value from a Property after a series of paths.
        /// The <see cref="DefaultFlags"/> are used.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was properly set or not.</returns>
        public static bool SetPropertyValue<T>(object obj, T value, params string[] path)
        {
            return SetPropertyValue(obj, value, DefaultFlags, path);
        }

        /// <summary>
        /// A function to set a value from a Property after a series of paths.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was properly set or not.</returns>
        public static bool SetPropertyValue<T>(object obj, T value, BindingFlags flags, params string[] path)
        {
            object current = obj; // The current object. This is a constantly changing reference.

            // Get the FieldInfo at the end of the series of paths. We will need the previous object.
            PropertyInfo property = GetPropertyInfo(ref current, out object previous, flags, path);

            // Make sure the field is not null.
            if (property == null)
                return false;

            // Switch on previous type. This also allows for extra types later if required.
            switch (previous)
            {
                // If the previous object is an IList, and the last path points to an array element.
                case IList iList when path.LastElement().Contains(DataKeyword):
                {
                    int index = ParseCollectionIndex(path.LastElement()); // Parse the index out.

                    // Check that the collection is not null, and the requested index is valid.
                    if (!iList.IsValidIndexNG(index))
                        return false;

                    iList[index] = value; // Set the value at the requested index.
                    return true; // The value was successfully set.
                }
                case IList<T> iList when path.LastElement().Contains(DataKeyword):
                {
                    int index = ParseCollectionIndex(path.LastElement()); // Parse the index out.

                    // Check that the collection is not null, and the requested index is valid.
                    if (!iList.IsValidIndex(index))
                        return false;

                    iList[index] = value; // Set the value at the requested index.
                    return true; // The value was successfully set.
                }
                default:
                {
                    try
                    {
                        property.SetValue(previous, value);
                        return property.GetValue(previous) == (object)value; // Return if the value was set.
                    }
                    catch
                    {
                        return false; // In the case of an error, return false.
                    }
                }
            }
        }
        
         /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetVariableType(object obj, params string[] path)
        {
            return GetVariableType(obj, DefaultFlags, path);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of an object's variable value, given a <paramref name="path"/>.
        /// </summary>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns the <see cref="Type"/> of the variable at the end of the paths.</returns>
        public static Type GetVariableType(object obj, BindingFlags flags, params string[] path)
        {
            // First, attempt to find the variable as a field.
            FieldInfo fInfo = GetFieldInfo(obj, flags, path);
            if (fInfo != null)
                return fInfo.FieldType;

            // Second, attempt to find the variable as a property.
            PropertyInfo pInfo = GetPropertyInfo(obj, flags, path);
            return pInfo?.PropertyType;
        }

        /// <summary>
        /// Gets a value from a member variable, checking multiple variable types.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetVariableValue<T>(object obj, out T value, params string[] path)
        {
            return GetVariableValue(obj, DefaultFlags, out value, path);
        }

        /// <summary>
        /// Gets a value from a member variable, checking multiple variable types.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="value">The found value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the <paramref name="value"/> was successfully found.</returns>
        public static bool GetVariableValue<T>(object obj, BindingFlags flags, out T value, params string[] path)
        {
            // First, attempt to find the variable as a field.
            if (GetFieldValue(obj, flags, out value, path))
                return true;

            // Second, attempt to find the variable as a property.
            return GetPropertyValue(obj, flags, out value, path);
        }

        /// <summary>
        /// Sets a value to a member variable, checking multiple variable types.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was set or not.</returns>
        public static bool SetVariableValue<T>(object obj, T value, params string[] path)
        {
            return SetVariableValue(obj, value, DefaultFlags, path);
        }

        /// <summary>
        /// Sets a value to a member variable, checking multiple variable types.
        /// </summary>
        /// <typeparam name="T">The type of the variable. If you don't know, use <see cref="object"/>.</typeparam>
        /// <param name="obj">The starting object to get the value off from.</param>
        /// <param name="value">The value to set into the object.</param>
        /// <param name="flags">The flags used to get access to the value.</param>
        /// <param name="path">The series of paths to get to the value.</param>
        /// <returns>Returns if the value was properly set or not.</returns>
        public static bool SetVariableValue<T>(object obj, T value, BindingFlags flags, params string[] path)
        {
            return !SetFieldValue(obj, value, flags, path) || SetPropertyValue(obj, value, flags, path);
        }

        /// <summary>
        /// Finds a managed type reference's true type.
        /// </summary>
        /// <param name="managedReferenceName">The path of the reference. This typically formats as
        /// "{Assembly Name} {Type Full Name}".</param>
        /// <returns>Returns the found type, if at all.</returns>
        public static Type FindManagedReferenceType(string managedReferenceName)
        {
            if (string.IsNullOrEmpty(managedReferenceName))
                return null;

            string[] reference = managedReferenceName.Split(' ');
            if (reference.IsEmptyOrNull() || reference.Length != 2)
                return null;

            return FindManagedReferenceType(reference[0], reference[1]);
        }

        /// <summary>
        /// Finds a managed type reference's true type.
        /// </summary>
        /// <param name="assemblyName">The name of the <see cref="Assembly"/>.</param>
        /// <param name="typeName">The full name of the <see cref="Type"/>.</param>
        /// <returns>Returns the found type, if at all.</returns>
        public static Type FindManagedReferenceType(string assemblyName, string typeName)
        {
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(typeName))
                return null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly == null)
                    continue;

                // Assembly full names only have the actual name part in the first part, followed with metadata.
                if (assembly.FullName.StartsWith($"{assemblyName},"))
                    return assembly.GetType(typeName);
            }

            return null;
        }
    }
}