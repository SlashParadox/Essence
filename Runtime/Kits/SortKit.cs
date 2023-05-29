// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections;
using System.Collections.Generic;

namespace SlashParadox.Essence.Kits
{
    /// <summary>
    /// A helper class for sorting collections.
    /// </summary>
    public static class SortKit
    {
        /// <summary>The array run size for a Tim Sort.</summary>
        private static readonly int TimSortRun = 32;
        
        /// <summary>
        /// Checks if a collection is sorted, via a Linear Method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the <paramref name="iList"/> is sorted.</returns>
        public static bool IsSortedLinear<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            return IsSortedLinear(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Checks if a collection is sorted, via a Linear Method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <param name="lastIndex">The exclusive index to check up to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the <paramref name="iList"/> is sorted.</returns>
        public static bool IsSortedLinear<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, true);
            
            for (int i = startIndex + 1; i < lastIndex; ++i)
            {
                // If the comparison is ever out of order (last > current), return false.
                if (compare(iList[i - 1], iList[i]) > 0)
                    return false;
            }

            return true; // The collection is sorted.
        }

        /// <summary>
        /// Checks if a collection is sorted, via a Cocktail Method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the <paramref name="iList"/> is sorted.</returns>
        public static bool IsSortedCocktail<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            return IsSortedCocktail(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Checks if a collection is sorted, via a Cocktail Method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <param name="lastIndex">The exclusive index to check up to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the <paramref name="iList"/> is sorted.</returns>
        public static bool IsSortedCocktail<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, true);

            // The number of moves to make. This is half of the array's size.
            int moves = (int)Math.Ceiling((lastIndex - startIndex) / 2.0f);

            --lastIndex; // Decrement the last index to fit the number of moves in the loop.

            for (int i = 0; i < moves; ++i)
            {
                // If the comparison on either side fails, return false.
                if (compare(iList[startIndex], iList[startIndex + 1]) > 0)
                    return false;
                if (compare(iList[lastIndex - 1], iList[lastIndex]) > 0)
                    return false;

                // Move the dual indexes closer to each other.
                ++startIndex;
                --lastIndex;
            }

