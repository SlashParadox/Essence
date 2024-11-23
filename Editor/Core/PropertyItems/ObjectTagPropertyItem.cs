// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> to draw <see cref="ObjectTag"/>s and <see cref="ObjectTagGroup"/>s.
    /// </summary>
    public sealed class ObjectTagPropertyItem : PropertyValueItem<ObjectTagGroup>
    {
        /// <summary>The <see cref="PropertyGroup"/> that displays all visible <see cref="ObjectTag"/>s.</summary>
        private readonly VerticalPropertyGroup _tagListGroup = new VerticalPropertyGroup();

        /// <summary>The <see cref="ObjectTag"/>s currently displaying their children.</summary>
        private readonly HashSet<string> _unfoldedTags = new HashSet<string>();

        /// <summary>If true, multiple tags can be selected. Otherwise, only one tag at a time can be selected.</summary>
        private readonly bool _allowMultiple;

        /// <summary>A cache of the root tag in the <see cref="EssenceTagSettings"/>.</summary>
        private readonly DataTreeNode _rootTag;

        /// <summary>Tags that have had their foldout status changed, and if they are now or no longer unfolded.</summary>
        private readonly List<Tuple<string, bool>> _updatedUnfoldedTags = new List<Tuple<string, bool>>();

        /// <summary>A cache of the current <see cref="ObjectTagGroup"/> in the <see cref="PropertyValueItem{T}.Value"/>.</summary>
        private ObjectTagGroup _currentValue;

        public ObjectTagPropertyItem(bool allowMultiple)
        {
            _allowMultiple = allowMultiple;

            // Get the root tag.
            ReflectionKit.GetPropertyValue<EssenceTagSettings, EssenceTagSettings>("CurrentSingleton", ReflectionKit.DefaultFlags, out EssenceTagSettings settings);
            ReflectionKit.GetFieldValue(settings, ReflectionKit.DefaultFlags, out _rootTag, "_rootTag");
        }

        public override float GetHeight()
        {
            return Value == null || _rootTag == null || _tagListGroup == null ? 0.0f : _tagListGroup.GetHeight();
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value == null)
                return new ObjectTagGroup();

            if (_tagListGroup == null || _rootTag == null)
                return Value.LastResult;

            // Reset our values.
            _currentValue = Value.GetCurrentValue();
            _updatedUnfoldedTags.Clear();
            _tagListGroup.ClearItems();

            // Update the tag list group to draw, and display.
            UpdateTagListGroup(_rootTag, 0);
            _tagListGroup.Draw(ref drawRect);

            foreach (Tuple<string, bool> update in _updatedUnfoldedTags)
            {
                if (update.Item2)
                    _unfoldedTags.Add(update.Item1);
                else
                    _unfoldedTags.Remove(update.Item1);
            }

            Value.SetLatestResult(_currentValue);
            return Value.LastResult;
        }

        /// <summary>
        /// Updates the <see cref="_tagListGroup"/> to display unfolded tags. This is recursive.
        /// </summary>
        /// <param name="root">The root <see cref="DataTreeNode"/> to draw the children of. The root is not drawn.</param>
        /// <param name="extraIndent">The current indentation level. Steps up by one every recursion.</param>
        private void UpdateTagListGroup(DataTreeNode root, int extraIndent)
        {
            root?.IterateChildren<DataTreeNode>(child =>
                                                {
                                                    ObjectTag tag = ExtractNodeTag(child);
                                                    DisplaySingleTagData(tag, child.HasChildren, extraIndent);

                                                    if (!_unfoldedTags.Contains(tag.ToString()))
                                                        return;

                                                    UpdateTagListGroup(child, extraIndent + 1);
                                                }, false);
        }

        /// <summary>
        /// Displays a single <see cref="ObjectTag"/>
        /// </summary>
        /// <param name="tag">The <see cref="ObjectTag"/> being drawn.</param>
        /// <param name="hasChildren">If true, the tag has child tags and should show a foldout.</param>
        /// <param name="extraIndent">The current indentation level.</param>
        private void DisplaySingleTagData(ObjectTag tag, bool hasChildren, int extraIndent)
        {
            if (!tag.IsValid)
                return;

            // Display the label.
            HorizontalPropertyGroup hGroup = new HorizontalPropertyGroup();
            hGroup.AddItem(new InvokePropertyItem((ref Rect rect, SerializedProperty _) => DrawTagLabel(tag.ToString(), hasChildren, rect)));
            hGroup.AddItem(new PropertySpacer(15.0f));

            // If this tag isn't part of the data, but a child is, show a mixed node.
            bool isSelected = _currentValue.HasTag(tag);
            bool isChildSelected = !isSelected && _currentValue.HasTag(tag, true);

            // Display a checkbox to add or remove the tag from the group.
            InvokePropertyItem toggle = new InvokePropertyItem((ref Rect rect, SerializedProperty _) => DrawSelectionToggle(tag, rect, isSelected, isChildSelected));
            toggle.IsFixedWeight = true;
            toggle.Weight = 25.0f;
            hGroup.AddItem(toggle);

            hGroup.IndentAmount = extraIndent;
            _tagListGroup.AddItem(hGroup);
        }

        /// <summary>
        /// Draws an <see cref="ObjectTag"/>'s label.
        /// </summary>
        /// <param name="tag">The tag to draw.</param>
        /// <param name="hasChildren">If true, the tag has child tags and should show a foldout.</param>
        /// <param name="rect">The rect to draw the label in.</param>
        private void DrawTagLabel(string tag, bool hasChildren, Rect rect)
        {
            if (!hasChildren || string.IsNullOrEmpty(tag))
            {
                EditorGUI.LabelField(rect, tag);
                return;
            }

            bool isDisplayed = _unfoldedTags.Contains(tag);
            bool newDisplayed = EditorGUI.Foldout(rect, isDisplayed, tag, true);
            if (newDisplayed == isDisplayed)
                return;

            _updatedUnfoldedTags.Add(new Tuple<string, bool>(tag, newDisplayed));
        }

        /// <summary>
        /// Draws a selection toggle to turn the tag on or off.
        /// </summary>
        /// <param name="tag">The drawn tag.</param>
        /// <param name="rect">The rect to draw the toggle in.</param>
        /// <param name="isSelected">If true, the tag itself is selected.</param>
        /// <param name="isChildSelected">If true, a child of the tag is selected.</param>
        private void DrawSelectionToggle(ObjectTag tag, Rect rect, bool isSelected, bool isChildSelected)
        {
            bool isShowingMixed = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = !isSelected && isChildSelected;

            bool newSelected = EditorGUI.Toggle(rect, isSelected);

            EditorGUI.showMixedValue = isShowingMixed;
            
            if (isSelected == newSelected)
                return;

            bool bForceOff = Event.current.shift && Event.current.clickCount > 0;
            if (!newSelected || bForceOff)
            {
                _currentValue.RemoveTag(tag, Event.current.shift);
                return;
            }

            if (!_allowMultiple)
                _currentValue = new ObjectTagGroup();

            _currentValue.AddTag(tag);
        }

        /// <summary>
        /// Extracts the actual tag out of a <see cref="DataTreeNode"/> for tag settings with reflection.
        /// </summary>
        /// <param name="data">The node to extract from.</param>
        /// <returns>Returns the represented <see cref="ObjectTag"/>.</returns>
        private ObjectTag ExtractNodeTag(DataTreeNode data)
        {
            ReflectionKit.GetFieldValue(data, ReflectionKit.DefaultFlags, out ObjectTag tag, "InternalTag");
            return tag;
        }
    }
}