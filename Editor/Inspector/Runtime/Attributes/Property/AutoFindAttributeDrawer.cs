// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="AutoFindAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(AutoFindAttribute))]
    public class AutoFindAttributeDrawer : EssencePropertyDrawer
    {
        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            PropertyField propertyField = new PropertyField(property);
            propertyField.RegisterValueChangeCallback(_ => FindComponentForProperty(property, data));
            FindComponentForProperty(property, data);

            return propertyField;
        }

        /// <summary>
        /// Finds a proper <see cref="Component"/> for a given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to find a value for. Must be looking for <see cref="Component"/>s.</param>
        /// <param name="data">Optional <see cref="PropertyDrawerData"/>.</param>
        private void FindComponentForProperty(SerializedProperty property, PropertyDrawerData data)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return;

            if (attribute is not AutoFindAttribute atr)
                return;

            Type compType = fieldInfo.FieldType;
            if (compType.IsArray)
                compType = compType.GetElementType();

            // Value already set. Ignore.
            if (property.objectReferenceValue && compType != null && property.objectReferenceValue.GetType().IsOrIsSubclassOf(compType))
                return;

            Component currentComponent = property.serializedObject.targetObject as Component;
            GameObject wantedObject = currentComponent ? currentComponent.gameObject : null;

            // If a specific game object is wanted, find it.
            if (!string.IsNullOrEmpty(atr.ObjectName))
            {
                Transform wantedTrans = wantedObject ? wantedObject.transform.Find(atr.ObjectName) : null;
                wantedObject = wantedTrans ? wantedTrans.gameObject : GameObject.Find(atr.ObjectName);
            }

            if (!wantedObject)
                return;

            // Get all of the components that match.
            Component[] components = wantedObject.GetComponentsInChildren(compType);

            // If the components were found, and the index is valid, set the value.
            if (components.IsValidIndex(atr.Index))
            {
                property.objectReferenceValue = components[atr.Index];
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
