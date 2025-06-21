using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(Alias))]
    public class LabelDrawer : EssencePropertyDrawer
    {
        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            SerializedProperty internalNameProperty = property.FindPropertyRelative("internalName");
            internalNameProperty.stringValue = EditorGUI.TextField(position, label, internalNameProperty.stringValue);
            internalNameProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
