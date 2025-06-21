// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    public class ScrollDrawerWrap : DrawerWrap
    {
        public float MinScrollBoxWidth;
        
        public float ScrollBoxHeight;

        private Vector2 _scrollPosition;

        private Rect _scrollRect;

        public ScrollDrawerWrap(IDrawerItem wrappedItem) : base(wrappedItem) { }

        public ScrollDrawerWrap(IDrawerItem wrappedItem, float height, float minWidth = 0.0f) : this(wrappedItem)
        {
            ScrollBoxHeight = height;
            MinScrollBoxWidth = minWidth;
        }

        protected override void OnDrawStart(ref Rect drawRect)
        {
            float selectedWidth = Math.Max(drawRect.width, MinScrollBoxWidth) - EditorKit.EditorScrollBarSize;
            Rect viewRect = new Rect(0.0f, 0.0f, selectedWidth, WrappedItem.GetHeight());
            _scrollRect = drawRect;
            _scrollRect.height = ScrollBoxHeight > 0.0f ? ScrollBoxHeight : drawRect.height;

            drawRect.width = selectedWidth - EditorKit.EditorScrollBarSize;
            
            // Reset the draw rect, as we are drawing within the view, which resets coordinates to 0.0f.
            // X is given a forced indent for foldouts.
            drawRect.x = EditorKit.IndentSpacing;
            drawRect.y = 0.0f;

            _scrollPosition = GUI.BeginScrollView(_scrollRect, _scrollPosition, viewRect, false, true);
        }

        protected override void OnDrawEnd(ref Rect drawRect)
        {
            drawRect = InitialRect;
            GUI.EndScrollView();
        }

        public override float GetHeight()
        {
            return _scrollRect.height + EditorGUIUtility.singleLineHeight;
        }
    }
}