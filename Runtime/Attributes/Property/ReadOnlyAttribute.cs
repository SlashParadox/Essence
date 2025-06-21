// Copyright (c) Craig Williams, SlashParadox

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

        /// <summary>If set, a callback method is consistently checked to see if the variable can be edited.</summary>
        public string CallbackMethod { get; }
#endif

        public ReadOnlyAttribute() : this(false, true) { }

        public ReadOnlyAttribute(bool applyOnCollection) : this(false, applyOnCollection) { }

        public ReadOnlyAttribute(bool playModeOnly, bool applyOnCollection) : base(applyOnCollection)
        {
#if UNITY_EDITOR
            PlayModeOnly = playModeOnly;
#endif
        }

        public ReadOnlyAttribute(string callbackMethod, bool playModeOnly, bool applyOnCollection = true) : this(playModeOnly, applyOnCollection)
        {
#if UNITY_EDITOR
            CallbackMethod = callbackMethod;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Checks if the attribute has modifiers. Used to know the starting state of the drawer.
        /// </summary>
        /// <returns>Returns if the attribute has any special modifiers.</returns>
        public bool HasModifiers()
        {
            return PlayModeOnly || !string.IsNullOrEmpty(CallbackMethod);
        }
#endif
    }
}
