using SlashParadox.Essence.Editor.Inspector.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SlashParadox.Essence.Editor
{
    [CustomPropertyDrawer(typeof(CurveScaledFloat))]
    public class CurveScaledFloatDrawer : EssencePropertyDrawer
    {
        protected override VisualElement OnCreatePropertyGUI(SerializedProperty property, PropertyDrawerData data)
        {
            return new CurveScaledFloatField(property);
        }
    }
}
