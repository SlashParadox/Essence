// Copyright (c) Craig Williams, SlashParadox

#if UNITY_EDITOR
using System;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="AutoFindAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(AutoFindAttribute))]
    public class AutoFindAttributeDrawer : EssencePropertyDrawer
    {
        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            AutoFindAttribute atr = attribute as AutoFindAttribute;
            if (atr == null || property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Component currentComponent = property.serializedObject.targetObject as Component;
            GameObject wantedObject = currentComponent ? currentComponent.gameObject : null;
            Type compType = fieldInfo.FieldType;

            if (compType.IsArray)
                compType = compType.GetElementType();
            
            if (property.objectReferenceValue != null && property.objectReferenceValue.GetType().IsOrIsSubclassOf(compType))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // If a specific game object is wanted, find it.
            if (!string.IsNullOrEmpty(atr.ObjectName))
            {
                Transform wantedTrans = wantedObject ? wantedObject.transform.Find(atr.ObjectName) : null;
                wantedObject = wantedTrans ? wantedTrans.gameObject : GameObject.Find(atr.ObjectName);
            }

            if (wantedObject)
            {
                // Get all of the components that match.
                Component[] components = wantedObject.GetComponentsInChildren(compType);

                // If the components were found, and the index is valid, set the value.
                if (components.IsValidIndex(atr.Index))
                    property.objectReferenceValue = components[atr.Index];
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif