// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A field for <see cref="HashMap{TKey,TValue}"/> properties.
    /// </summary>
    [VisualTreePath] [UxmlElement]
    public partial class HashMapField : VisualElement
    {
        /// <summary>The view for the <see cref="_listProperty"/>.</summary>
        private readonly MultiColumnListView _listView;

        /// <summary>A dictionary of <see cref="VisualElement"/>s displayed in the <see cref="ListView"/>, and their respective data index.</summary>
        private readonly Dictionary<VisualElement, int> _keyElements = new Dictionary<VisualElement, int>();

        /// <summary>A dictionary of all keys in the <see cref="_listProperty"/>, and the respective first index the key was found.</summary>
        private readonly Dictionary<object, int> _firstKeyInstances = new Dictionary<object, int>();

        /// <summary>The original property for the hash map.</summary>
        private SerializedProperty _mapProperty;

        /// <summary>The property for the internal <see cref="HashMap{TKey,TValue}.editorData"/>.</summary>
        private SerializedProperty _listProperty;

        /// <summary>See: <see cref="ShowValueColumn"/></summary>
        private bool _showValueColumn = true;

        /// <summary>See: <see cref="KeyCellTemplate"/></summary>
        private VisualTreeAsset _keyCellTemplate;

        /// <summary>The default template for key cells.</summary>
        private VisualTreeAsset _defaultKeyCellTemplate;

        /// <summary>If true, the value column is shown. Otherwise, only keys are shown. Useful if values are shown in different spots.</summary>
        [UxmlAttribute]
        public bool ShowValueColumn
        {
            get { return _showValueColumn; }
            set
            {
                _showValueColumn = value;
                UpdateDisplayedColumns();
            }
        }

        /// <summary>A cell template for the key cells. If not used, the default is used instead.</summary>
        [UxmlAttribute]
        public VisualTreeAsset KeyCellTemplate
        {
            get { return _keyCellTemplate; }
            set
            {
                SetKeyElementTemplate(value);
            }
        }

        public HashMapField()
        {
            VisualTreeAsset asset = EditorCache.LoadVisualTreeForType(GetType());
            if (asset)
                asset.CloneTree(this);

            _listView = this.Q<MultiColumnListView>("DataView");

            UpdateDisplayedColumns();

            if (_listView.columns.Contains("KeyCol"))
            {
                _defaultKeyCellTemplate = _listView.columns["KeyCol"].cellTemplate;
                _listView.columns["KeyCol"].bindCell = OnBindKeyCell;
                _listView.columns["KeyCol"].unbindCell = OnUnbindKeyCell;
            }

            if (_listView.columns.Contains("ValueCol"))
                _listView.columns["ValueCol"].bindCell = OnBindValueCell;

            _listView.itemIndexChanged += (_, _) => RefreshListView();
            _listView.itemsAdded += OnItemsAdded;
            _listView.itemsRemoved += _ => UpdateDuplicateWarnings(true);

            SetKeyElementTemplate(_keyCellTemplate);
        }

        public HashMapField(SerializedProperty mapProperty) : this()
        {
            SetSerializedProperty(mapProperty);
        }

        /// <summary>
        /// Sets the displayed <see cref="SerializedProperty"/> of a <see cref="HashMap{TKey,TValue}"/>.
        /// </summary>
        /// <param name="inMapProperty">The property to display. Must be a <see cref="HashMap{TKey,TValue}"/>.</param>
        public void SetSerializedProperty(SerializedProperty inMapProperty)
        {
            _mapProperty = inMapProperty;
            _listProperty = null;
            if (_mapProperty == null)
                return;

            _listProperty = _mapProperty.FindPropertyRelative("editorData");
            if (_listProperty == null)
                return;

            Foldout foldout = this.Q<Foldout>();
            if (foldout != null)
                foldout.text = $"{_mapProperty.displayName}";

            if (_listView != null)
            {
                _listView.BindProperty(_listProperty);
                _listView.Rebuild();
            }
        }

        /// <summary>
        /// Updates the visual element used for the key cell.
        /// </summary>
        /// <param name="inTree"></param>
        protected void SetKeyElementTemplate(VisualTreeAsset inTree)
        {
            if (_listView == null || !_listView.columns.Contains("KeyCol"))
                return;

            _keyCellTemplate = inTree ? inTree : _defaultKeyCellTemplate;

            _listView.columns["KeyCol"].cellTemplate = _keyCellTemplate;
            _listView.Rebuild();
        }

        /// <summary>
        /// Refreshes the <see cref="_listView"/>.
        /// </summary>
        private void RefreshListView()
        {
            _listView.RefreshItems();
            UpdateDuplicateWarnings(true);
        }

        /// <summary>
        /// An event called when items are added to the <see cref="_listView"/>.
        /// </summary>
        /// <param name="indices">The added indices.</param>
        private void OnItemsAdded(IEnumerable<int> indices)
        {
            if (_listProperty == null)
                return;

            // Initialize the properties at the indices. They'll start as null if they are reference types, which won't show up at all.
            foreach (int index in indices)
            {
                SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(index);
                if (itemProperty != null)
                {
                    InitializeNewItemProperty(itemProperty.FindPropertyRelative("key"), itemProperty.boxedValue.GetType(), 0);
                    InitializeNewItemProperty(itemProperty.FindPropertyRelative("value"), itemProperty.boxedValue.GetType(), 1);
                }
            }

            UpdateDuplicateWarnings(true);
        }

        /// <summary>
        /// Initializes a new value to the given property of a newly created array element.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to set.</param>
        /// <param name="parentType">The owning <see cref="Type"/> to get generic type arguments out of.</param>
        /// <param name="genericTypeIndex">The index of the generic <see cref="Type"/> to use for initialization.</param>
        private void InitializeNewItemProperty(SerializedProperty property, Type parentType, int genericTypeIndex)
        {
            if (parentType == null || !parentType.GenericTypeArguments.IsValidIndex(genericTypeIndex))
                return;

            // Initialize the value, if it is not already.
            Type valueType = parentType.GenericTypeArguments[genericTypeIndex];
            if (valueType == null || valueType.IsOrIsSubclassOf(typeof(UnityEngine.Object)) || !valueType.HasDefaultConstructor())
                return;

            property.boxedValue = Activator.CreateInstance(valueType);
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Updates the displayed columns on the <see cref="_listView"/>.
        /// </summary>
        private void UpdateDisplayedColumns()
        {
            if (_listView == null || !_listView.columns.Contains("ValueCol"))
                return;

            _listView.columns["ValueCol"].visible = _showValueColumn;
            _listView.columns.stretchMode = _showValueColumn ? Columns.StretchMode.Grow : Columns.StretchMode.GrowAndFill;
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

            _keyElements[element] = index;

            PropertyField propertyField = element.Q<PropertyField>("KeyField");
            if (propertyField == null)
                return;

            propertyField.UnregisterCallback<SerializedPropertyChangeEvent>(OnKeyChanged);
            propertyField.RegisterValueChangeCallback(OnKeyChanged);

            SerializedProperty itemProperty = _listProperty.GetArrayElementAtIndex(index);
            SerializedProperty keyProperty = itemProperty.FindPropertyRelative("key");
            propertyField.BindProperty(keyProperty);
            propertyField.label = "";

            UpdateDuplicateWarnings(false);
        }

        /// <summary>
        /// An event called when a <see cref="VisualElement"/> is unhooked from the <see cref="_listView"/>.
        /// </summary>
        /// <param name="element">The removed/refreshed <see cref="VisualElement"/>.</param>
        /// <param name="index">The index of the <paramref name="element"/>'s data.</param>
        private void OnUnbindKeyCell(VisualElement element, int index)
        {
            PropertyField propertyField = element.Q<PropertyField>("KeyField");
            if (propertyField == null)
                return;

            _keyElements.Remove(element);

            propertyField.Unbind();
            propertyField.ClearBindings();
            propertyField.UnregisterCallback<SerializedPropertyChangeEvent>(OnKeyChanged);
            UpdateDuplicateWarnings(false);
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

        /// <summary>
        /// An event called when a key in the map changes.
        /// </summary>
        /// <param name="evt">The property event.</param>
        private void OnKeyChanged(SerializedPropertyChangeEvent evt)
        {
            UpdateDuplicateWarnings(true);
        }

        /// <summary>
        /// Refreshes the list of first key instances found.
        /// </summary>
        private void RefreshKeyInstances()
        {
            if (_listProperty == null)
                return;

            _firstKeyInstances.Clear();
            _firstKeyInstances.EnsureCapacity(_listProperty.arraySize);
            for (int i = 0; i < _listProperty.arraySize; ++i)
            {
                SerializedProperty property = _listProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProperty = property?.FindPropertyRelative("key");
                if (keyProperty == null)
                    continue;

                _firstKeyInstances.TryAdd(keyProperty.boxedValue, i);
            }
        }

        /// <summary>
        /// Updates the display of duplicate key warnings.
        /// </summary>
        /// <param name="refreshKeyInstances">If true, the <see cref="_firstKeyInstances"/> are refreshed.</param>
        private void UpdateDuplicateWarnings(bool refreshKeyInstances)
        {
            if (_listProperty == null)
                return;

            if (refreshKeyInstances)
                RefreshKeyInstances();

            foreach (KeyValuePair<VisualElement, int> pair in _keyElements)
            {
                if (pair.Key == null)
                    continue;

                PropertyField propertyField = pair.Key.Q<PropertyField>("KeyField");
                if (propertyField == null)
                    continue;

                SerializedProperty keyProperty = _mapProperty.serializedObject.FindProperty(propertyField.bindingPath);
                if (keyProperty == null)
                    continue;

                if (!_firstKeyInstances.TryGetValue(keyProperty.boxedValue, out int index))
                    continue;

                HelpBox warningField = pair.Key.Q<HelpBox>("DuplicateWarning");
                if (index != pair.Value)
                {
                    pair.Key.style.backgroundColor = new StyleColor(Color.darkRed);

                    if (warningField != null)
                        warningField.visible = true;
                }
                else
                {
                    pair.Key.style.backgroundColor = new StyleColor();

                    if (warningField != null)
                        warningField.visible = false;
                }
            }
        }
    }
}
