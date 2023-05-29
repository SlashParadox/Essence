// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for any <see cref="IList"/>.
    /// </summary>
    [CustomPropertyItem(typeof(IList), true)]
    public class ListPropertyDrawer : PropertyValueItem<IList>
    {
        /// <summary>If true, the items are draggable to reorder.</summary>
        public bool IsDraggable = true;

        /// <summary>If true, the list header is shown.</summary>
        public bool ShowHeader = true;

        /// <summary>If true, the add button is shown.</summary>
        public bool ShowAdd = true;

        /// <summary>If true, the remove button is shown.</summary>
        public bool ShowRemove = true;

        /// <summary>The created <see cref="ReorderableList"/>.</summary>
        private ReorderableList _reorderableList;

        /// <summary>The inner <see cref="Type"/> of the property.</summary>
        private Type _elementType;

        /// <summary>A cache of the different heights of the items.</summary>
        private List<float> _cachedHeights = new List<float>();

        public ListPropertyDrawer() { }

        public ListPropertyDrawer(EditorValue<IList> value, GUIContent label)
            : base(value, label) { }

        public ListPropertyDrawer(EditorValue<IList> value, PropertyLabel label)
            : base(value, label) { }

        public ListPropertyDrawer(EditorValue<IList> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        public override float GetHeight()
        {
            return _reorderableList?.GetHeight() ?? 0.0f;
        }

        public override bool ApplyGenericEditorValue(EditorValue<object> genericValue)
        {
            if (genericValue == null || !genericValue.GetVariableType().IsSubclassOrImplements(typeof(IList)))
                return false;

            Value = new EditorValue<IList>(genericValue);
            return true;
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value?.GetCurrentValue() == null)
                return null;

            ConditionalResetCache();

            _reorderableList?.DoList(drawRect);
            Value.SProperty?.serializedObject.ApplyModifiedProperties();

            return Value.GetCurrentValue();
        }

        /// <summary>
        /// Creates the <see cref="ReorderableList"/> and initializes it.
        /// </summary>
        /// <param name="bForce">If true, the list recreates even if it already exists.</param>
        protected void CreateReorderableList(bool bForce)
        {
            if (Value == null)
                return;

            if (_reorderableList != null && !bForce)
                return;

            bool isGUIEnabled = GUI.enabled;
            bool canShowHeader = ShowHeader && Label != null && Label.ConditionalLabel.IsValidGUIContent();
            bool canShowAdd = ShowAdd && isGUIEnabled;
            bool canShowRemove = ShowRemove && isGUIEnabled;

            if (Value.SProperty != null)
            {
                _reorderableList = new ReorderableList(Value.SProperty.serializedObject, Value.SProperty, IsDraggable, canShowHeader, canShowAdd, canShowRemove);
                _reorderableList.drawElementCallback = OnDrawSerializedPropertyElement;
            }
            else
            {
                _reorderableList = new ReorderableList(Value.GetCurrentValue(), Value.GetVariableType(), IsDraggable, canShowHeader, canShowAdd, canShowRemove);
                _reorderableList.drawElementCallback = OnDrawObjectElement;
            }
            
            _reorderableList.drawHeaderCallback = OnDrawHeader;
            _reorderableList.elementHeightCallback = GetElementHeight;
            _reorderableList.onChangedCallback = OnListChanged;
        }
        
        /// <summary>
        /// Draws the header for the <see cref="_reorderableList"/>.
        /// </summary>
        /// <param name="rect">The rect to draw the label in.</param>
        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, Label?.ConditionalLabel);
        }
        
        /// <summary>
        /// Draws a <see cref="SerializedProperty"/> element.
        /// </summary>
        /// <param name="rect">The rect to draw in.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="isActive">If true, the item is active.</param>
        /// <param name="isFocused">If true, the item is focused.</param>
        protected void OnDrawSerializedPropertyElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = Value.SProperty.GetArrayElementAtIndex(index);
            GUIContent label = new GUIContent($"{property.name} {index}", property.tooltip);

            EditorGUI.PropertyField(rect, property, label, true);
            CacheHeight(index, EditorGUI.GetPropertyHeight(property, label));
        }

        /// <summary>
        /// Draws a regular object element. Used if there is no <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="rect">The rect to draw in.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="isActive">If true, the item is active.</param>
        /// <param name="isFocused">If true, the item is focused.</param>
        protected void OnDrawObjectElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            // Get a fresh property item for the element type.
            PropertyItem item = EditorCache.GetPropertyItem<PropertyItem>(_elementType);
            IList value = Value?.GetCurrentValue();
            if (item == null || value == null || !value.IsValidIndexNG(index))
                return;

            // Apply the data.
            EditorValue<object> valueObj = new EditorValue<object>(true, value[index]);
            item.ApplyGenericEditorValue(valueObj);
            item.Label = new NormalPropertyLabel(new GUIContent(index.ToString()));

            item.Draw(ref rect);
            CacheHeight(index, item.GetHeight());

            value[index] = valueObj.LastResult;
            Value.SetLatestResult(value);
        }

        /// <summary>
        /// Gets the height of an element at a given index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>Returns the element's height.</returns>
        protected float GetElementHeight(int index)
        {
            return _cachedHeights.IsValidIndex(index) ? _cachedHeights[index] : EditorGUIUtility.singleLineHeight;
        }
        
        /// <summary>
        /// A callback when the <see cref="_reorderableList"/> changes in any way.
        /// </summary>
        /// <param name="list">The list that changed.</param>
        protected void OnListChanged(ReorderableList list)
        {
            if (_cachedHeights == null || list == null)
                return;

            if (_cachedHeights.Count == list.count)
                return;

            ResetCachedData(false);
        }
        
        /// <summary>
        /// Conditionally resets the cache if necessary.
        /// </summary>
        private void ConditionalResetCache()
        {
            Type listType = Value?.GetVariableType();
            if (listType == null)
                return;

            Type innerType = typeof(object);

            if (listType.GenericTypeArguments.Length > 0)
                innerType = listType.GenericTypeArguments[0];

            // If the types do not match, reset the cached data.
            if (_elementType == innerType)
                return;

            _elementType = innerType;
            ResetCachedData(true);
        }

        /// <summary>
        /// Caches a property height of at a given index.
        /// </summary>
        /// <param name="index">The index of the property.</param>
        /// <param name="height">The height of the property.</param>
        private void CacheHeight(int index, float height)
        {
            // Fill up to the index.
            if (!_cachedHeights.IsValidIndex(index))
            {
                for (int i = _cachedHeights.Count; i <= index; ++i)
                {
                    _cachedHeights.Add(0);
                }
            }

            // Set the new height.
            _cachedHeights[index] = height;
        }
        
        /// <summary>
        /// Resets the cached data, with the option to recreate the list.
        /// </summary>
        /// <param name="recreateList">If true, recreates the list, even if it exists.</param>
        private void ResetCachedData(bool recreateList)
        {
            _cachedHeights = new List<float>();
            
            CreateReorderableList(recreateList);
        }
    }
}
#endif