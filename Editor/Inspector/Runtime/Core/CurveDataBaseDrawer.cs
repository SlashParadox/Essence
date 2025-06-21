// Copyright (c) Craig Williams, SlashParadox

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A base class for editors related to <see cref="CurveTableBase{TValue}"/>s.
    /// </summary>
    public abstract class CurveDataBaseDrawer : EssencePropertyDrawer
    {
        [SerializeField] protected VisualTreeAsset CurveElementAsset;

        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            return new CurveDataBaseField(property, CreateCurveElement(property));
        }

        protected virtual VisualElement CreateCurveElement(SerializedProperty property)
        {
            if (!CurveElementAsset)
                return null;

            VisualElement element = CurveElementAsset.Instantiate();

            CurveField curveField = element?.Q<CurveField>("CurveField");
            curveField?.BindProperty(property.FindPropertyRelative("curve"));

            return curveField;
        }
    }
}
