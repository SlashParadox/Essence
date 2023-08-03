// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A generic <see cref="PropertyValueItem{T}"/> for any <see cref="object"/>. This will
    /// find other <see cref="PropertyItem"/>s registered with a <see cref="CustomPropertyItemAttribute"/>.
    /// </summary>
    public class GenericPropertyItem : PropertyValueItem<object>
    {
        public override EditorValue<object> Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
                BuildGenericPropertyItem();
            }
        }

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
            if (Value?.SProperty != null)
            {
                EditorGUI.PropertyField(drawRect, Value.SProperty, Label?.ConditionalLabel, true);
                return Value.LastResult;
            }
            
            _vGroup?.Draw(ref drawRect);
            
            Value?.SetLatestResult(Value?.GetCurrentValue());
            return Value?.LastResult;
        }

        public override float GetHeight()
        {
            if (Value?.SProperty != null)
                return EditorGUI.GetPropertyHeight(Value.SProperty, Label?.ConditionalLabel);

            return _vGroup?.GetHeight() ?? 0.0f;
        }

        /// <summary>
        /// Builds the generic property recursively.
        /// </summary>
        private void BuildGenericPropertyItem()
        {
            // Don't draw serialized properties. We just use the normal property fields for them.
            if (Value?.SProperty != null)
                return;
            
            _vGroup = new VerticalPropertyGroup();
            _vGroup.AddItem(GetDrawerItemForValue(Value, Label));
        }

        /// <summary>
        /// Finds a <see cref="PropertyItem"/> for a given value.
        /// </summary>
        /// <param name="inValue">The value to find the <see cref="PropertyItem"/> for.</param>
        /// <param name="inLabel">The <see cref="PropertyLabel"/> for the item.</param>
        /// <returns></returns>
        private IDrawerItem GetDrawerItemForValue(EditorValue<object> inValue, PropertyLabel inLabel)
        {
            // If we can find a proper property item, use it. Otherwise, dive deeper.
            System.Type variableType = inValue?.GetVariableType();
            if (variableType == null)
                return null;
            
            PropertyItem valueItem = EditorCache.GetPropertyItem<PropertyItem>(variableType);
            if (valueItem != null)
            {
                valueItem.ApplyGenericEditorValue(inValue);
                valueItem.Label = inLabel;

                return valueItem;
            }

            VerticalPropertyGroup propertyGroup = new VerticalPropertyGroup();
            propertyGroup.Label = new FoldoutPropertyLabel(inLabel?.ConditionalLabel, inLabel?.Style);

            // Get the member info of the inner variables, and find a drawer for each.
            MemberInfo[] members = variableType.GetMembers(ReflectionKit.DefaultFlags);
            foreach (MemberInfo member in members)
            {
                FieldInfo field = member as FieldInfo;
                if (field == null)
                    continue;
                
                // Only show the member if public or previewed.
                if (!field.IsPublic && field.GetCustomAttribute<PreviewAttribute>() == null)
                    continue;

                EditorValue<object> newValue = new EditorValue<object>(true, inValue.GetCurrentValue(), field);
                GUIContent newLabel = new GUIContent(field.Name, EditorCache.TryFindTooltip(field));
                PropertyLabel newPropLabel = new NormalPropertyLabel(newLabel);
                propertyGroup.AddItem(GetDrawerItemForValue(newValue, newPropLabel));
                
            }

            return propertyGroup.GetItemCount() > 0 ? propertyGroup : null;
        }
    }
}
#endif