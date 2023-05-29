// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyItem"/> with some <see cref="EditorValue{T}"/> attached.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="EditorValue{T}"/>.</typeparam>
    public abstract class PropertyValueItem<T> : PropertyItem
    {
        /// <summary>The value being used and displayed in this property.</summary>
        public EditorValue<T> Value;
        
        protected PropertyValueItem() { }
        
        protected PropertyValueItem(EditorValue<T> value, GUIContent label)
            : base(label)
        {
            Value = value;
        }

        protected PropertyValueItem(EditorValue<T> value, PropertyLabel label)
            : base(label)
        {
            Value = value;
        }
        
        protected PropertyValueItem(EditorValue<T> value, bool bMakeLabelFromValue = true)
            : base(null)
        {
            Value = value;

            if (Value == null || !bMakeLabelFromValue || Value.SProperty == null)
                return;
            
            GUIContent innerLabel = new GUIContent(Value.SProperty.displayName, Value.SProperty.tooltip);
            Label = new NormalPropertyLabel(innerLabel);
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            return Value.LastResult;
        }

        public override bool ApplyGenericEditorValue(EditorValue<object> genericValue)
        {
            System.Type genericType = genericValue.GetVariableType();
            if (genericType == null || !genericType.IsOrIsSubclassOf(typeof(T)))
                return false;
            
            Value = new EditorValue<T>(genericValue);
            return true;
        }
    }
}
#endif