// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="PropertyAttribute"/> for making serialized properties read-only in the editor view.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        /// <summary>If true, the property is only read-only in Play Mode.</summary>
        public bool PlayModeOnly { get; }
#endif

        public ReadOnlyAttribute()
        {
#if UNITY_EDITOR
            PlayModeOnly = false;
#endif
        }

        public ReadOnlyAttribute(bool playModeOnly)
        {
#if UNITY_EDITOR
            PlayModeOnly = playModeOnly;
#endif
        }
    }
}
#endif