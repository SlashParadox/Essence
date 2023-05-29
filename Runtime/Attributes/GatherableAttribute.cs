// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="PropertyAttribute"/> for <see cref="PropertyGatherer{T}"/>s.
    /// </summary>
    public abstract class GatherableAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        /// <summary>A sort order for the attribute. A lower number appears first.</summary>
        public int SortOrder;
        
        /// <summary>An optional group to add this item to. Otherwise, appears in an unsorted group.</summary>
        public string Group;
#endif

        protected GatherableAttribute(int sortOrder = 0, string group = null)
        {
#if UNITY_EDITOR
            SortOrder = sortOrder;
            Group = group;
#endif
        }
    }
}
#endif