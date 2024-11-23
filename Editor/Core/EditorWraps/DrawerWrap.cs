using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    public abstract class DrawerWrap : IDrawerItem
    {
        public float Weight { get; set; }

        public bool IsFixedWeight { get; set; }

        public int IndentAmount { get; set; }

        public PropertyLabel Label { get { return null; } set { } }

        public GUIStyle Style { get; set; }

        public bool AutoDrawLabel { get { return false; } set { } }
        
        /// <summary>The initial <see cref="Rect"/> at the start of the draw call.</summary>
        protected Rect InitialRect { get; private set; }
        
        /// <summary>The single child of the wrapper, which is drawn within the wrap.</summary>
        public IDrawerItem WrappedItem { get; set; }

        protected DrawerWrap(IDrawerItem wrappedItem)
        {
            WrappedItem = wrappedItem;
        }

        public virtual bool CanDraw()
        {
            return WrappedItem != null;
        }

        public void Draw(ref Rect drawRect)
        {
            if (!CanDraw() || WrappedItem == null)
                return;

            float indent = IndentAmount * EditorKit.IndentSpacing;
            drawRect.x += indent;
            drawRect.width -= indent;
            InitialRect = drawRect;
            
            OnDrawStart(ref drawRect);
            WrappedItem.Draw(ref drawRect);
            OnDrawEnd(ref drawRect);

            drawRect.x -= indent;
            drawRect.width += indent;
        }

        public virtual float GetHeight()
        {
            return WrappedItem?.GetHeight() ?? 0.0f;
        }
        
        /// <summary>
        /// An event called just before drawing the <see cref="WrappedItem"/>.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the group.</param>
        /// <returns>Returns if further elements can be drawn.</returns>
        protected virtual void OnDrawStart(ref Rect drawRect) { }

        /// <summary>
        /// An event called just at the end of drawing the <see cref="WrappedItem"/>.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the group.</param>
        protected virtual void OnDrawEnd(ref Rect drawRect) { }
    }
}
