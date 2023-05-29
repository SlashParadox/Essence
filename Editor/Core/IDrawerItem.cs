// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// An <see langword="interface"/> for <see cref="PropertyDrawer"/> items, which formats itself automatically.
    /// </summary>
    public interface IDrawerItem
    {
        /// <summary>
        /// The weight of this item when spacing it out. <see cref="PropertyGroup"/>s tally this weight up, and
        /// distribute the space in some way based on how big of a percentage the weight is. The easiest way to
        /// use this is by ensuring all items add up to a weight of 100, but this is not a hard rule.
        /// </summary>
        public float SpaceWeight { get; set; }
        
        /// <summary>The amount of indentation to apply to this item.</summary>
        public int IndentAmount { get; set; }

        /// <summary>An optional label to use.</summary>
        public PropertyLabel Label { get; set; }
        
        /// <summary>An optional style to use.</summary>
        public GUIStyle Style { get; set; }
        
        /// <summary>If true, the item should ideally automatically handle drawing the label at the start.</summary>
        public bool AutoDrawLabel { get; set; }

        /// <summary>
        /// Checks if the item can be drawn at all.
        /// </summary>
        /// <returns>Returns whether or not the item can be drawn currently.</returns>
        public bool CanDraw();

        /// <summary>
        /// Draws an <see cref="IDrawerItem"/>, using the given <see cref="Rect"/>.
        /// </summary>
        /// <param name="drawRect">The <see cref="Rect"/> of the property to draw.</param>
        /// <remarks><paramref name="drawRect"/> is automatically updated by <see cref="PropertyGroup"/>s. As such,
        /// only actually edit the drawRect if necessary.</remarks>
        public void Draw(ref Rect drawRect);

        /// <summary>
        /// Gets the current total height of the <see cref="IDrawerItem"/>.
        /// </summary>
        /// <returns>Returns the maximum height.</returns>
        public float GetHeight();
    }
}
#endif