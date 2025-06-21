using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    public class BoxDrawerWrap : DrawerWrap
    {
        public Vector2 BoxSize = Vector2.zero;
        
        public Vector2 BoxOverdraw = Vector2.zero;

        public GUIContent BoxContent = GUIContent.none;

        private Rect _boxRect;
        
        public BoxDrawerWrap(IDrawerItem wrappedItem) : base(wrappedItem)
        {
        }

        public BoxDrawerWrap(IDrawerItem wrappedItem, Vector2 size) : this(wrappedItem)
        {
            BoxSize = size;
        }

        protected override void OnDrawStart(ref Rect drawRect)
        {
            Style = GUI.skin.box;
            _boxRect = drawRect;
            _boxRect.width = BoxSize.x > 0.0f ? BoxSize.x : drawRect.width;
            _boxRect.height = BoxSize.y > 0.0f ? BoxSize.y : drawRect.height;
            _boxRect.width += BoxOverdraw.x;
            _boxRect.height += BoxOverdraw.y;
            
            BoxContent ??= GUIContent.none;
            Style ??= GUI.skin.box;
            
            GUI.Box(_boxRect, BoxContent, Style);
        }

        public override float GetHeight()
        {
            return _boxRect.height;
        }
    }
}
