// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Color"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Gradient))]
    public class GradientPropertyItem : PropertyValueItem<Gradient>
    {
        /// <summary>If true, HDR is shown.</summary>
        public bool ShowHDR = false;

        /// <summary>The <see cref="ColorSpace"/> to use.</summary>
        public ColorSpace Space = ColorSpace.Gamma;
        
        public GradientPropertyItem() { }

        public GradientPropertyItem(EditorValue<Gradient> value, GUIContent label)
            : base(value, label) { }

        public GradientPropertyItem(EditorValue<Gradient> value, PropertyLabel label)
            : base(value, label) { }

        public GradientPropertyItem(EditorValue<Gradient> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value == null)
                return null;
            
            Gradient result = EditorGUI.GradientField(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), ShowHDR, Space);
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif