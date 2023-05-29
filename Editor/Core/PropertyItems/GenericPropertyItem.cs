// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A generic <see cref="PropertyValueItem{T}"/> for any <see cref="object"/>. This will
    /// find other <see cref="PropertyItem"/>s registered with a <see cref="CustomPropertyItemAttribute"/>.
    /// </summary>
    public class GenericPropertyItem : PropertyValueItem<object>
    {
        /// <summary>The <see cref="VerticalPropertyGroup"/> to place items into.</summary>
        private VerticalPropertyGroup _vGroup;

        public GenericPropertyItem(EditorValue<object> value, GUIContent label)
            : base(value, label)
        {
            BuildGenericPropertyItem();
        }

        public GenericPropertyItem(EditorValue<object> value, PropertyLabel label)
            : base(value, label)
        {
            BuildGenericPropertyItem();
        }

        public GenericPropertyItem(EditorValue<object> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue)
        {
            BuildGenericPropertyItem();
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            _vGroup?.Draw(ref drawRect);
            
            Value.SetLatestResult(Value.GetCurrentValue());
            return Value.LastResult;
        }

        public override float GetHeight()
        {
            return _vGroup?.GetHeight() ?? 0.0f;
        }

        /// <summary>
        /// Builds the generic property recursively.
        /// </summary>
        private void BuildGenericPropertyItem()
        {
            _vGroup = new VerticalPropertyGroup();
            _vGroup.AddItem(GetDrawerItemForValue(Value, Label));
        }

        /// <summary>
        /// Finds a <see cref="PropertyItem"/> for a given value.
        /// </summary>
        /// <param name="inValue">The value to find the <see cref="PropertyItem"/> for.</param>
        /// <param name="inLabel">The <see cref="PropertyLabel"/> for the item.</param>
        /// <returns></returns>
        private PropertyItem GetDrawerItemForValue(EditorValue<object> inValue, PropertyLabel inLabel)
        {
            PropertyItem valueItem = EditorCache.GetPropertyItem<PropertyItem>(inValue.GetVariableType());
            if (valueItem == null)
                return null;

            System.Type valueType = inValue.GetVariableType();
            

            valueItem.ApplyGenericEditorValue(inValue);
            valueItem.Label = inLabel;

            return valueItem;
        }
    }
}
#endif