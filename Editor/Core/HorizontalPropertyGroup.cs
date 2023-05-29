// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyGroup"/> that spaces out items on a single line.
    /// </summary>
    public class HorizontalPropertyGroup : PropertyGroup
    {
        /// <summary>The total amount of weight of the <see cref="PropertyGroup.DrawerItems"/>.</summary>
        private float _totalWeight;

        /// <summary>The initial <see cref="Rect"/> at the start of the draw call.</summary>
        private Rect _initialRect;

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

        protected override void OnDrawStart(ref Rect drawRect)
        {
            _totalWeight = 0;
            _initialRect = drawRect;

            foreach (IDrawerItem item in DrawerItems)
            {
                if (item.CanDraw())
                    _totalWeight += item.SpaceWeight;
            }
        }

        protected override void OnDrawEnd(ref Rect drawRect)
        {
            drawRect.x = _initialRect.x;
            drawRect.width = _initialRect.width;
            drawRect.y += GetHeight() + GetLineSpacing();
        }

        protected override void OnBeforeItemDrawn(ref Rect drawRect, IDrawerItem item)
        {
            drawRect.width *= item.SpaceWeight / _totalWeight;
        }

        protected override void OnAfterItemDrawn(ref Rect drawRect, IDrawerItem item)
        {
            drawRect.x += _initialRect.width * (item.SpaceWeight / _totalWeight);
            drawRect.width = _initialRect.width;
        }
    }
}
#endif