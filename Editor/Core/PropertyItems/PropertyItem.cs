// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A base class for any property item. Resorts to <see cref="object"/>.
    /// </summary>
    public abstract class PropertyItem : IDrawerItem
    {
        public delegate void PropertyItemEvent(PropertyItem item);

        /// <summary>An event called when the item finishes drawing.</summary>
        public event PropertyItemEvent OnDrawComplete;

        /// <summary>See: <see cref="Label"/></summary>
        private PropertyLabel _label;

        /// <summary>The value obtained on the last draw call.</summary>
        public object LastDrawnValue { get; private set; }

        /// <summary>The initial <see cref="Rect"/> before drawing.</summary>
        protected Rect InitialRect { get; private set; }

        public float Weight { get; set; } = 100;
        
        public bool IsFixedWeight { get; set; }

        public int IndentAmount { get; set; }

        public PropertyLabel Label { get { return _label; } set { _label = value ?? new NormalPropertyLabel(); } }

        public virtual GUIStyle Style { get; set; }

        public bool AutoDrawLabel { get; set; } = true;

        protected PropertyItem()
        {
            Label = null;
        }

        protected PropertyItem(PropertyLabel label)
        {
            Label = label;
        }

        protected PropertyItem(GUIContent label, GUIStyle overrideStyle = null)
        {
            Label = new NormalPropertyLabel(label, overrideStyle);
        }

        public virtual bool CanDraw()
        {
            return true;
        }

        public void Draw(ref Rect drawRect)
        {
            InitialRect = drawRect;

            if (AutoDrawLabel)
                Label?.DrawLabel(ref drawRect);

            LastDrawnValue = OnDraw(ref drawRect);

            if (AutoDrawLabel)
                Label?.CleanUpLabel(ref drawRect);

            OnDrawComplete?.Invoke(this);
        }

        public virtual float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public virtual bool ApplyGenericEditorValue(EditorValue<object> genericValue)
        {
            throw new NotSupportedException($"Type {GetType()} does not support generic value application!");
        }

        /// <summary>
        /// An event called when drawing the item.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> to draw with.</param>
        /// <returns>Returns the final value after drawing.</returns>
        protected abstract object OnDraw(ref Rect drawRect);
    }
}
#endif