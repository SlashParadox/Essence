// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="PreviewGatherer"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(PreviewGatherer))]
    public class PreviewGathererDrawer : EssencePropertyDrawer
    {
        /// <summary>The <see cref="VerticalPropertyGroup"/> for the drawer.</summary>
        protected PropertyGroup VGroup;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return VGroup?.GetHeight() ?? 0.0f;
        }

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            base.OnDrawerInitialized(property, label);
            
            // Find the gatherer.
            bool success = ReflectionKit.GetFieldValue(property.serializedObject.targetObject, out PreviewGatherer gatherer, property.propertyPath);
            if (!success)
                return;

            // Make a label.
            PropertyLabel foldoutLabel = new FoldoutPropertyLabel(label);
            VGroup = new VerticalPropertyGroup(label);
            VGroup.Label = foldoutLabel;

            // Gather the members.
            List<MemberInfo> members = gatherer.GatherAttributeMembers(property.serializedObject.targetObject);

            // Create a generic property item for every property.
            foreach (MemberInfo member in members)
            {
                EditorValue<object> objValue = new EditorValue<object>(true, property.serializedObject.targetObject, member);
                GUIContent itemLabel = new GUIContent(objValue.GetVariableName(), EditorCache.TryFindTooltip(member));
                PropertyItem item = new GenericPropertyItem(objValue, itemLabel);

                VGroup.AddItem(item);
            }
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = false;

            VGroup?.Draw(ref position);

            GUI.enabled = enabled;
        }
    }
}
#endif