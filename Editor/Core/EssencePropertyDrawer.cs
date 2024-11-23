// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    public class PropertyDrawerData
    {
        
    }
    
    
    /// <summary>
    /// An enhanced version of a <see cref="PropertyDrawer"/>.
    /// </summary>
    public abstract class EssencePropertyDrawer : PropertyDrawer
    {
        protected static readonly string EssenceIconFolder = "Packages/com.slashparadox.essence/Editor/Assets/Icon/";
        
        /// <summary>If true, the drawer has been fully initialized.</summary>
        protected bool IsInitialized { get; private set; }

        private bool? _isValidDrawer;

        /// <summary>The default <see cref="PropertyDrawer"/>, from the <see cref="EditorCache"/>.</summary>
        protected PropertyDrawer DefaultDrawer { get; private set; }

        private SerializedObject _serializedObject;

        private Dictionary<string, PropertyDrawerData> _propertyDatas = new Dictionary<string, PropertyDrawerData>();

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width <= 1)
                return;

            _isValidDrawer ??= IsDrawerValid(property);
            if (_isValidDrawer == false)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!IsInitialized || EditorKit.IsSerializedObjectDisposed(_serializedObject))
                InitializeDrawer(property, label);

            PropertyDrawerData data = GetOrCreatePropertyData(property, label);

            OnGUIDraw(position, property, label, data);
        }

        /// <summary>
        /// Initializes the <see cref="EssencePropertyDrawer"/>. Only ever called once.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> being drawn.</param>
        /// <param name="label">The <see cref="GUIContent"/> being drawn.</param>
        private void InitializeDrawer(SerializedProperty property, GUIContent label)
        {
            _serializedObject = property.serializedObject;
            DefaultDrawer = EditorCache.GetPropertyDrawer(property);
            _propertyDatas.Clear();
            OnDrawerInitialized(property, label);
            IsInitialized = true;
        }

        protected T FindPropertyData<T>(SerializedProperty property) where T : PropertyDrawerData
        {
            _propertyDatas.TryGetValue(property.propertyPath, out PropertyDrawerData propertyData);
            return propertyData as T;
        }

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
        protected virtual void OnDrawerInitialized(SerializedProperty property, GUIContent label) { }

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
    }
}
#endif