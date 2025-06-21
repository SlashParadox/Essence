// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(HashMap<,>))]
    public class HashMapDrawer : EssencePropertyDrawer
    {
        /// <summary>Keys that have already been added to the <see cref="HashMap{TKey,TValue}"/>.</summary>
        private readonly List<object> _addedKeys = new List<object>();

        /// <summary>A <see cref="VisualElement"/> for the map's keys.</summary>
        [SerializeField] private VisualTreeAsset _keyTreeAsset;

        private SerializedProperty currentProperty;

        /// <summary>The property for the internal <see cref="HashMap{TKey,TValue}.editorData"/>.</summary>
        private SerializedProperty _listProperty;

        /// <summary>The view for the <see cref="_listProperty"/>.</summary>
        private MultiColumnListView _listView;

        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            return new HashMapField(property);
        }

        /// <summary>
        /// Refreshes the displayed list of elements.
        /// </summary>
        protected void RefreshListView()
        {
            _addedKeys.Clear();
            _listView.RefreshItems();
        }

        /// <summary>
        /// Creates a key cell <see cref="VisualElement"/>.
        /// </summary>
        /// <returns>Returns the created <see cref="VisualElement"/>.</returns>
        private VisualElement OnMakeKeyCell()
        {
            if (!_keyTreeAsset)
                return null;

            VisualElement element = new VisualElement();
            _keyTreeAsset.CloneTree(element);
            return element;
        }

        /// <summary>
        /// An event called to bind a <see cref="VisualElement"/> to a key in the map.
        /// </summary>
        /// <param name="element">The element to bind.</param>
        /// <param name="index">The index of the data to bind to.</param>
        private void OnBindKeyCell(VisualElement element, int index)
        {
            if (_listProperty.arraySize <= index)
                return;

            PropertyField propertyField = element.Q<PropertyField>("KeyField");
            HelpBox warningField = element.Q<HelpBox>("DuplicateWarning");
            if (propertyField == null)
                return;

            SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(index);
            SerializedProperty keyProperty = itemProperty.FindPropertyRelative("key");
            propertyField.BindProperty(keyProperty);
            propertyField.label = string.Empty;

            // If a key is already in the map, warn the user instead of blocking them.
            object value = keyProperty.boxedValue;
            int foundIndex = _addedKeys.IndexOf(value);
            if (foundIndex >= 0 && foundIndex != index)
            {
                element.style.backgroundColor = new StyleColor(Color.darkRed);

                if (warningField != null)
                    warningField.visible = true;
            }
            else
            {
                _addedKeys.Add(value);
                element.style.backgroundColor = new StyleColor();

                if (warningField != null)
                    warningField.visible = false;
            }
        }

        /// <summary>
        /// An event called to bind a <see cref="VisualElement"/> to a value in the map.
        /// </summary>
        /// <param name="element">The element to bind.</param>
        /// <param name="index">The index of the data to bind to.</param>
        private void OnBindValueCell(VisualElement element, int index)
        {
            if (_listProperty.arraySize <= index)
                return;

            if (element is not PropertyField field)
                return;

            SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(index);

            field.BindProperty(itemProperty.FindPropertyRelative("value"));
            field.label = string.Empty;
        }

        private void OnItemsAdded(IEnumerable<int> indices)
        {
            if (_listProperty == null)
                return;

            foreach (int index in indices)
            {
                SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(index);
                SerializedProperty valueProperty = itemProperty.FindPropertyRelative("value");

                // Initialize the value, if it is not already.
                Type valueType = itemProperty.boxedValue.GetType().GenericTypeArguments[1];
                valueProperty.boxedValue = Activator.CreateInstance(valueType);
                valueProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
