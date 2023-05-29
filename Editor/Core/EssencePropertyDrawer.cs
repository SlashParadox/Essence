// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// An enhanced version of a <see cref="PropertyDrawer"/>.
    /// </summary>
    public class EssencePropertyDrawer : PropertyDrawer
    {
        /// <summary>If true, the drawer has been fully initialized.</summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>The default <see cref="PropertyDrawer"/>, from the <see cref="EditorCache"/>.</summary>
        protected PropertyDrawer DefaultDrawer { get; private set; }

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width <= 1)
                return;

            if (!IsInitialized)
                InitializeDrawer(property, label);

            OnGUIDraw(position, property, label);
        }

        /// <summary>
        /// Initializes the <see cref="EssencePropertyDrawer"/>. Only ever called once.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        private void InitializeDrawer(SerializedProperty property, GUIContent label)
        {
            DefaultDrawer = EditorCache.GetPropertyDrawer(property);
            OnDrawerInitialized(property, label);
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes the <see cref="EssencePropertyDrawer"/>. Only ever called once.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        protected virtual void OnDrawerInitialized(SerializedProperty property, GUIContent label) { }

        /// <summary>
        /// Draws the custom IMGUI for a <see cref="EssencePropertyDrawer"/>. <see cref="OnDrawerInitialized"/> is
        /// guaranteed to have been called.
        /// </summary>
        /// <param name="position">The <see cref="Rect"/> used to position the drawer.</param>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        protected virtual void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
}
#endif