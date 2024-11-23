// Copyright (c) Craig Williams, SlashParadox

using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A drawer for a <see cref="SceneReferenceAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public sealed class SceneReferenceAttributeDrawer : EssencePropertyDrawer
    {
        protected override bool IsDrawerValid(SerializedProperty property)
        {
            return property != null && property.propertyType == SerializedPropertyType.String;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            SceneAsset previousScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

            if (!previousScene && !string.IsNullOrEmpty(property.stringValue))
            {
                Debug.LogError($"Scene path [{property.stringValue}] no longer valid! The scene may have moved or was deleted!", property.serializedObject.targetObject);
                property.stringValue = string.Empty;
            }

            EditorGUI.BeginChangeCheck();
            SceneAsset newScene = EditorGUI.ObjectField(position, label, previousScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
                property.stringValue = AssetDatabase.GetAssetPath(newScene);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}