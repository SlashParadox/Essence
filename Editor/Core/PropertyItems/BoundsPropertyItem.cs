// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Bounds"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Bounds))]
    public class BoundsPropertyItem : PropertyValueItem<Bounds>
    {
        public BoundsPropertyItem() { }

        public BoundsPropertyItem(EditorValue<Bounds> value, GUIContent label)
            : base(value, label) { }

        public BoundsPropertyItem(EditorValue<Bounds> value, PropertyLabel label)
            : base(value, label) { }

        public BoundsPropertyItem(EditorValue<Bounds> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Bounds result = EditorGUI.BoundsField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }

    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="BoundsInt"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(BoundsInt))]
    public class BoundsIntPropertyItem : PropertyValueItem<BoundsInt>
    {
        public BoundsIntPropertyItem() { }

        public BoundsIntPropertyItem(EditorValue<BoundsInt> value, GUIContent label)
            : base(value, label) { }

        public BoundsIntPropertyItem(EditorValue<BoundsInt> value, PropertyLabel label)
            : base(value, label) { }

        public BoundsIntPropertyItem(EditorValue<BoundsInt> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            BoundsInt result = EditorGUI.BoundsIntField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif