// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Color"/>.
    /// </summary>
    [CustomPropertyItem(typeof(AnimationCurve))]
    public class CurvePropertyItem : PropertyValueItem<AnimationCurve>
    {
        /// <summary>The color of the curve.</summary>
        public Color CurveColor = Color.green;

        /// <summary>An optional rect to limit the curve in.</summary>
        public Rect CurveRect;

        public CurvePropertyItem() { }

        public CurvePropertyItem(EditorValue<AnimationCurve> value, GUIContent label)
            : base(value, label) { }

        public CurvePropertyItem(EditorValue<AnimationCurve> value, PropertyLabel label)
            : base(value, label) { }

        public CurvePropertyItem(EditorValue<AnimationCurve> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value == null)
                return null;
            
            AnimationCurve result = EditorGUI.CurveField(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), CurveColor, CurveRect);
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif