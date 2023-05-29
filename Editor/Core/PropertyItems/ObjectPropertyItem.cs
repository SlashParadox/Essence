// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="UnityEngine.Object"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Object), true)]
    public class ObjectPropertyItem : PropertyValueItem<Object>
    {
        public ObjectPropertyItem() { }

        public ObjectPropertyItem(EditorValue<Object> value, GUIContent label)
            : base(value, label) { }

        public ObjectPropertyItem(EditorValue<Object> value, PropertyLabel label)
            : base(value, label) { }

        public ObjectPropertyItem(EditorValue<Object> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Rect initialRect = drawRect;
            
            if (Value == null)
                return null;

            if (Value.SProperty != null)
            {
                EditorGUI.ObjectField(drawRect, Value.SProperty, Label?.ConditionalLabel);
                return Value.GetCurrentValue();
            }

            if (Label != null && !Label.WasDrawn && Label.ConditionalLabel.IsValidGUIContent())
            {
                EditorGUI.LabelField(drawRect, Label.ConditionalLabel);
                drawRect.x += EditorGUIUtility.labelWidth;
                drawRect.width -= EditorGUIUtility.labelWidth;
            }

            Object result = EditorGUI.ObjectField(drawRect, Value.GetCurrentValue(), Value.GetVariableType(), true);
            Value.SetLatestResult(result);
            Label?.CleanUpLabel(ref drawRect);
            drawRect = initialRect;

            return result;
        }
    }
}
#endif