using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeAttributeDrawer : EssencePropertyDrawer
    {
        protected override bool IsDrawerValid(SerializedProperty property)
        {
            return base.IsDrawerValid(property) && property.propertyType is SerializedPropertyType.Vector2 or SerializedPropertyType.Vector2Int;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            MinMaxRangeAttribute atr = attribute as MinMaxRangeAttribute;
            if (atr == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            bool isFloat = property.propertyType == SerializedPropertyType.Vector2;
            Vector2 value = isFloat ? property.vector2Value : property.vector2IntValue;
            float min = value.x;
            float max = value.y;

            EditorGUI.MinMaxSlider(position, label, ref min, ref max, atr.Range.x, atr.Range.y);

            if (isFloat)
                property.vector2Value = new Vector2(min, max);
            else
                property.vector2IntValue = new Vector2Int((int)min, (int)max);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
