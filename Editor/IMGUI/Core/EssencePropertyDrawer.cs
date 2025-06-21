// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A base class for data related to an <see cref="EssencePropertyDrawer"/>. Due to how Unity handles property drawers in a list,
    /// only one drawer is used, meaning instanced variables cannot be cached. If you have data to cache per item, you can store them in
    /// data.
    /// </summary>
    public class PropertyDrawerData { }

    /// <summary>
    /// An enhanced version of a <see cref="PropertyDrawer"/>.
    /// </summary>
    public abstract class EssencePropertyDrawer : PropertyDrawer
    {
        /// <summary>A default <see cref="VisualTreeAsset"/> to use. If set, this is used to create a default property GUI.</summary>
        [SerializeField] protected VisualTreeAsset defaultVisualTree;

        /// <summary>A path to icons for the editor.</summary>
        protected static readonly string EssenceIconFolder = "Packages/com.slashparadox.essence/Editor/Assets/Icon/";

        /// <summary>A map of <see cref="PropertyDrawerData"/> to the property paths they belong to.</summary>
        private readonly Dictionary<string, PropertyDrawerData> _propertyDatas = new Dictionary<string, PropertyDrawerData>();

        /// <summary>If true, the drawer has been fully initialized.</summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>The default <see cref="PropertyDrawer"/>, from the <see cref="EditorCache"/>.</summary>
        protected PropertyDrawer DefaultDrawer { get; private set; }

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width <= 1)
                return;

            bool isValidDrawer = IsDrawerValid(property);
            if (isValidDrawer == false)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!IsInitialized || EditorKit.IsSerializedObjectDisposed(property.serializedObject))
                InitializeDrawer(property, label);

            PropertyDrawerData data = GetOrCreatePropertyData(property, label);

            OnGUIDraw(position, property, label, data);
        }

        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            bool isValidDrawer = IsDrawerValid(property);
            if (isValidDrawer == false)
            {
                return base.CreatePropertyGUI(property);
            }

            if (!IsInitialized || EditorKit.IsSerializedObjectDisposed(property.serializedObject))
                InitializeDrawer(property, null);

            PropertyDrawerData data = GetOrCreatePropertyData(property, new GUIContent(property.displayName));

            return OnCreatePropertyGUI(property, data);
        }

        /// <summary>
        /// Appends a <see cref="VisualTreeAsset"/> to the given <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="tree">The <see cref="VisualElement"/> to add to. If not set, a new element will be created.</param>
        /// <param name="treeAsset">The <see cref="VisualTreeAsset"/> to use.</param>
        /// <returns>Returns the created <see cref="VisualElement"/> for the <paramref name="treeAsset"/>.</returns>
        protected VisualElement AppendTreeAsset(ref VisualElement tree, VisualTreeAsset treeAsset)
        {
            if (!treeAsset)
                return null;

            tree ??= new VisualElement();

            VisualElement newBranch = new VisualElement().SetName(treeAsset.name);
            treeAsset.CloneTree(newBranch);
            tree.Add(newBranch);

            return newBranch;
        }

        /// <summary>
        /// Finds the <see cref="PropertyDrawerData"/> to use for the given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check.</param>
        /// <typeparam name="T">The parent type of the <see cref="PropertyDrawerData"/>.</typeparam>
        /// <returns>Returns the found <see cref="PropertyDrawerData"/>.</returns>
        protected T FindPropertyData<T>(SerializedProperty property) where T : PropertyDrawerData
        {
            _propertyDatas.TryGetValue(property.propertyPath, out PropertyDrawerData propertyData);
            return propertyData as T;
        }

        /// <summary>
        /// Removes <see cref="PropertyDrawerData"/> for the given <see cref="SerializedProperty"/>.
        /// Use this if your property needs to be completely refreshed. Especially true for changing managed references.
        /// </summary>
        /// <param name="property"></param>
        protected void RemovePropertyData(SerializedProperty property)
        {
            if (property != null && !string.IsNullOrEmpty(property.propertyPath))
                _propertyDatas.Remove(property.propertyPath);
        }

        /// <summary>
        /// Initializes the <see cref="EssencePropertyDrawer"/>. Only ever called once.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        private void InitializeDrawer(SerializedProperty property, GUIContent label)
        {
            DefaultDrawer = EditorCache.GetPropertyDrawer(property);
            _propertyDatas.Clear();
            OnDrawerInitialized(property, label);
            IsInitialized = true;
        }

        /// <summary>
        /// Finds <see cref="PropertyDrawerData"/> for a given <see cref="SerializedProperty"/>, or creates it if necessary.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to get or make data for.</param>
        /// <param name="label">The <paramref name="property"/>'s label.</param>
        /// <returns>Returns the found <see cref="PropertyDrawerData"/>.</returns>
        private PropertyDrawerData GetOrCreatePropertyData(SerializedProperty property, GUIContent label)
        {
            if (property == null)
                return null;

            if (_propertyDatas.TryGetValue(property.propertyPath, out PropertyDrawerData propertyData))
                return propertyData;

            PropertyDrawerData data = CreatePropertyData(property, label) ?? new PropertyDrawerData();

            _propertyDatas.Add(property.propertyPath, data);

            return data;
        }

        /// <summary>
        /// Checks if this drawer's attribute is valid for the attached property.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check.</param>
        /// <returns>If false is returned, the default editor GUI field is drawn.</returns>
        protected virtual bool IsDrawerValid(SerializedProperty property)
        {
            return true;
        }

        /// <summary>
        /// Initializes the <see cref="EssencePropertyDrawer"/>. Only ever called once.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        /// <remarks>This is called once even with lists. Use this to cache information all items could potentially use.</remarks>
        protected virtual void OnDrawerInitialized(SerializedProperty property, GUIContent label) { }

        /// <summary>
        /// Creates <see cref="PropertyDrawerData"/> for the given property, if necessary.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to make data for.</param>
        /// <param name="label">The <paramref name="property"/>'s label.</param>
        /// <returns>Returns the created <see cref="PropertyDrawerData"/>, if any.</returns>
        protected virtual PropertyDrawerData CreatePropertyData(SerializedProperty property, GUIContent label)
        {
            return null;
        }

        /// <summary>
        /// Draws the custom IMGUI for a <see cref="EssencePropertyDrawer"/>. <see cref="OnDrawerInitialized"/> is
        /// guaranteed to have been called.
        /// </summary>
        /// <param name="position">The <see cref="Rect"/> used to position the drawer.</param>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        protected virtual void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            base.OnGUI(position, property, label);
        }

        protected virtual VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            VisualElement root = base.CreatePropertyGUI(property);

            if (defaultVisualTree)
                AppendTreeAsset(ref root, defaultVisualTree);

            return root;
        }
    }
}
