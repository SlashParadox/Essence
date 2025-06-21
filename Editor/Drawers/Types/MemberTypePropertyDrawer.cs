// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A type of <see cref="GUIContent"/> in use by the <see cref="MemberTypePropertyDrawer"/>.
    /// </summary>
    public class MemberTypeGUIContent : GUIContent
    {
        /// <summary>The <see cref="System.Type"/> the option relates to.</summary>
        public readonly System.Type RegisteredType;

        public MemberTypeGUIContent(System.Type type, string label)
            : base(label)
        {
            RegisteredType = type;
        }
    }

    /// <summary>
    /// A <see cref="CustomPropertyDrawer"/> for <see cref="MemberType"/>s.
    /// </summary>
    [CustomPropertyDrawer(typeof(MemberType))]
    [CustomPropertyDrawer(typeof(MemberType<>))]
    public class MemberTypePropertyDrawer : EssencePropertyDrawer
    {
        private TypeGrouping _grouping;

        private MemberTypeFilterAttribute[] _filters;

        private MemberTypeGUIContent[] _filteredTypes;

        private SerializedProperty _typeNameProperty;

        private MemberType _memberType;

        private int _selectedIndex = Literals.InvalidIndex;
        
        /// <summary>
        /// Caches a list of possible <see cref="System.Type"/>s a <see cref="MemberType"/> value can be. Use this ahead of any editor field to prevent constant heavy processing.
        /// </summary>
        /// <param name="type">The current value of the <see cref="MemberType"/>.</param>
        /// <param name="typeIndex">The index of the <paramref name="type"/> in the options.</param>
        /// <param name="grouping">The grouping to use for the options.</param>
        /// <param name="filters">Filters to use for the options.</param>
        /// <returns>Returns an array of <see cref="MemberTypeGUIContent"/> options. Use these in an editor field function.</returns>
        public static MemberTypeGUIContent[] CacheFilteredTypeOptions(MemberType type, out int typeIndex, TypeGrouping grouping = TypeGrouping.None, IList<MemberTypeFilterAttribute> filters = null)
        {
            List<MemberTypeGUIContent> outContent = new List<MemberTypeGUIContent>();
            outContent.Add(new MemberTypeGUIContent(null, "None"));
            typeIndex = 0;

            foreach (Assembly referencedAssembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                GetFilteredTypesByAssembly(referencedAssembly, type, grouping, outContent, ref typeIndex, filters);
            }

            return outContent.ToArray();
        }

        /// <summary>
        /// Creates an <see cref="EditorGUILayout"/> field for a <see cref="MemberType"/>. Gather your options using <see cref="CacheFilteredTypeOptions"/>.
        /// </summary>
        /// <param name="label">The displayed label.</param>
        /// <param name="value">The current value of the <see cref="MemberType"/>.</param>
        /// <param name="index">The index of the <paramref name="value"/> in the <see cref="options"/>. Updated to match the new index of the returned value.</param>
        /// <param name="options">The available options for the final value.</param>
        /// <typeparam name="T">The base type of the <see cref="MemberType"/>.</typeparam>
        /// <returns>Returns the selected <see cref="MemberType"/>.</returns>
        public static MemberType<T> MemberTypeLayoutField<T>(GUIContent label, MemberType<T> value, ref int index, MemberTypeGUIContent[] options)
        {
            // ReSharper disable once CoVariantArrayConversion
            index = EditorGUILayout.Popup(label, index, options);

            System.Type finalType = options.IsValidIndex(index) ? options[index].RegisteredType : null;
            EditorGUILayout.LabelField(new GUIContent(finalType != null ? finalType.Name : "None"));

            MemberType<T> result = new MemberType<T>(finalType, value == null || value.CanBeBaseType);
            return result;
        }

        /// <summary>
        /// Creates an <see cref="EditorGUILayout"/> field for a <see cref="MemberType"/>. Gather your options using <see cref="CacheFilteredTypeOptions"/>.
        /// </summary>
        /// <param name="label">The displayed label.</param>
        /// <param name="value">The current value of the <see cref="MemberType"/>.</param>
        /// <param name="index">The index of the <paramref name="value"/> in the <see cref="options"/>. Updated to match the new index of the returned value.</param>
        /// <param name="options">The available options for the final value.</param>
        /// <typeparam name="T">The base type of the <see cref="MemberType"/>.</typeparam>
        /// <returns>Returns the selected <see cref="MemberType"/>.</returns>
        public static MemberType MemberTypeLayoutField(GUIContent label, MemberType value, ref int index, MemberTypeGUIContent[] options)
        {
            // ReSharper disable once CoVariantArrayConversion
            index = EditorGUILayout.Popup(label, index, options);

            System.Type finalType = options.IsValidIndex(index) ? options[index].RegisteredType : null;
            EditorGUILayout.LabelField(new GUIContent(finalType != null ? finalType.Name : "None"));

            MemberType result = new MemberType(finalType);
            return result;
        }
        
        private static void GetFilteredTypesByAssembly(Assembly inAssembly, MemberType memberType, TypeGrouping grouping, List<MemberTypeGUIContent> outContent, ref int index, IList<MemberTypeFilterAttribute> filters = null)
        {
            if (inAssembly == null)
                return;

            foreach (System.Type assemblyType in inAssembly.GetTypes())
            {
                if (!assemblyType.IsVisible)
                    continue;

                if (memberType != null && !memberType.CanTypeBeSet(assemblyType, out string _))
                    continue;

                bool passedFilters = true;
                if (filters.IsNotEmptyOrNull())
                    foreach (MemberTypeFilterAttribute filter in filters)
                    {
                        if (!filter.SatisfiesFilter(assemblyType))
                        {
                            passedFilters = false;
                            break;
                        }
                    }

                if (!passedFilters)
                    continue;

                BuildTypeMenuPath(assemblyType, grouping, outContent);

                if (memberType != null && assemblyType.AssemblyQualifiedName == memberType.ToString()) index = outContent.Count - 1;
            }
        }

        private static void BuildTypeMenuPath(System.Type type, TypeGrouping grouping, List<MemberTypeGUIContent> outContent)
        {
            string path = EditorKit.BuildTypeMenuPath(type, grouping);
            
            if (!string.IsNullOrEmpty(path))
                outContent.Add(new MemberTypeGUIContent(type, path));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.fixedHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            MemberTypeGroupAttribute groupAttribute = fieldInfo != null ? fieldInfo.GetCustomAttribute<MemberTypeGroupAttribute>() : null;
            _grouping = groupAttribute?.Grouping ?? TypeGrouping.None;
            _filters = fieldInfo != null ? fieldInfo.GetCustomAttributes<MemberTypeFilterAttribute>().ToArray() : null;
            _typeNameProperty = property.FindPropertyRelative("assemblyQualifiedName");
            _memberType = (MemberType)fieldInfo?.GetValue(property.serializedObject.targetObject);

            _filteredTypes = CacheFilteredTypeOptions(_memberType, out _selectedIndex, _grouping, _filters);

            base.OnDrawerInitialized(property, label);
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            // ReSharper disable once CoVariantArrayConversion
            _selectedIndex = EditorGUI.Popup(position, label, _selectedIndex, _filteredTypes);

            System.Type finalType = _filteredTypes.IsValidIndex(_selectedIndex) ? _filteredTypes[_selectedIndex].RegisteredType : null;
            _typeNameProperty.stringValue = finalType != null ? finalType.AssemblyQualifiedName : string.Empty;

            System.Type currentType = MemberType.AssemblyQualifiedStringToType(_typeNameProperty.stringValue);
            position.y += EditorGUIUtility.standardVerticalSpacing * 4.0f;
            EditorGUI.LabelField(position, new GUIContent(currentType != null ? currentType.Name : "None"));
        }
    }
}