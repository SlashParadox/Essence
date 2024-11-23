// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// <see cref="PropertyDrawerData"/> for any property drawer with <see cref="ObjectTag"/>s.
    /// </summary>
    internal class ObjectTagDrawerData : PropertyDrawerData
    {
        /// <summary>The top-level <see cref="IDrawerItem"/> to draw.</summary>
        public IDrawerItem TagPropertyItemContainer;
        
        /// <summary>If true, the related property is in tag edit mode.</summary>
        public bool IsEditing;
        
        /// <summary>The <see cref="ObjectTagPropertyItem"/> showcasing the tag tree.</summary>
        public ObjectTagPropertyItem TagPropertyItem;

        /// <summary>An organizing <see cref="PropertyGroup"/> just for <see cref="ObjectTagGroup"/>s.</summary>
        public PropertyGroup TagGroupContainer;
    }

    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="ObjectTag"/>s.
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectTag))]
    public class ObjectTagDrawer : EssencePropertyDrawer
    {
        /// <summary>The width of the scroll area.</summary>
        private const float ScrollWidth = 400.0f;
        
        /// <summary>The height of the scroll area.</summary>
        private const float ScrollHeight = 150.0f;

        /// <summary>The <see cref="SerializedProperty"/> of the inner string tag value.</summary>
        private SerializedProperty _currentTag;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ObjectTagDrawerData data = FindPropertyData<ObjectTagDrawerData>(property);
            if (data == null || !data.IsEditing || data.TagPropertyItem == null)
                return EditorGUIUtility.singleLineHeight;

            return data.TagPropertyItemContainer.GetHeight() + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        protected override PropertyDrawerData CreatePropertyData(SerializedProperty property, GUIContent label)
        {
            ObjectTagDrawerData data = new ObjectTagDrawerData();
            data.TagPropertyItem = new ObjectTagPropertyItem(false);
            data.TagPropertyItem.IndentAmount = 1;

            ScrollDrawerWrap scrollWrap = new ScrollDrawerWrap(data.TagPropertyItem, ScrollHeight, ScrollWidth);
            data.TagPropertyItemContainer = new BoxDrawerWrap(scrollWrap, new Vector2(0.0f, ScrollHeight));

            return data;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            if (data is not ObjectTagDrawerData tagData)
                return;

            _currentTag = property.FindPropertyRelative("tag");

            HorizontalPropertyGroup hGroup = new HorizontalPropertyGroup();
            hGroup.AddItem(new FunctionalPropertyItem<string>(rect => DrawTagName(rect, label)));
            hGroup.AddItem(new PropertySpacer(10.0f, true));

            InvokePropertyItem editItem = new InvokePropertyItem((ref Rect rect, SerializedProperty _) =>
                                                                 {
                                                                     if (GUI.Button(rect, tagData.IsEditing ? "Close" : "Edit"))
                                                                         tagData.IsEditing = !tagData.IsEditing;
                                                                 });
            editItem.Weight = 50.0f;
            editItem.IsFixedWeight = true;
            hGroup.AddItem(editItem);

            Rect hGroupRect = position;
            hGroup.Draw(ref hGroupRect);

            if (!tagData.IsEditing)
                return;

            ObjectTagGroup group = new ObjectTagGroup();
            group.AddTag(ObjectTag.FindTag(_currentTag.stringValue));
            tagData.TagPropertyItem.Value = new EditorValue<ObjectTagGroup>(false, group);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            tagData.TagPropertyItemContainer.Draw(ref position);

            ObjectTagGroup finalTags = tagData.TagPropertyItem.Value.LastResult;
            List<ObjectTag> listTags = finalTags.GetTags();
            ObjectTag singleTag = listTags.Count > 0 ? listTags[0] : ObjectTag.None;
            _currentTag.stringValue = singleTag.ToString();

            property.serializedObject.ApplyModifiedProperties();
        }

        private string DrawTagName(Rect rect, GUIContent label)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.TextField(rect, label, _currentTag.stringValue);
            GUI.enabled = enabled;

            return _currentTag.stringValue;
        }
    }

    [CustomPropertyDrawer(typeof(ObjectTagGroup))]
    public class ObjectTagGroupDrawer : EssencePropertyDrawer
    {
        /// <summary>The width of the scroll area.</summary>
        private const float ScrollWidth = 400.0f;
        
        /// <summary>The height of the scroll area.</summary>
        private const float ScrollHeight = 150.0f;
        
        /// <summary>The <see cref="SerializedProperty"/> of the inner string tag values.</summary>
        private SerializedProperty _currentTags;

        protected override PropertyDrawerData CreatePropertyData(SerializedProperty property, GUIContent label)
        {
            ObjectTagDrawerData data = new ObjectTagDrawerData();
            data.TagPropertyItem = new ObjectTagPropertyItem(true);
            data.TagPropertyItem.IndentAmount = 1;

            data.TagPropertyItemContainer = new ScrollDrawerWrap(null, ScrollHeight, ScrollWidth);

            data.TagGroupContainer = new VerticalPropertyGroup();
            data.TagGroupContainer.Label = new FoldoutPropertyLabel(label);
            data.TagGroupContainer.AddItem(new InvokePropertyItem((ref Rect rect, SerializedProperty _) =>
                                                                 {
                                                                     if (GUI.Button(rect, data.IsEditing ? "View Tags" : "Edit Tags"))
                                                                         data.IsEditing = !data.IsEditing;
                                                                 }));
            
            data.TagGroupContainer.AddItem(new BoxDrawerWrap(data.TagPropertyItemContainer, new Vector2(0.0f, ScrollHeight)));

            return data;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            if (data is not ObjectTagDrawerData tagData)
                return;

            _currentTags = property.FindPropertyRelative("tags");
            ReflectionKit.GetFieldValue(property.serializedObject.targetObject, ReflectionKit.DefaultFlags, out ObjectTagGroup tagGroup, ReflectionKit.BreakReflectionPath(property.propertyPath));
            tagData.TagPropertyItem.Value = new EditorValue<ObjectTagGroup>(false, tagGroup);

            if (tagData.TagPropertyItemContainer is DrawerWrap wrap)
                wrap.WrappedItem = tagData.IsEditing ? tagData.TagPropertyItem : CreateTagListDisplay(ref tagGroup);
            
            tagData.TagGroupContainer.Draw(ref position);

            if (!tagData.IsEditing)
                return;

            tagGroup = tagData.TagPropertyItem.Value.LastResult;
            List<ObjectTag> tags = tagGroup.GetTags();
            _currentTags.arraySize = tags.Count;

            for (int i = 0; i < tags.Count; ++i)
            {
                SerializedProperty itemProperty = _currentTags.GetArrayElementAtIndex(i);
                SerializedProperty tagProperty = itemProperty.FindPropertyRelative("tag");
                tagProperty.stringValue = tags[i].ToString();
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ObjectTagDrawerData data = FindPropertyData<ObjectTagDrawerData>(property);
            return data?.TagGroupContainer?.GetHeight() ?? EditorGUIUtility.singleLineHeight;
        }

        private PropertyGroup CreateTagListDisplay(ref ObjectTagGroup inGroup)
        {
            VerticalPropertyGroup group = new VerticalPropertyGroup();
            List<ObjectTag> tags = inGroup.GetTags();

            for (int i = 0; i < tags.Count; ++i)
            {
                int iValue = i;
                group.AddItem(new InvokePropertyItem(((ref Rect rect, SerializedProperty _) =>
                                                      {
                                                          EditorGUI.LabelField(rect, $"({iValue}) {tags[iValue].ToString()}");
                                                      })));
            }

            return group;
        }
    }
}