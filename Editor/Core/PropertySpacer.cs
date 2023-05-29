// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A spacer element for <see cref="PropertyGroup"/>s.
    /// <see cref="VerticalSpacing"/> is for vertical groups.
    /// <see cref="SpaceWeight"/> is for horizontal groups.
    /// </summary>
    public class PropertySpacer : IDrawerItem
    {
        /// <summary>A simple basic spacer with default sizing.</summary>
        public static readonly PropertySpacer DefaultSpacer = new PropertySpacer();

        /// <summary>The vertical spacing to apply, in pixels.</summary>
        public float VerticalSpacing = EditorGUIUtility.singleLineHeight;

        public float SpaceWeight { get; set; }

        public int IndentAmount { get { return 0; } set { } }

        public PropertyLabel Label { get { return null; } set { } }

        public GUIStyle Style { get { return null; } set { } }

        public bool AutoDrawLabel { get { return false; } set { } }

        public PropertySpacer(float spaceWeight = 1.0f)
        {
            SpaceWeight = spaceWeight;
        }

        public bool CanDraw()
        {
            return SpaceWeight > 0;
        }

        public void Draw(ref Rect drawRect)
        {
            // Always empty. This draws nothing, just takes up space.
        }

        public float GetHeight()
        {
            return VerticalSpacing;
        }
    }
}
#endif