// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="PropertyAttribute"/> that allows setting up instances of inherited classes in the editor, thus allowing for serialization of child classes.
    /// Must be paired with a <see cref="SerializeReference"/> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InstancedAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        /// <summary>The display grouping method to use.</summary>
        public TypeGrouping Grouping { get; }
#endif

        public InstancedAttribute()
        {
#if UNITY_EDITOR
            Grouping = TypeGrouping.None;
#endif
        }

        public InstancedAttribute(TypeGrouping group)
        {
#if UNITY_EDITOR
            Grouping = group;
#endif
        }
    }
}