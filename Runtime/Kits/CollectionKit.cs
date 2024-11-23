// Copyright (c) SlashParadox

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for collection types.
    /// </summary>
    public static class CollectionKit
    {
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is empty.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns if <paramref name="iCol"/> is empty.</returns>
        public static bool IsEmpty<T>(this ICollection<T> iCol)
        {
            return iCol.Count <= 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is empty.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <returns>Returns if <paramref name="iCol"/> is empty.</returns>
        public static bool IsEmptyNG(this ICollection iCol)
        {
            return iCol.Count <= 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is not empty.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns if <paramref name="iCol"/> is not empty.</returns>
        public static bool IsNotEmpty<T>(this ICollection<T> iCol)
        {
            return iCol.Count > 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is not empty.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <returns>Returns if <paramref name="iCol"/> is not empty.</returns>
        public static bool IsNotEmptyNG(this ICollection iCol)
        {
            return iCol.Count > 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is empty or null.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns if <paramref name="iCol"/> is empty.</returns>
        public static bool IsEmptyOrNull<T>(this ICollection<T> iCol)
        {
            return iCol == null || iCol.Count <= 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is empty or null.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <returns>Returns if <paramref name="iCol"/> is empty.</returns>
        public static bool IsEmptyOrNullNG(this ICollection iCol)
        {
            return iCol == null || iCol.Count <= 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is not empty or null.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns if <paramref name="iCol"/> is not empty.</returns>
        public static bool IsNotEmptyOrNull<T>(this ICollection<T> iCol)
        {
            return iCol != null && iCol.Count > 0;
        }
        
        /// <summary>
        /// Checks if the given <see cref="ICollection"/> is not empty or null.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <returns>Returns if <paramref name="iCol"/> is not empty.</returns>
        public static bool IsNotEmptyOrNullNG(this ICollection iCol)
        {
            return iCol != null && iCol.Count > 0;
        }

        /// <summary>
        /// Checks if the given <paramref name="index"/> is valid.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <param name="index">The index to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns if <paramref name="index"/> is valid.</returns>
        public static bool IsValidIndex<T>(this ICollection<T> iCol, int index)
        {
            return index >= 0 && index < iCol.Count;
        }
        
        /// <summary>
        /// Checks if the given <paramref name="index"/> is valid.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <param name="index">The index to check.</param>
        /// <returns>Returns if <paramref name="index"/> is valid.</returns>
        public static bool IsValidIndexNG(this ICollection iCol, int index)
        {
            return index >= 0 && index < iCol.Count;
        }

        /// <summary>
        /// Gets the last index of the given <see cref="ICollection"/>.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <typeparam name="T">The type stored in <paramref name="iCol"/>.</typeparam>
        /// <returns>Returns <paramref name="iCol"/>'s last index.</returns>
        public static int LastIndex<T>(this ICollection<T> iCol)
        {
            return iCol.Count - 1;
        }
        
        /// <summary>
        /// Gets the last index of the given <see cref="ICollection"/>.
        /// </summary>
        /// <param name="iCol">The collection to check.</param>
        /// <returns>Returns <paramref name="iCol"/>'s last index.</returns>
        public static int LastIndexNG(this ICollection iCol)
        {
            return iCol.Count - 1;
        }

        /// <summary>
        /// Gets the last element of a given <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns <paramref name="iList"/>'s last element.</returns>
        public static T LastElement<T>(this IList<T> iList)
        {
            return iList[iList.LastIndex()];
        }
        
        /// <summary>
        /// Gets the last element of a given <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <returns>Returns <paramref name="iList"/>'s last element.</returns>
        public static object LastElementNG(this IList iList)
        {
            return iList[iList.LastIndexNG()];
        }
        
        /// <summary>
        /// Swaps the values at two indices of an <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="indexA">The first index.</param>
        /// <param name="indexB">The second index.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void SwapValues<T>(this IList<T> iList, int indexA, int indexB)
        {
            (iList[indexA], iList[indexB]) = (iList[indexB], iList[indexA]);
        }
        
        /// <summary>
        /// Swaps the values at two indices of an <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="indexA">The first index.</param>
        /// <param name="indexB">The second index.</param>
        public static void SwapValuesNG(this IList iList, int indexA, int indexB)
        {
            (iList[indexA], iList[indexB]) = (iList[indexB], iList[indexA]);
        }

        /// <summary>
        /// Swaps the indices of two values of an <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="itemA">The first item.</param>
        /// <param name="itemB">The second item.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the swap was successful.</returns>
        public static bool SwapIndices<T>(this IList<T> iList, T itemA, T itemB)
        {
            int indexA;
            int indexB;
            
            // Special care is required for arrays, which do not implement 'IndexOf' directly.
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (iList is Array array)
            {
                indexA = Array.IndexOf(array, itemA);
                indexB = Array.IndexOf(array, itemB);
            }
            else
            {
                indexA = iList.IndexOf(itemA);
                indexB = iList.IndexOf(itemB);
            }

            if (!iList.IsValidIndex(indexA) || !iList.IsValidIndex(indexB))
                return false;
            
            iList.SwapValues(indexA, indexB);
            return true;
        }
        
        /// <summary>
        /// Swaps the indices of two values of an <see cref="IList"/>.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="itemA">The first item.</param>
        /// <param name="itemB">The second item.</param>
        /// <returns>Returns if the swap was successful.</returns>
        public static bool SwapIndicesNG(this IList iList, object itemA, object itemB)
        {
            int indexA;
            int indexB;

            // Special care is required for arrays, which do not implement 'IndexOf' directly.
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (iList is Array array)
            {
                indexA = Array.IndexOf(array, itemA);
                indexB = Array.IndexOf(array, itemB);
            }
            else
            {
                indexA = iList.IndexOf(itemA);
                indexB = iList.IndexOf(itemB);
            }

            if (!iList.IsValidIndexNG(indexA) || !iList.IsValidIndexNG(indexB))
                return false;
            
            iList.SwapValuesNG(indexA, indexB);
            return true;
        }

        /// <summary>
        /// Adds an item to an <see cref="IList"/> if it is not already added.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="item">The item to add.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns the index of the <paramref name="item"/>.</returns>
        public static int AddUnique<T>(this IList<T> iList, T item)
        {
            int index = iList.IndexOf(item);
            if (index >= 0) 
                return index;
            
            iList.Add(item);
            return iList.LastIndex();

        }
        
        /// <summary>
        /// Adds an item to an <see cref="IList"/> if it is not already added.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>Returns the index of the <paramref name="item"/>.</returns>
        public static int AddUniqueNG(this IList iList, object item)
        {
            int index = iList.IndexOf(item);
            if (index >= 0) 
                return index;
            
            iList.Add(item);
            return iList.LastIndexNG();

        }

        /// <summary>
        /// Removes a single item from an <see cref="IList"/>, swapping places with the last element for a
        /// faster removal.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="item">The item to remove.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns the number of items removed.</returns>
        public static int RemoveSingleSwap<T>(this IList<T> iList, T item)
        {
            int index = iList.IndexOf(item);
            if (index < 0)
                return 0;

            // Only swap values if the item is not already at the last index.
            int lastIndex = iList.LastIndex();
            if (index != lastIndex)
                iList.SwapValues(index, lastIndex);
            
            iList.RemoveAt(lastIndex);
            return 1;
        }

        /// <summary>
        /// Removes a single item from an <see cref="IList"/>, swapping places with the last element for a
        /// faster removal.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns>Returns the number of items removed.</returns>
        public static int RemoveSingleSwapNG(this IList iList, object item)
        {
            int index = iList.IndexOf(item);
            if (index < 0)
                return 0;

            // Only swap values if the item is not already at the last index.
            int lastIndex = iList.LastIndexNG();
            if (index != lastIndex)
                iList.SwapValuesNG(index, lastIndex);
            
            iList.RemoveAt(lastIndex);
            return 1;
        }
        
        /// <summary>
        /// Removes a single item from an <see cref="IList"/>, swapping places with the last element for a
        /// faster removal.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="index">The index of the item to remove.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns the number of items removed.</returns>
        public static int RemoveAtSwap<T>(this IList<T> iList, int index)
        {
            if (!iList.IsValidIndex(index))
                return 0;

            // Only swap values if the item is not already at the last index.
            int lastIndex = iList.LastIndex();
            if (index != lastIndex)
                iList.SwapValues(index, lastIndex);
            
            iList.RemoveAt(lastIndex);
            return 1;
        }
        
        /// <summary>
        /// Removes a single item from an <see cref="IList"/>, swapping places with the last element for a
        /// faster removal.
        /// </summary>
        /// <param name="iList">The list to check.</param>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>Returns the number of items removed.</returns>
        public static int RemoveAtSwapNG(this IList iList, int index)
        {
            if (!iList.IsValidIndexNG(index))
                return 0;

            // Only swap values if the item is not already at the last index.
            int lastIndex = iList.LastIndexNG();
            if (index != lastIndex)
                iList.SwapValuesNG(index, lastIndex);
            
            iList.RemoveAt(lastIndex);
            return 1;
        }

        /// <summary>
        /// Shuffles a given <see cref="IList"/> by swapping every index with at least one other index.
        /// </summary>
        /// <param name="iList">The list to shuffle.</param>
        /// <param name="random">The generator to use.</param>
        /// <param name="startIndex">The inclusive first index.</param>
        /// <param name="lastIndex">The exclusive last index.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/></typeparam>
        public static void Shuffle<T>(this IList<T> iList, Random random, int startIndex, int lastIndex)
        {
            // For every index, swap at least once with another random index.
            for (int i = startIndex; i < lastIndex; i++)
            {
                int randomIndex = random.Next(i, lastIndex);
                iList.SwapValues(i, randomIndex);
            }
        }
        
        /// <summary>
        /// Shuffles a given <see cref="IList"/> by swapping every index with at least one other index.
        /// </summary>
        /// <param name="iList">The list to shuffle.</param>
        /// <param name="random">The generator to use.</param>
        /// <param name="startIndex">The inclusive first index.</param>
        /// <param name="lastIndex">The exclusive last index.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/></typeparam>
        public static void Shuffle<T>(this IList<T> iList, RandomNumberGenerator random, int startIndex, int lastIndex)
        {
            // For every index, swap at least once with another random index.
            for (int i = startIndex; i < lastIndex; i++)
            {
                int randomIndex = random.GetRandomIntIE(i, lastIndex);
                iList.SwapValues(i, randomIndex);
            }
        }
        
        /// <summary>
        /// Attempts to get a value from a <see cref="IDictionary{TKey,TValue}"/>, if it exists. If the key is not valid, a new entry is created with
        /// the default value.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> to check.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The found or created value.</param>
        /// <typeparam name="TKey">The type of the <see cref="key"/>.</typeparam>
        /// <typeparam name="TValue">The type of the <see cref="value"/>.</typeparam>
        public static void GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
                return;

            value = default;
            dictionary.Add(key, value);
        }

        /// <summary>
        /// Attempts to get a value from a <see cref="IDictionary{TKey,TValue}"/>, if it exists. If the key is not valid, or is null, a new entry is
        /// created with the default constructor.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> to check.</param>
        /// <param name="key">The key of the value.</param>
        /// <param name="value">The found or created value.</param>
        /// <typeparam name="TKey">The type of the <see cref="key"/>.</typeparam>
        /// <typeparam name="TValue">The type of the <see cref="value"/>.</typeparam>
        public static void GetOrInitializeValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value) where TValue : new()
        {
            bool isContained = dictionary.TryGetValue(key, out value);

            if (value != null)
                return;

            value = new TValue();

            if (isContained)
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
