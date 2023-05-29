// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Rect"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Rect))]
    public class RectPropertyItem : PropertyValueItem<Rect>
    {
        public RectPropertyItem() { }

        public RectPropertyItem(EditorValue<Rect> value, GUIContent label)
            : base(value, label) { }

        public RectPropertyItem(EditorValue<Rect> value, PropertyLabel label)
            : base(value, label) { }

        public RectPropertyItem(EditorValue<Rect> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Rect result = EditorGUI.RectField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="RectInt"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(RectInt))]
    public class RectIntPropertyItem : PropertyValueItem<RectInt>
    {
        public RectIntPropertyItem() { }

        public RectIntPropertyItem(EditorValue<RectInt> value, GUIContent label)
            : base(value, label) { }

        public RectIntPropertyItem(EditorValue<RectInt> value, PropertyLabel label)
            : base(value, label) { }

        public RectIntPropertyItem(EditorValue<RectInt> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            RectInt result = EditorGUI.RectIntField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
        
        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2.0f + EditorGUIUtility.standardVerticalSpacing;
        }
        
    }
}
#endif