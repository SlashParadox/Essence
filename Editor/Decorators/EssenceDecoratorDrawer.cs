// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// An enhanced version of a <see cref="DecoratorDrawer"/>.
    /// </summary>
    public class EssenceDecoratorDrawer : DecoratorDrawer
    {
        /// <summary>If true, the drawer has been fully initialized.</summary>
        protected bool IsInitialized { get; private set; }

        public sealed override void OnGUI(Rect position)
        {
            if (position.width <= 1)
                return;

            if (!IsInitialized)
                InitializeDrawer();

            OnGUIDraw(position);
        }

        /// <summary>
        /// Initializes the <see cref="EssenceDecoratorDrawer"/>. Only ever called once.
        /// </summary>
        private void InitializeDrawer()
        {
            OnDrawerInitialized();
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes the <see cref="EssenceDecoratorDrawer"/>. Only ever called once.
        /// </summary>
        protected virtual void OnDrawerInitialized() { }

        /// <summary>
        /// Draws the custom IMGUI for a <see cref="EssenceDecoratorDrawer"/>. <see cref="OnDrawerInitialized"/> is
        /// guaranteed to have been called.
        /// </summary>
        /// <param name="position">The <see cref="Rect"/> used to position the drawer.</param>
        protected virtual void OnGUIDraw(Rect position)
        {
            base.OnGUI(position);
        }
    }
}
#endif