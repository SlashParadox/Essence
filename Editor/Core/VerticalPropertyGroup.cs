// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyGroup"/> that places items in a standard column layout. This will be the most
    /// common type of <see cref="PropertyGroup"/>.
    /// </summary>
    public class VerticalPropertyGroup : PropertyGroup
    {
        public VerticalPropertyGroup(GUIContent label = null, GUIStyle style = null, int estimatedElements = 0)
            : base(estimatedElements)
        {
            Label = new VerticalPropertyLabel(label, style);
        }

        public override float GetHeight()
        {
            float height = Label.Label.IsValidGUIContent() ? EditorGUIUtility.singleLineHeight : 0.0f;
            float lineSpacing = DrawerItems.Count > 1 ? GetLineSpacing() : 0.0f;

            if (Label.Label.IsValidGUIContent() && !Label.CanDrawFurtherElements())
                return height;

            foreach (IDrawerItem item in DrawerItems)
            {
                height += item.GetHeight() + lineSpacing;
            }

            return height;
        }

        protected override void OnAfterItemDrawn(ref Rect drawRect, IDrawerItem item)
        {
            if (item != DrawerItems[^1])
                drawRect.y += item.GetHeight() + GetLineSpacing();
        }
    }
}
#endif