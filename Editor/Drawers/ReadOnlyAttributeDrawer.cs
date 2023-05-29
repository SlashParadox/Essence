// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="ReadOnlyAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : EssencePropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            bool enabled = GUI.enabled;
            
            ReadOnlyAttribute atr = attribute as ReadOnlyAttribute;
            GUI.enabled = atr == null || (atr.PlayModeOnly && !EditorApplication.isPlayingOrWillChangePlaymode);
            EditorGUI.PropertyField(position, property, label);

            GUI.enabled = enabled;
        }
    }
}
#endif