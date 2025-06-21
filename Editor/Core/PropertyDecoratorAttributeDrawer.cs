using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    public abstract class PropertyDecoratorDrawer : EssencePropertyDrawer
    {
        internal VisualElement nestedElement = null;

        protected virtual void DecorateProperty(SerializedProperty property, PropertyDrawerData data, VisualElement element)
        {

        }

        protected sealed override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            VisualElement usedElement = nestedElement ?? new PropertyField(property);
            DecorateProperty(property, data, usedElement);

            // Don't allow any return when this is a nested drawer.
            return nestedElement != null ? null : usedElement;
        }
    }

    [CustomPropertyDrawer(typeof(PropertyDecoratorAttribute))]
    public class PropertyDecoratorAttributeDrawer : EssencePropertyDrawer
    {
        private class PropertyDecoratorDrawerData : PropertyDrawerData
        {
            public readonly List<PropertyDecoratorDrawer> Drawers = new List<PropertyDecoratorDrawer>();
        }

        protected override PropertyDrawerData CreatePropertyData(SerializedProperty property, GUIContent label)
        {
            PropertyDecoratorDrawerData data = new PropertyDecoratorDrawerData();

            return data;
        }

        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            PropertyDecoratorDrawerData drawerData = data as PropertyDecoratorDrawerData;

            // Gather all other property attributes and sort them into groups: PropertyDecoratorAttributes or regular PropertyAttributes
            List<PropertyAttribute> attributes = fieldInfo?.GetCustomAttributes<PropertyAttribute>().ToList();
            if (attributes.IsEmptyOrNull() || drawerData == null)
                return new PropertyField(property);

            List<PropertyDecoratorAttribute> propertyDecorators = new List<PropertyDecoratorAttribute>();
            PropertyAttribute nextPropertyDrawerType = null;
            int bestPriority = int.MaxValue;

            foreach (PropertyAttribute attr in attributes)
            {
                if (attr == null || Equals(attr, attribute))
                    continue;

                if (attr is PropertyDecoratorAttribute decorator)
                {
                    propertyDecorators.Add(decorator);
                    continue;
                }

                if (EditorCache.HasGUIDrawer<PropertyDrawer>(attr.GetType()))
                {
                    if (attr.order < bestPriority)
                    {
                        bestPriority = attr.order;
                        nextPropertyDrawerType = attr;
                    }
                }
            }

            VisualElement element = new PropertyField(property);

            if (propertyDecorators.Count <= 1 && nextPropertyDrawerType == null)
                return element;

            foreach (PropertyDecoratorAttribute attr in propertyDecorators)
            {
                PropertyDecoratorDrawer drawer = EditorCache.GetGUIDrawer<PropertyDecoratorDrawer>(attr.GetType());
                if (drawer != null)
                {
                    drawerData.Drawers.Add(drawer);
                    drawer.CreatePropertyGUI(property);
                }
            }

            return element;
        }
    }
}
