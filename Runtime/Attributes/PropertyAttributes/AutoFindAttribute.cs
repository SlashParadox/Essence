// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="PropertyAttribute"/> to automatically set a <see cref="GameObject"/> or <see cref="Component"/> variable.
    /// </summary>
    public class AutoFindAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        /// <summary>The index of the <see cref="Component"/>, if there are multiple.</summary>
        public int Index { get; private set; }

        /// <summary>The name of the <see cref="GameObject"/> to search. Defaults to the current object.</summary>
        public string ObjectName { get; private set; }
#endif
        
        public AutoFindAttribute() : this(string.Empty) { }

        public AutoFindAttribute(int index) : this(string.Empty, index) { }

        public AutoFindAttribute(string objectName, int index = 0)
        {
#if UNITY_EDITOR
            Index = System.Math.Max(index, 0);
            ObjectName = objectName;
#endif
        }
    }
}
#endif