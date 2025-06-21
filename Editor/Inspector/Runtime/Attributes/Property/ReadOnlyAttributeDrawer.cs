// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="ReadOnlyAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : EssencePropertyDrawer
    {
        /// <summary>The amount of time to wait before updating via the <see cref="_callbackInfo"/>.</summary>
        private const long CallbackScheduleMS = 1000;

        /// <summary>All created <see cref="VisualElement"/>s under this drawer that need to be updated.</summary>
        private readonly List<VisualElement> _createdElements = new List<VisualElement>();

        /// <summary>The target object of the drawer.</summary>
        private Object _targetObject;

        /// <summary>The <see cref="MethodInfo"/> related to the <see cref="ReadOnlyAttribute.CallbackMethod"/>.</summary>
        private MethodInfo _callbackInfo;

        ~ReadOnlyAttributeDrawer()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            _createdElements.Clear();
        }

        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            VisualElement propertyField = new PropertyField(property);
            _createdElements.Add(propertyField);

            ReadOnlyAttribute attr = attribute as ReadOnlyAttribute;

            // Only create bindings for the first element.
            if (attr != null && _createdElements.Count == 1)
            {
                _targetObject = property.serializedObject.targetObject;

                if (attr.PlayModeOnly)
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

                // Find a valid callback method.
                if (!string.IsNullOrEmpty(attr.CallbackMethod))
                {
                    _callbackInfo = ReflectionKit.GetMethodInfo(property.serializedObject.targetObject.GetType(), attr.CallbackMethod, ReflectionKit.DefaultFlags);
                    if (_callbackInfo != null)
                    {
                        // Validate the callback.
                        if (_callbackInfo.GetParameters().IsNotEmptyOrNull() || _callbackInfo.ReturnType != typeof(bool))
                        {
                            Debug.Log($"{nameof(ReadOnlyAttribute)} on [{property.serializedObject.targetObject.name}::{fieldInfo.Name}] has callback with parameters or non-bool return type. Cannot use callback [{attr.CallbackMethod}]!");
                            _callbackInfo = null;
                        }

                        if (_callbackInfo != null)
                            propertyField.schedule.Execute(EvaluateEnabledState).Every(CallbackScheduleMS);
                    }
                    else
                    {
                        Debug.Log($"{nameof(ReadOnlyAttribute)} on [{property.serializedObject.targetObject.name}::{fieldInfo.Name}] could not find callback [{attr.CallbackMethod}]!");
                    }
                }
            }

            EvaluateEnabledState();
            return propertyField;
        }

        /// <summary>
        /// An event called when the play state changes in editor.
        /// </summary>
        /// <param name="state">The new <see cref="PlayModeStateChange"/>.</param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            EvaluateEnabledState();
        }

        /// <summary>
        /// Evaluates the enabled state of the drawers.
        /// </summary>
        private void EvaluateEnabledState()
        {
            // Initialize isEnabled based on if there are modifiers. If there are none, the drawer will always be disabled.
            ReadOnlyAttribute attr = attribute as ReadOnlyAttribute;
            bool isEnabled = attr != null && attr.HasModifiers();

            // Go through the modifiers.
            if (isEnabled)
            {
                if (_callbackInfo != null && _targetObject)
                    isEnabled = (bool)_callbackInfo.Invoke(_targetObject, null);

                if (attr.PlayModeOnly)
                    isEnabled &= !EditorApplication.isPlayingOrWillChangePlaymode;
            }

            // Update all displayed elements.
            foreach (VisualElement element in _createdElements)
            {
                element?.SetEnabled(isEnabled);
            }
        }
    }
}
