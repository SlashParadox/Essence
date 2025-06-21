// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyGroup"/> that spaces out items on a single line.
    /// </summary>
    public class HorizontalPropertyGroup : PropertyGroup
    {
        public bool AppendHeightToY = true;
        
        public HorizontalPropertyGroup(GUIContent label = null, GUIStyle style = null, int estimatedElements = 0)
            : base(estimatedElements)
        {
            Label = new HorizontalPropertyLabel(label, style);
        }

        public override float GetHeight()
        {
            float maxHeight = 0;

            foreach (IDrawerItem item in DrawerItems)
            {
                if (item.CanDraw()) 
                    maxHeight = System.Math.Max(maxHeight, item.GetHeight());
            }

            return maxHeight;
        }

        protected override void OnDrawEnd(ref Rect drawRect)
        {
            drawRect.x = InitialRect.x;
            drawRect.width = InitialRect.width;
            
            // if (AppendHeightToY)
            //     drawRect.y += GetHeight() + GetLineSpacing();
        }

        protected override void OnBeforeItemDrawn(ref Rect drawRect, IDrawerItem item)
        {
            drawRect.width = GetExpectedItemWeightedWidth(item);
        }

        protected override void OnAfterItemDrawn(ref Rect drawRect, IDrawerItem item)
        {
            drawRect.x += GetExpectedItemWeightedWidth(item);
            drawRect.width = InitialRect.width;
        }
    }
}
#endif