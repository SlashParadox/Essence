// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A helper class for caching off information about editor properties.
    /// </summary>
    public static class EditorCache
    {
        /// <summary>The <see cref="Type"/> of a <see cref="CustomPropertyDrawer"/>.</summary>
        private static readonly Type TypeCPD = typeof(CustomPropertyDrawer);

        /// <summary>The <see cref="Type"/> of a <see cref="CustomPropertyDrawer"/>.</summary>
        private static readonly Type TypeCPVI = typeof(CustomPropertyItemAttribute);

        /// <summary>The info of a <see cref="CustomPropertyDrawer"/>'s type field.</summary>
        private static readonly FieldInfo FieldCPDType;

        /// <summary>The info of a <see cref="CustomPropertyItemAttribute"/>'s type field.</summary>
        private static readonly FieldInfo FieldCPVIType;

        /// <summary>The info of a <see cref="CustomPropertyDrawer"/>'s child usage field.</summary>
        private static readonly FieldInfo FieldCPDChild;

        /// <summary>A cache of <see cref="Type"/>s and their <see cref="CustomPropertyDrawer"/>.</summary>
        private static readonly Dictionary<Type, Type> DrawerCache = new Dictionary<Type, Type>();

        /// <summary>A cache of <see cref="Type"/>s and their <see cref="PropertyItem"/>.</summary>
        private static readonly Dictionary<Type, Type> ValueItemCache = new Dictionary<Type, Type>();

        /// <summary>
        /// A static constructor for the <see cref="EditorCache"/>.
        /// </summary>
        static EditorCache()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldCPDType = TypeCPD.GetField("m_Type", flags);
            FieldCPDChild = TypeCPD.GetField("m_UseForChildren", flags);

#if ES_PRELOAD_GUIDRAWERS
            PreloadGUIDrawers();
            PreloadCustomPropertyValueItems();
#endif
        }

        /// <summary>
        /// Resets the <see cref="DrawerCache"/>. Use if a bad drawer is stored.
        /// </summary>
        [MenuItem("Essence/Reset GUI Drawer Cache")]
        public static void ResetGUIDrawerCache()
        {
            DrawerCache.Clear();

#if ES_PRELOAD_GUIDRAWERS
            PreloadGUIDrawers();
#endif
        }

        /// <summary>
        /// Resets the <see cref="ValueItemCache"/>. Use if a bad item is stored.
        /// </summary>
        [MenuItem("Essence/Reset Property Item Cache")]
        public static void ResetPropertyItemCache()
        {
            ValueItemCache.Clear();

#if ES_PRELOAD_GUIDRAWERS
            PreloadCustomPropertyValueItems();
#endif
        }

        /// <summary>
        /// Preloads all <see cref="GUIDrawer"/>s into the cache.
        /// </summary>
        public static void PreloadGUIDrawers()
        {
            foreach (Type drawerType in TypeCache.GetTypesDerivedFrom<GUIDrawer>())
            {
                // Get all attributes for a custom property drawer.
                object[] attributes = drawerType.GetCustomAttributes(TypeCPD, true);
                int count = attributes.Length;

                for (int i = 0; i < count; ++i)
                {
                    // Get the hidden, reflected fields.
                    object currentAtr = attributes[i];
                    Type currentDrawerType = FieldCPDType.GetValue(currentAtr) as Type;
                    bool forChildren = (bool)FieldCPDChild.GetValue(currentAtr);
                    
                    if (!currentDrawerType.IsOrIsSubclassOf(drawerType))
                        continue;

                    // Ensure the type is added.
                    if (!DrawerCache.TryAdd(drawerType, currentDrawerType))
                        continue;

                    if (!forChildren)
                        continue;

                    // Add children type if not already added.
                    foreach (Type childType in TypeCache.GetTypesDerivedFrom(drawerType))
                    {
                        DrawerCache.TryAdd(childType, currentDrawerType);
                    }
                }
            }
        }

        public static void PreloadCustomPropertyItems()
        {
            foreach (Type itemType in TypeCache.GetTypesDerivedFrom(typeof(PropertyItem)))
            {
                // Get all attributes for a custom property value item.
                object[] attributes = itemType.GetCustomAttributes(TypeCPVI, true);
                int count = attributes.Length;

                for (int i = 0; i < count; ++i)
                {
                    // Get the hidden, reflected fields.
                    CustomPropertyItemAttribute currentAtr = (CustomPropertyItemAttribute)attributes[i];
                    Type currentItemType = currentAtr.RegisteredType;
                    bool forChildren = currentAtr.UseForChildren;
                    
                    if (!currentItemType.IsOrIsSubclassOf(itemType))
                        continue;

                    // Ensure the type is added.
                    if (!ValueItemCache.TryAdd(itemType, currentItemType))
                        continue;

                    if (!forChildren)
                        continue;

                    // Add children type if not already added.
                    foreach (Type childType in TypeCache.GetTypesDerivedFrom(itemType))
                    {
                        ValueItemCache.TryAdd(childType, currentItemType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="GUIDrawer"/> for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns an instance of the <see cref="GUIDrawer"/> for the <paramref name="propertyType"/>.</returns>
        public static T GetGUIDrawer<T>(Type propertyType) where T : GUIDrawer
        {
            if (propertyType == null)
                return null;

            // See if the type has already been cached. If so, return an instance of it.
            if (DrawerCache.TryGetValue(propertyType, out Type drawerType))
                return (T)Activator.CreateInstance(drawerType);

            // Otherwise, attempt to find the type.
            drawerType = FindDrawerTypeFromTypeCache(propertyType);
            if (drawerType == null)
                return null;

            // If the drawer type is valid, cache it and return an instance of it.
            DrawerCache.Add(propertyType, drawerType);
            return (T)Activator.CreateInstance(drawerType);
        }

        /// <summary>
        /// Gets the <see cref="GUIDrawer"/> for a given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check..</param>
        /// <returns>Returns an instance of the <see cref="GUIDrawer"/> for the <paramref name="property"/>.</returns>
        public static T GetGUIDrawer<T>(SerializedProperty property) where T : GUIDrawer
        {
            return GetGUIDrawer<T>(property.GetVariableType());
        }

        /// <summary>
        /// Gets the <see cref="PropertyItem"/> for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns an instance of the <see cref="PropertyItem"/> for the <paramref name="propertyType"/>.</returns>
        public static T GetPropertyItem<T>(Type propertyType) where T : PropertyItem
        {
            if (propertyType == null)
                return null;

            // See if the type has already been cached. If so, return an instance of it.
            if (ValueItemCache.TryGetValue(propertyType, out Type itemType))
                return (T)Activator.CreateInstance(itemType);

            // Otherwise, attempt to find the type.
            itemType = FindPropertyItemTypeFromTypeCache(propertyType);
            if (itemType == null)
                return null;

            // If the item type is valid, cache it and return an instance of it.
            ValueItemCache.Add(propertyType, itemType);
            return (T)Activator.CreateInstance(itemType);
        }
        
        /// <summary>
        /// Gets the <see cref="PropertyItem"/> for a given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check..</param>
        /// <returns>Returns an instance of the <see cref="PropertyItem"/> for the <paramref name="property"/>.</returns>
        public static T GetPropertyItem<T>(SerializedProperty property) where T : PropertyItem
        {
            return GetPropertyItem<T>(property.GetVariableType());
        }

        /// <summary>
        /// Gets the <see cref="PropertyDrawer"/> for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns an instance of the <see cref="PropertyDrawer"/> for the <paramref name="propertyType"/>.</returns>
        public static PropertyDrawer GetPropertyDrawer(Type propertyType)
        {
            return GetGUIDrawer<PropertyDrawer>(propertyType);
        }

        /// <summary>
        /// Gets the <see cref="PropertyDrawer"/> for a given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check..</param>
        /// <returns>Returns an instance of the <see cref="PropertyDrawer"/> for the <paramref name="property"/>.</returns>
        public static PropertyDrawer GetPropertyDrawer(SerializedProperty property)
        {
            return GetPropertyDrawer(property.GetVariableType());
        }

        /// <summary>
        /// Gets the <see cref="DecoratorDrawer"/> for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns an instance of the <see cref="DecoratorDrawer"/> for the <paramref name="propertyType"/>.</returns>
        public static DecoratorDrawer GetDecoratorDrawer(Type propertyType)
        {
            return GetGUIDrawer<DecoratorDrawer>(propertyType);
        }

        /// <summary>
        /// Gets the <see cref="DecoratorDrawer"/> for a given <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to check..</param>
        /// <returns>Returns an instance of the <see cref="DecoratorDrawer"/> for the <paramref name="property"/>.</returns>
        public static DecoratorDrawer GetDecoratorDrawer(SerializedProperty property)
        {
            return GetDecoratorDrawer(property.GetVariableType());
        }

        /// <summary>
        /// Attempts to find a <see cref="TooltipAttribute"/> on some <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="inMember">The <see cref="MemberInfo"/> to check.</param>
        /// <returns>Returns the tooltip, if found.</returns>
        public static string TryFindTooltip(MemberInfo inMember)
        {
            if (inMember == null)
                return string.Empty;

            TooltipAttribute tooltipAtr = inMember.GetCustomAttribute<TooltipAttribute>();
            return tooltipAtr?.tooltip ?? string.Empty;
        }

        /// <summary>
        /// Looks through the <see cref="TypeCache"/> for the drawer related to a given property <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns the <see cref="Type"/> of drawer to use for the <see cref="propertyType"/>.</returns>
        private static Type FindDrawerTypeFromTypeCache(Type propertyType)
        {
            foreach (Type drawerType in TypeCache.GetTypesDerivedFrom<GUIDrawer>())
            {
                // Iterate through all attributes to find the proper drawer.
                object[] attributes = drawerType.GetCustomAttributes(TypeCPD, true);
                int count = attributes.Length;

                for (int i = 0; i < count; ++i)
                {
                    object currentAtr = attributes[i];
                    Type drawerPropertyType = FieldCPDType.GetValue(currentAtr) as Type;
                    bool forChildren = (bool)FieldCPDChild.GetValue(currentAtr);

                    if (drawerPropertyType == null)
                        continue;

                    // If the types match, return the drawer.
                    if (drawerPropertyType == propertyType)
                        return drawerType;

                    // If the type is a child type, return the drawer.
                    if (forChildren && (propertyType.IsSubclassOf(drawerPropertyType) || propertyType.IsGenericSubclassOf(drawerPropertyType)))
                        return drawerType;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Looks through the <see cref="TypeCache"/> for the <see cref="PropertyItem"/> related to a given property <see cref="Type"/>.
        /// </summary>
        /// <param name="propertyType">The <see cref="Type"/> of the property.</param>
        /// <returns>Returns the <see cref="Type"/> of <see cref="PropertyItem"/> to use for the <see cref="propertyType"/>.</returns>
        private static Type FindPropertyItemTypeFromTypeCache(Type propertyType)
        {
            foreach (Type itemType in TypeCache.GetTypesDerivedFrom(typeof(PropertyItem)))
            {
                // Get all attributes for a custom property value item.
                object[] attributes = itemType.GetCustomAttributes(TypeCPVI, true);
                int count = attributes.Length;

                for (int i = 0; i < count; ++i)
                {
                    // Get the hidden, reflected fields.
                    CustomPropertyItemAttribute currentAtr = (CustomPropertyItemAttribute)attributes[i];
                    Type currentItemType = currentAtr.RegisteredType;
                    bool forChildren = currentAtr.UseForChildren;

                    if (currentItemType == null)
                        continue;

                    // If the types match, return the drawer.
                    if (currentItemType == propertyType)
                        return itemType;
                    
                    // If the wanted type is an interface, and the property type inherits it, use it.
                    if (currentItemType.IsInterface && propertyType.GetInterfaces().Contains(currentItemType))
                        return itemType;

                    // If the type is a child type, return the drawer.
                    if (forChildren && (propertyType.IsSubclassOf(currentItemType) || propertyType.IsGenericSubclassOf(currentItemType)))
                        return itemType;
                }
            }

            return null;
        }
    }
}
#endif