            return true; // The collection is sorted.
        }

        public static bool IsSortedDivided<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            return IsSortedDivided(iList, compare, 0, iList.Count);
        }

        /// <summary>
        /// Checks if a collection is sorted, via a Divide-And-Conquer Method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <param name="lastIndex">The exclusive index to check up to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the <paramref name="iList"/> is sorted.</returns>
        public static bool IsSortedDivided<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, true);

            // Get two middle index points, as the collection will be divided in two.
            int startMidIndex = lastIndex / 2;
            int endMidIndex = startMidIndex;

            // If the array is of even size, we need to compare the two middle indexes first.
            if (lastIndex % 2 == 0)
            {
                startMidIndex -= 1; // Decrement the low end to properly split.

                // Compare with the middle index.
                if (compare(iList[startMidIndex], iList[endMidIndex]) > 0)
                    return false;
            }

            lastIndex--; // Decrement the end index by 1, as it is an exclusive input.

            // Return if both sides are sorted.
            return IsSortedDividedPartition(iList, compare, startIndex, startMidIndex) &&
                   IsSortedDividedPartition(iList, compare, endMidIndex, lastIndex);
        }

        /// <summary>
        /// Sorts a given collection via the Improved Bogo Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="random">An optional <see cref="Random"/> generator. <see cref="RandomKit.DefaultGenerator"/> is used if null.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <remarks>This algorithm is for educational purposes only.</remarks>
        public static void BogoSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0, Random random = null)
        {
            BogoSort(iList, compare, startIndex, iList.Count, random);
        }

        /// <summary>
        /// Sorts a given collection via the Improved Bogo Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <param name="random">An optional <see cref="Random"/> generator. <see cref="RandomKit.DefaultGenerator"/> is used if null.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <remarks>This algorithm is for educational purposes only.</remarks>
        public static void BogoSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex, Random random = null)
        {
            Random usedGenerator = random ?? RandomKit.DefaultGenerator;

            // Continue through the collection until all items are sorted.
            while (startIndex < lastIndex)
            {
                T temp = iList[startIndex]; // Get a temporary value to check order against.
                int position = startIndex; // Create a value for the current position in the iteration.

                // Check each position, up until something is found to be out of order.
                for (; position < lastIndex; ++position)
                {
                    if (compare(temp, iList[position]) > 0)
                        break;
                }

                // If the comparison so far is fine, increment where we start. Otherwise, shuffle.
                if (position == lastIndex)
                    ++startIndex;
                else
                    iList.Shuffle(usedGenerator, startIndex, lastIndex);
            }
        }

        /// <summary>
        /// Sorts a given collection via the True Bogo Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="random">An optional <see cref="Random"/> generator. <see cref="RandomKit.DefaultGenerator"/> is used if null.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <remarks>This algorithm is for educational purposes only.</remarks>
        public static void TrueBogoSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0, Random random = null)
        {
            TrueBogoSort(iList, compare, startIndex, iList.Count, random);
        }

        /// <summary>
        /// Sorts a given collection via the True Bogo Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <param name="random">An optional <see cref="Random"/> generator. <see cref="RandomKit.DefaultGenerator"/> is used if null.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <remarks>This algorithm is for educational purposes only.</remarks>
        public static void TrueBogoSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex, Random random = null)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, true);

            Random usedGenerator = random ?? RandomKit.DefaultGenerator;

            // Continue through the collection until all items are sorted.
            while (!iList.IsSortedLinear(compare, startIndex, lastIndex))
            {
                iList.Shuffle(usedGenerator, startIndex, lastIndex);
            }
        }

        /// <summary>
        /// Sorts a given collection via the Bubble Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void BubbleSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            BubbleSort(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Sorts a given collection via the Bubble Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void BubbleSort<T>(this IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            for (int i = startIndex; i < lastIndex - 1; ++i)
            {
                bool hasSwapped = false; // A toggle for if any swaps were done.
                T tempA = iList[startIndex];
                int innerLastIndex = lastIndex - (i - startIndex);

                // Swap for the remainder of the array section.
                for (int j = startIndex + 1; j < innerLastIndex; ++j)
                {
                    T tempB = iList[j];

                    // If the values are not sorted, swap adjacent values.
                    if (compare(tempA, tempB) > 0)
                    {
                        iList.SwapValues(j, j - 1);
                        hasSwapped = true;
                    }
                    else
                        tempA = tempB; // Otherwise, replace the first comparison to make a new pivot.
                }

                // If nothing was swapped, the array is in order, and the loop can be stopped.
                if (!hasSwapped)
                    break;
            }
        }

        /// <summary>
        /// Sorts a given collection via the Heap Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void HeapSort<T>(IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            HeapSort(iList, compare, startIndex, iList.Count);
        }
        
        /// <summary>
        /// Sorts a given collection via the Heap Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void HeapSort<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, true);

            int lastSortedIndex = lastIndex - startIndex; // Get the last valid index to sort.
            int heapStart = startIndex + (lastSortedIndex - 2) / 2; // Get the starting heap index.
      
            // Perform the loop backwards to begin creating heaps.
            for (int i = heapStart; i >= startIndex; --i)
                HeapSortBuildHeap(iList, compare, startIndex, lastIndex, i);

            // Swap values within the array section.
            for (int i = 1; i < lastSortedIndex; ++i)
            {
                int highPartition = lastIndex - i; // The highest index to go to in the heap.
                iList.SwapValues(startIndex, highPartition); // Swap the values in the partition.
                HeapSortBuildHeap(iList, compare, startIndex, highPartition, startIndex); // Update the heap.
            }
        }

        /// <summary>
        /// Sorts a given collection via the Selection Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void SelectionSort<T>(IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            SelectionSort(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Sorts a given collection via the Selection Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void SelectionSort<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            // Loop through the section of the collection to be sorted.
            for (int i = startIndex; i < lastIndex - 1; ++i)
            {
                int minIndex = i; // The index containing the smallest value.

                // For the remaining indexes, continuously compare to get the smallest value.
                for (int j = i; j < lastIndex; ++j)
                {
                    if (compare(iList[minIndex], iList[j]) > 0)
                        minIndex = j;
                }

                iList.SwapValues(minIndex, i); // Swap the minimum index with the current index.
            }
        }

        /// <summary>
        /// Sorts a given collection via the Quick Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void QuickSort<T>(IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            QuickSort(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Sorts a given collection via the Quick Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void QuickSort<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            MinMaxException<int>.CheckAndThrow(startIndex, lastIndex, false);
            
            if (lastIndex - startIndex < 2)
                return;

            int[] stack = new int[lastIndex - startIndex]; // Create a stack of the swaps required.
            int stackTopIndex = -1; // The top index of the stack.
            int stackLow = startIndex; // The low value of the stack.
            int stackHigh = lastIndex - 1; // The high value of the stack.

            // Initialize the stack by pushing on the low and high values.
            stack[++stackTopIndex] = stackLow;
            stack[++stackTopIndex] = stackHigh;

            // Continue removing from the stack until all swaps are finished.
            while (stackTopIndex >= 0)
            {
                // Remove the current high and low values from the stack.
                stackHigh = stack[stackTopIndex--];
                stackLow = stack[stackTopIndex--];

                // Find a pivot value at random and set it to its sorted position.
                int pivot = QuickSortGetPivot(iList, compare, stackLow, stackHigh);

                // If there are elements to the left of the pivot, add them to the stack.
                if (pivot - 1 > stackLow)
                {
                    stack[++stackTopIndex] = stackLow;
                    stack[++stackTopIndex] = pivot - 1;
                }

                // If there are elements to the right of the pivot, add them to the stack.
                if (pivot + 1 < stackHigh)
                {
                    stack[++stackTopIndex] = pivot + 1;
                    stack[++stackTopIndex] = stackHigh;
                }
            }
        }

        /// <summary>
        /// Sorts a given collection via the Insertion Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void InsertionSort<T>(IList<T> iList, Comparison<T> compare, int startIndex = 0)
        {
            InsertionSort(iList, compare, startIndex, iList.Count);
        }

        /// <summary>
        /// Sorts a given collection via the Insertion Sort method.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        public static void InsertionSort<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            for (int i = startIndex + 1; i < lastIndex; ++i)
            {
                T currentKey = iList[i]; // Get a key item to compare everything to.
                int j = i - 1; // Make an indexer for moving elements ahead.

                for (; j >= startIndex; --j)
                {
                    // If the current elements are in order with the key, break early.
                    if (compare(iList[j], currentKey) <= 0)
                        break;

                    iList[j + 1] = iList[j]; // Shift elements one ahead of their current position.
                }

                iList[j + 1] = currentKey; // Place the key back into the ilist, regardless of the break.
            }
        }
        
        /// <summary>
        /// A <see cref="Comparison{T}"/> method for sorting least to greatest.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <typeparam name="T">The type being compared.</typeparam>
        /// <returns>Returns 0 if equal, -1 if <paramref name="a"/> is less, or 1 if <paramref name="b"/> is less.</returns>
        public static int CompareMinToMax<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b); // Return a's comparison to b.
        }
        
        /// <summary>
        /// A <see cref="Comparison{T}"/> method for sorting least to greatest.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <typeparam name="T">The type being compared.</typeparam>
        /// <returns>Returns 0 if equal, -1 if <paramref name="b"/> is less, or 1 if <paramref name="a"/> is less.</returns>
        public static int CompareMaxToMin<T>(T a, T b) where T : IComparable<T>
        {
            return b.CompareTo(a); // Return a's comparison to b.
        }

        /// <summary>
        /// Checks if a partition within a Divide-And-Conquer Method sort is sorted.
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <param name="lastIndex">The exclusive index to check up to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns if the partition. is sorted.</returns>
        private static bool IsSortedDividedPartition<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            // Continue while the indexes do not pass each other.
            while (startIndex < lastIndex)
            {
                // If the elements are out of order, return false.
                if (compare(iList[startIndex], iList[lastIndex]) > 0)
                    return false;

                // Move the indexes closer to each other.
                ++startIndex;
                --lastIndex;
            }

            return true; // This partition is sorted.
        }

        /// <summary>
        /// Builds a heap for use with the Heap Sort Algorithm
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to check.</param>
        /// <param name="compare">The comparison delegate.</param>
        /// <param name="startIndex">The inclusive index to check from.</param>
        /// <param name="lastIndex">The exclusive index to check up to.</param>
        /// <param name="pivot">The pivot index to create the partition around.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        private static void HeapSortBuildHeap<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex, int pivot)
        {
            T pivotItem = iList[pivot]; // Get the item that represents the pivot.

            // Continue until the heap is built.
            while (true)
            {
                // Create two pivot indexes around the given pivot.
                int pivotLow = startIndex + (pivot - startIndex) * 2 + 1;
                int pivotHigh = pivotLow + 1;

                // If past the max, break immediately.
                if (pivotLow >= lastIndex)
                    break;

                T itemLeft = iList[pivotLow]; // Get the first compared value.

                // If the higher pivot is valid, compare with a second value.
                if (pivotHigh < lastIndex)
                {
                    T itemRight = iList[pivotHigh]; // Get the second compared value.

                    // If the values are in order, update the low item and pivot.
                    if (compare(itemLeft, itemRight) <= 0)
                    {
                        pivotLow = pivotHigh;
                        itemLeft = itemRight;
                    }
                }

                // If the pivot is in order with the left, lower item, then this can break.
                if (compare(itemLeft, pivotItem) <= 0)
                    break;

                // Otherwise, swap the values, and update the main pivot.
                iList.SwapValues(pivot, pivotLow);
                pivot = pivotLow;
            }
        }
        
        /// <summary>
        /// Generates a pivot position for the Quick Sort Algorithm
        /// </summary>
        /// <param name="iList">The <see cref="IList"/> to sort.</param>
        /// <param name="compare">The comparison function to use.</param>
        /// <param name="startIndex">The inclusive index to sort from.</param>
        /// <param name="lastIndex">The exclusive index to sort to.</param>
        /// <typeparam name="T">The type stored in the <paramref name="iList"/>.</typeparam>
        /// <returns>Returns the pivot position to use.</returns>
        private static int QuickSortGetPivot<T>(IList<T> iList, Comparison<T> compare, int startIndex, int lastIndex)
        {
            // Generate a random pivot for better accuracy and speed.
            int randomPivot = RandomKit.DefaultGenerator.GetRandomIntII(startIndex, lastIndex);
            iList.SwapValues(randomPivot, lastIndex);
            T pivotItem = iList[lastIndex];

            int pivotIndex = startIndex - 1; // Get a starting index.

            // Loop through the entire collection section.
            for (int i = startIndex; i < lastIndex; ++i)
            {
                // If the pivot is not in order, swap the value and increment the pivot.
                if (compare(pivotItem, iList[i]) <= 0) 
                    continue;
                
                ++pivotIndex;
                iList.SwapValues(pivotIndex, i);
            }

            // Make one final swap with the highest index, before returning.
            ++pivotIndex;
            iList.SwapValues(pivotIndex, lastIndex);
            return pivotIndex;
        }
    }
}