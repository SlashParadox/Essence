// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor.Inspector.Runtime
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for the <see cref="InstancedAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(InstancedAttribute))]
    public class InstancedAttributePropertyDrawer : EssencePropertyDrawer
    {
        /// <summary>The available <see cref="System.Type"/>s for the property to be.</summary>
        private readonly List<System.Type> _validTypes = new List<System.Type>();

        /// <summary>The labels for the <see cref="_validTypes"/>.</summary>
        private List<string> _typeLabels;

        /// <summary>The base <see cref="System.Type"/> of the property.</summary>
        private System.Type _baseType;

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            base.OnDrawerInitialized(property, label);
            InitializeValidTypes(property);
        }

        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            VisualElement root = base.OnCreatePropertyGUI(property, data);
            if (root == null)
                return null;

            DropdownField dropdown = root.Q<DropdownField>();
            dropdown.choices = _typeLabels;
            dropdown.label = property.displayName;
            dropdown.tooltip = property.tooltip;

            PropertyField propertyField = root.Q<PropertyField>();
            propertyField.BindProperty(property);

            // If no type has actually been set to the property, make a new one immediately.
            int index = Literals.InvalidIndex;
            if (!string.IsNullOrEmpty(property.type))
            {
                for (int i = 0; i < _validTypes.Count; ++i)
                {
                    System.Type type = _validTypes[i];

                    // The managed reference typename is formatted "{AssemblyName} {FullTypeName}", so we just need to check from the end.
                    if (!string.IsNullOrEmpty(type?.FullName) && property.managedReferenceFullTypename.EndsWith(type.FullName))
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index < 0)
            {
                dropdown.index = 0;
                OnCreateNewInstance(property, dropdown);
            }
            else
            {
                dropdown.index = index;
            }

            dropdown.RegisterValueChangedCallback((evt => OnCreateNewInstance(property, dropdown)));
            return root;
        }

        private void OnCreateNewInstance(SerializedProperty property, DropdownField dropdown)
        {
            if (property == null || dropdown == null)
                return;

            int index = System.Math.Max(0, dropdown.index);
            if (!_validTypes.IsValidIndex(index))
                return;

            dropdown.index = index;
            property.managedReferenceValue = System.Activator.CreateInstance(_validTypes[index]);
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Initializes the <see cref="_validTypes"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to base the types on.</param>
        private void InitializeValidTypes(SerializedProperty property)
        {
            InstancedAttribute currentAttribute = attribute as InstancedAttribute;
            if (currentAttribute == null || property == null)
                return;

            // Figure out the base type from the value the attribute is on.
            _baseType = ReflectionKit.FindManagedReferenceType(property.managedReferenceFieldTypename);
            if (_baseType == null)
                return;

            // Add the base type as an option, if allowed.
            if (IsValidType(_baseType))
                _validTypes.Add(_baseType);

            // Iterate through all derived types and add valid options.
            TypeCache.TypeCollection typeCollection = TypeCache.GetTypesDerivedFrom(_baseType);
            foreach (System.Type type in typeCollection)
            {
                if (IsValidType(type))
                    _validTypes.Add(type);
            }

            SortKit.QuickSort(_validTypes, (a, b) => string.CompareOrdinal(a.FullName, b.FullName));

            _typeLabels = new List<string>(_validTypes.Count);
            for (int i = 0; i < _validTypes.Count; ++i)
            {
                System.Type type = _validTypes[i];
                if (type == null)
                    continue;

                _typeLabels.Add(EditorKit.BuildTypeMenuPath(type, currentAttribute.Grouping));
            }
        }

        /// <summary>
        /// Checks if the given <see cref="System.Type"/> is a valid option for the instanced variable.
        /// </summary>
        /// <param name="inType">The <see cref="System.Type"/> to test.</param>
        /// <returns>Returns if the <paramref name="inType"/> is valid to use.</returns>
        private bool IsValidType(System.Type inType)
        {
            // The type must follow certain properties. Interfaces and abstract types cannot be instanced, Unity objects can't be instanced, and we must be able to construct it.
            return inType != null && !(inType.IsInterface
                                       || inType.IsAbstract
                                       || !_baseType.IsAssignableFrom(inType)
                                       || inType.IsSubclassOf(typeof(Object))
                                       || (inType.GetConstructor(System.Type.EmptyTypes) == null && !inType.IsValueType));
        }
    }
}
