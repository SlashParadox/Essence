// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SlashParadox.Essence.Kits;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base class for a gatherer of properties with a certain <see cref="GatherableAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="GatherableAttribute"/>.</typeparam>
    public abstract class PropertyGatherer<T> where T : GatherableAttribute
    {
#if UNITY_EDITOR
        /// <summary>If set, this gatherer will only get objects of the same group. Otherwise, it will get all ungrouped items.</summary>
        public string Group;
#endif

        protected PropertyGatherer(string group = null)
        {
#if UNITY_EDITOR
            Group = group;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gathers all members with the <see cref="GatherableAttribute"/> from an <see cref="object"/>.
        /// </summary>
        /// <param name="inObject">The <see cref="object"/> to get members off of.</param>
        /// <returns>Returns the <see cref="MemberInfo"/> with the <see cref="GatherableAttribute"/>.</returns>
        public List<MemberInfo> GatherAttributeMembers(object inObject)
        {
            if (inObject == null)
                return null;

            List<MemberInfo> outMembers = new List<MemberInfo>();

            // Go through all members from the start.
            MemberInfo[] members = inObject.GetType().GetMembers(ReflectionKit.DefaultFlags);
            foreach (MemberInfo member in members)
            {
                // Get the attribute off of it.
                T attribute = member.GetCustomAttribute<T>();
                if (attribute == null)
                    continue;

                // If this gatherer can only have a specific group, find it.
                if (string.Compare(attribute.Group, Group, false, CultureInfo.InvariantCulture) != 0)
                {
                    if (!(string.IsNullOrEmpty(attribute.Group) && string.IsNullOrEmpty(Group)))
                        continue;
                }

                if (!IsMemberValid(member))
                    continue;

                outMembers.Add(member);
            }

            SortKit.InsertionSort(outMembers, GetSortComparison());

            return outMembers;
        }

        /// <summary>
        /// Gets a sort comparison function to sort out members at the end.
        /// </summary>
        /// <returns>Returns the comparison function.</returns>
        protected virtual Comparison<MemberInfo> GetSortComparison()
        {
            return (a, b) => { return a.GetCustomAttribute<T>().SortOrder.CompareTo(b.GetCustomAttribute<T>().SortOrder); };
        }

        /// <summary>
        /// Checks if some <see cref="MemberInfo"/> is valid and can be gathered.
        /// </summary>
        /// <param name="info">The <see cref="MemberInfo"/> to check.</param>
        /// <returns>Returns whether or not the <paramref name="info"/> is gatherable.</returns>
        protected virtual bool IsMemberValid(MemberInfo info)
        {
            return true;
        }
#endif
    }
}
#endif