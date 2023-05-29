// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Color"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Color))]
    public class ColorPropertyItem : PropertyValueItem<Color>
    {
        /// <summary>If true, the eyedropper is shown.</summary>
        public bool ShowEyedropper = true;

        /// <summary>If true, the alpha is shown.</summary>
        public bool ShowAlpha = true;

        /// <summary>If true, HDR is shown.</summary>
        public bool ShowHDR = false;

        public ColorPropertyItem() { }

        public ColorPropertyItem(EditorValue<Color> value, GUIContent label)
            : base(value, label) { }

        public ColorPropertyItem(EditorValue<Color> value, PropertyLabel label)
            : base(value, label) { }

        public ColorPropertyItem(EditorValue<Color> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value == null)
                return null;
            
            Color result = EditorGUI.ColorField(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), ShowEyedropper, ShowAlpha, ShowHDR);
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif