// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// An easy to use grouping for <see cref="IDrawerItem"/>s. Use this to draw multiple items in a row.
    /// </summary>
    public abstract class PropertyGroup : IDrawerItem
    {
        /// <summary>The <see cref="IDrawerItem"/>s to draw in this group.</summary>
        protected readonly List<IDrawerItem> DrawerItems;

        /// <summary>If true, the label was drawn for this item.</summary>
        private bool _wasLabelDrawn;

        public float SpaceWeight { get; set; } = 100;

        public int IndentAmount { get; set; }

        public PropertyLabel Label { get; set; }

        public GUIStyle Style { get; set; }

        public bool AutoDrawLabel { get; set; } = true;

        protected PropertyGroup(int estimatedElements = 0)
        {
            DrawerItems = new List<IDrawerItem>(System.Math.Max(estimatedElements, 0));
        }

        public virtual bool CanDraw()
        {
            return true;
        }

        public void Draw(ref Rect drawRect)
        {
            if (DrawerItems.IsEmptyOrNull() || !CanDraw())
                return;

            EditorGUI.indentLevel += IndentAmount;
            drawRect.height = EditorGUIUtility.singleLineHeight;
            
            if (AutoDrawLabel)
                DrawlLabel(ref drawRect);
            
            OnDrawStart(ref drawRect);

            if (CanDrawPastLabel())

                // Draw every item in a row.
                foreach (IDrawerItem item in DrawerItems)
                {
                    OnBeforeItemDrawn(ref drawRect, item);
                    item.Draw(ref drawRect);
                    OnAfterItemDrawn(ref drawRect, item);
                }

            if (AutoDrawLabel)
                CleanUpLabel(ref drawRect);
            
            OnDrawEnd(ref drawRect);
            EditorGUI.indentLevel -= IndentAmount;
        }

        public virtual float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Adds a <see cref="IDrawerItem"/> to this <see cref="PropertyGroup"/>.
        /// </summary>
        /// <param name="item">The <see cref="IDrawerItem"/> to add.</param>
        /// <returns>Returns whether or not the item was added.</returns>
        public bool AddItem(IDrawerItem item)
        {
            if (item == null)
                return false;

            DrawerItems.Add(item);
            OnItemAdded(item);
            return true;
        }

        /// <summary>
        /// Adds several <see cref="IDrawerItem"/>s to this <see cref="PropertyGroup"/>.
        /// </summary>
        /// <param name="items">The <see cref="IDrawerItem"/>s to add.</param>
        public void AddItems(params IDrawerItem[] items)
        {
            if (items.IsEmptyOrNull())
                return;

            foreach (IDrawerItem item in items)
            {
                AddItem(item);
            }
        }

        /// <summary>
        /// Removes a <see cref="IDrawerItem"/> at the given <see cref="index"/>.
        /// </summary>
        /// <param name="index">The index of the <see cref="IDrawerItem"/> to remove.</param>
        /// <returns>Returns if the item was removed.</returns>
        public bool RemoveItem(int index)
        {
            if (!DrawerItems.IsValidIndex(index))
                return false;

            IDrawerItem item = DrawerItems[index];
            DrawerItems.RemoveAt(index);
            OnItemRemoved(item);

            return true;
        }

        /// <summary>
        /// Gets the number of <see cref="IDrawerItem"/>s contained.
        /// </summary>
        /// <returns>Returns the number of <see cref="IDrawerItem"/>s contained.</returns>
        public float GetItemCount()
        {
            return DrawerItems.Count;
        }

        /// <summary>
        /// An event when a <see cref="IDrawerItem"/> is added to the group.
        /// </summary>
        /// <param name="item">The new <see cref="IDrawerItem"/>.</param>
        protected virtual void OnItemAdded(IDrawerItem item) { }

        /// <summary>
        /// An event when a <see cref="IDrawerItem"/> is removed from the group.
        /// </summary>
        /// <param name="item">The removed <see cref="IDrawerItem"/>.</param>
        protected virtual void OnItemRemoved(IDrawerItem item) { }

        /// <summary>
        /// Draws a label, if wanted.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        protected virtual void DrawlLabel(ref Rect drawRect)
        {
            Label?.DrawLabel(ref drawRect);
        }

        /// <summary>
        /// Cleans up a label, if one was drawn.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        protected virtual void CleanUpLabel(ref Rect drawRect)
        {
            Label?.CleanUpLabel(ref drawRect);
        }

        /// <summary>
        /// Checks if more elements past the label can be drawn.
        /// </summary>
        /// <returns>Returns whether or not to draw past the label.</returns>
        protected virtual bool CanDrawPastLabel()
        {
            return Label == null || Label.CanDrawFurtherElements();
        }

        /// <summary>
        /// An event called just before drawing the <see cref="DrawerItems"/>.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the group.</param>
        /// <returns>Returns if further elements can be drawn.</returns>
        protected virtual void OnDrawStart(ref Rect drawRect) { }

        /// <summary>
        /// An event called just at the end of drawing the <see cref="DrawerItems"/>.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the group.</param>
        protected virtual void OnDrawEnd(ref Rect drawRect) { }

        /// <summary>
        /// An event called when a <see cref="IDrawerItem"/> is about to be drawn.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        /// <param name="item">The new <see cref="IDrawerItem"/>.</param>
        protected virtual void OnBeforeItemDrawn(ref Rect drawRect, IDrawerItem item) { }

        /// <summary>
        /// An event called when a <see cref="IDrawerItem"/> is finished being drawn.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        /// <param name="item">The removed <see cref="IDrawerItem"/>.</param>
        protected virtual void OnAfterItemDrawn(ref Rect drawRect, IDrawerItem item) { }

        /// <summary>
        /// Gets the amount of spacing to have between lines. May not be used.
        /// </summary>
        /// <returns>Returns the amount of space between lines.</returns>
        protected virtual float GetLineSpacing()
        {
            return EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif