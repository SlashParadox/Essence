// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(MemberType))]
    [CustomPropertyDrawer(typeof(MemberType<>))]
    public class MemberTypePropertyDrawer : EssencePropertyDrawer
    {
        private class MemberTypeGUIContent : GUIContent
        {
            public readonly System.Type registeredType;

            public MemberTypeGUIContent(System.Type type, string label)
                : base(label)
            {
                registeredType = type;
            }
        }

        private static readonly char TypeNameSeparator = '.';

        private static readonly char MenuPathSeparator = '/';

        private static readonly string ClassGroupingName = "Class/";

        private static readonly string StructGroupingName = "Struct/";

        private static readonly string InterfaceGroupingName = "Interface/";
        
        private static readonly string EnumGroupingName = "Enum/";

        private MemberTypeGroup _grouping;

        private MemberTypeFilterAttribute[] _filters;

        private MemberTypeGUIContent[] filteredTypes;

        private GenericMenu dropdownMenu = new GenericMenu();

        private SerializedProperty _typeNameProperty;

        private MemberType _memberType;

        private int selectedIndex = Literals.InvalidIndex;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.fixedHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            MemberTypeGroupAttribute groupAttribute = fieldInfo != null ? fieldInfo.GetCustomAttribute<MemberTypeGroupAttribute>() : null;
            _grouping = groupAttribute?.Grouping ?? MemberTypeGroup.None;
            _filters = fieldInfo != null ? fieldInfo.GetCustomAttributes<MemberTypeFilterAttribute>().ToArray() : null;
            _typeNameProperty = property.FindPropertyRelative("assemblyQualifiedName");
            _memberType = (MemberType)fieldInfo?.GetValue(property.serializedObject.targetObject);

            GetFilteredTypes();

            base.OnDrawerInitialized(property, label);
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label, PropertyDrawerData data)
        {
            // ReSharper disable once CoVariantArrayConversion
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, filteredTypes);

            System.Type finalType = filteredTypes.IsValidIndex(selectedIndex) ? filteredTypes[selectedIndex].registeredType : null;
            _typeNameProperty.stringValue = finalType != null ? finalType.AssemblyQualifiedName : string.Empty;

            System.Type currentType = MemberType.AssemblyQualifiedStringToType(_typeNameProperty.stringValue);
            position.y += EditorGUIUtility.standardVerticalSpacing * 4.0f;
            EditorGUI.LabelField(position, new GUIContent(currentType != null ? currentType.Name : "None"));
        }

        private void GetFilteredTypes()
        {
            List<MemberTypeGUIContent> outContent = new List<MemberTypeGUIContent>();
            outContent.Add(new MemberTypeGUIContent(null, "None"));
            selectedIndex = 0;

            foreach (Assembly referencedAssembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                GetFilteredTypesByAssembly(referencedAssembly, outContent);
            }

            filteredTypes = outContent.ToArray();
        }

        private void GetFilteredTypesByAssembly(Assembly inAssembly, List<MemberTypeGUIContent> outContent)
        {
            if (inAssembly == null)
                return;

            foreach (System.Type assemblyType in inAssembly.GetTypes())
            {
                if (!assemblyType.IsVisible)
                    continue;
                
                if (_memberType != null && !_memberType.CanTypeBeSet(assemblyType, out string _))
                    continue;

                bool passedFilters = true;
                if (_filters.IsNotEmptyOrNull())
                {
                    foreach (MemberTypeFilterAttribute filter in _filters)
                    {
                        if (!filter.SatisfiesFilter(assemblyType))
                        {
                            passedFilters = false;
                            break;
                        }
                    }
                }

                if (!passedFilters)
                    continue;

                BuildTypeMenuPath(assemblyType, outContent);

                if (assemblyType.AssemblyQualifiedName == _typeNameProperty.stringValue)
                {
                    selectedIndex = outContent.Count - 1;
                }
            }
        }

        private void BuildTypeMenuPath(System.Type type, List<MemberTypeGUIContent> outContent)
        {
            if (type == null)
                return;

            switch (_grouping)
            {
                case MemberTypeGroup.ByInheritance:
                {
                    if (type.IsValueType || type.IsInterface)
                    {
                        outContent.Add(new MemberTypeGUIContent(type, type.FullName));
                    }
                    else if (type.IsClass)
                    {
                        string currentPath = type.FullName;
                        System.Type currentType = type;
                        while (currentType != null && currentType.IsClass)
                        {
                            currentPath = $"{currentType.FullName}/{currentPath}";
                            currentType = currentType.BaseType;
                        }

                        outContent.Add(new MemberTypeGUIContent(type, currentPath));
                    }

                    break;
                }
                case MemberTypeGroup.ByNamespace:
                {
                    outContent.Add(new MemberTypeGUIContent(type, type.FullName?.Replace(TypeNameSeparator, MenuPathSeparator) ?? string.Empty));
                    break;
                }
                case MemberTypeGroup.ByIdentity:
                {
                    string usedPrefix;
                    if (type.IsClass)
                        usedPrefix = ClassGroupingName;
                    else if (type.IsInterface)
                        usedPrefix = InterfaceGroupingName;
                    else if (type.IsEnum)
                        usedPrefix = EnumGroupingName;
                    else
                        usedPrefix = StructGroupingName;
                    
                    outContent.Add(new MemberTypeGUIContent(type, $"{usedPrefix}{type.FullName}"));
                    break;
                }
                default:
                {
                    outContent.Add(new MemberTypeGUIContent(type, type.FullName));
                    break;
                }
            }
        }
    }
}