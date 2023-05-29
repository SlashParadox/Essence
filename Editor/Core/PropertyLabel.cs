// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A container for label properties. Override to create custom ways of drawing a prefix label. You can even just apply
    /// a width if your label is drawn in a different control or field.
    /// </summary>
    public abstract class PropertyLabel
    {
        /// <summary>
        /// If true, this label does not actually draw a label field, but rather, sets <see cref="EditorGUIUtility.labelWidth"/>
        /// temporarily to a new size, then resets it on cleanup. Set to true if you are drawing the label with a different field.
        /// </summary>
        public bool OnlyApplyWidth;

        /// <summary>The amount of indent to apply to future lines. Optionally used by the label.</summary>
        public int IndentAmount;

        /// <summary>An optional fixed pixel width to use for the label. Does not take priority over <see cref="WidthRatio"/>.
        /// Ignored if 0 or less.</summary>
        public float FixedWidth;

        /// <summary>See: <see cref="Label"/></summary>
        private GUIContent _label = GUIContent.none;

        /// <summary>See: <see cref="Style"/></summary>
        private GUIStyle _style = EditorStyles.label;

        /// <summary>See: <see cref="WidthRatio"/></summary>
        private float _widthRatio;

        /// <summary>Gives the <see cref="Label"/> if not drawn, or empty <see cref="GUIContent"/> if already drawn.</summary>
        public GUIContent ConditionalLabel { get { return WasDrawn ? GUIContent.none : Label; } }

        /// <summary>The <see cref="GUIContent"/> to display.</summary>
        public GUIContent Label { get { return _label ?? GUIContent.none; } set { _label = value; } }

        /// <summary>The <see cref="GUIStyle"/> to use.</summary>
        public GUIStyle Style { get { return _style ?? EditorStyles.label; } set { _style = value; } }

        /// <summary>If true, the last draw call was successful.</summary>
        public bool WasDrawn { get; private set; }

        /// <summary>
        /// The ratio of space this label should take up horizontally. If 0 or less, the label's width will match up to
        /// that of <see cref="EditorGUIUtility.labelWidth"/>. Otherwise, it will be this ratio of the given <see cref="Rect"/>'s width.
        /// </summary>
        public float WidthRatio { get { return _widthRatio; } set { _widthRatio = System.Math.Min(value, 1.0f); } }

        /// <summary>The width decided on for the label.</summary>
        protected float UsedLabelWidth { get; private set; }

        /// <summary>The initial label width that was in use for this control.</summary>
        protected float InitialLabelWidth { get; private set; }

        protected PropertyLabel(GUIContent label = null, GUIStyle style = null, bool onlyWidth = false, float widthRatio = 0.0f, float fixedWidth = 0.0f)
        {
            Label = label;
            Style = style;
            OnlyApplyWidth = onlyWidth;
            WidthRatio = widthRatio;
            FixedWidth = fixedWidth;

            WasDrawn = false;
        }

        /// <summary>
        /// Draws the label. Check <see cref="WasDrawn"/> for success or failure.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        public void DrawLabel(ref Rect drawRect)
        {
            WasDrawn = false;

            InitialLabelWidth = EditorGUIUtility.labelWidth;
            UsedLabelWidth = CalculateLabelWidth(drawRect);

            // Apply the label width to use first and foremost. Stop here if required.
            EditorGUIUtility.labelWidth = UsedLabelWidth;
            if (OnlyApplyWidth)
                return;

            // Start drawing the label with the rect allowed.
            WasDrawn = OnDrawLabel(ref drawRect, UsedLabelWidth);

            if (WasDrawn)
                return;

            // If not drawn, reset our settings.
            UsedLabelWidth = -1.0f;
            ForceResetToInitialLabelWidth();
            InitialLabelWidth = -1.0f;
        }

        /// <summary>
        /// Cleans up the label if it was drawn.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        public void CleanUpLabel(ref Rect drawRect)
        {
            if (WasDrawn)
                OnCleanupLabel(ref drawRect, UsedLabelWidth);

            UsedLabelWidth = -1.0f;
            ForceResetToInitialLabelWidth();
            InitialLabelWidth = -1.0f;
        }

        /// <summary>
        /// resets <see cref="EditorGUIUtility.labelWidth"/> back to the initial width at first draw.
        /// </summary>
        public void ForceResetToInitialLabelWidth()
        {
            if (InitialLabelWidth > 0.0f)
                EditorGUIUtility.labelWidth = InitialLabelWidth;
        }

        /// <summary>
        /// Calculates the width of the label to use, based on current settings and the given <see cref="Rect"/>.
        /// </summary>
        /// <param name="drawRect">The <see cref="Rect"/> the label is being drawn into.</param>
        /// <returns>Returns the width to use.</returns>
        protected float CalculateLabelWidth(Rect drawRect)
        {
            float editorLabelWidth = EditorGUIUtility.labelWidth;
            float maxLabelWidth = System.Math.Min(editorLabelWidth, drawRect.width);

            // Width ratio takes priority.
            if (_widthRatio > 0.0f)
                return maxLabelWidth * _widthRatio;

            // Failing that, use a fixed width.
            if (FixedWidth > 0.0f)
                return System.Math.Min(editorLabelWidth, FixedWidth);

            // Otherwise, return the standard width.
            return maxLabelWidth;
        }

        /// <summary>
        /// Checks if further elements can be drawn. Useful to change if you have a foldout.
        /// </summary>
        /// <returns>Returns if further elements can be drawn.</returns>
        public virtual bool CanDrawFurtherElements()
        {
            return true;
        }

        /// <summary>
        /// Attempts to draw the label.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        /// <param name="labelWidth">The chosen label width. This has already applied to <see cref="EditorGUIUtility.labelWidth"/>.</param>
        /// <returns>Returns if the label was drawn or not.</returns>
        protected virtual bool OnDrawLabel(ref Rect drawRect, float labelWidth)
        {
            return false;
        }

        /// <summary>
        /// Cleans up the label if it was drawn.
        /// </summary>
        /// <param name="drawRect">The current <see cref="Rect"/> of the property to draw.</param>
        /// <param name="labelWidth">The chosen label width.</param>
        protected virtual void OnCleanupLabel(ref Rect drawRect, float labelWidth) { }
    }

    public class NormalPropertyLabel : PropertyLabel
    {
        public NormalPropertyLabel(GUIContent label = null, GUIStyle style = null)
            : base(label, style, false, -1.0f, -1.0f)
        {
            IndentAmount = 0;
        }

        protected override bool OnDrawLabel(ref Rect drawRect, float labelWidth)
        {
            return false;
        }
    }

    /// <summary>
    /// A standard type of <see cref="PropertyLabel"/>, which draws to the left of a property.
    /// </summary>
    public class HorizontalPropertyLabel : PropertyLabel
    {
        public HorizontalPropertyLabel(GUIContent label = null, GUIStyle style = null, bool onlyWidth = false, float widthRatio = 0, float fixedWidth = 0)
            : base(label, style, onlyWidth, widthRatio, fixedWidth)
        {
            IndentAmount = 0;
        }

        protected override bool OnDrawLabel(ref Rect drawRect, float labelWidth)
        {
            if (!Label.IsValidGUIContent())
                return false;

            EditorGUI.indentLevel += IndentAmount;
            EditorGUI.LabelField(drawRect, Label, Style);

            float totalWidth = labelWidth + EditorKit.LabelHorizontalSpacing;
            drawRect.width -= totalWidth;
            drawRect.x += totalWidth;

            return true;
        }

        protected override void OnCleanupLabel(ref Rect drawRect, float labelWidth)
        {
            base.OnCleanupLabel(ref drawRect, labelWidth);

            EditorGUI.indentLevel -= IndentAmount;
        }
    }

    /// <summary>
    /// A standard type of <see cref="PropertyLabel"/>, which draws on top of other items.
    /// </summary>
    public class VerticalPropertyLabel : PropertyLabel
    {
        /// <summary>If true, vertical spacing after this label is applied automatically.</summary>
        public bool ApplyVerticalSpacing = false;

        public VerticalPropertyLabel(GUIContent label = null, GUIStyle style = null, bool onlyWidth = false, float widthRatio = 0, float fixedWidth = 0)
            : base(label, style, onlyWidth, widthRatio, fixedWidth)
        {
            IndentAmount = 1;
        }

        protected override bool OnDrawLabel(ref Rect drawRect, float labelWidth)
        {
            if (!Label.IsValidGUIContent())
                return false;

            EditorGUI.LabelField(drawRect, Label, Style);

            drawRect.y += EditorGUIUtility.singleLineHeight + (ApplyVerticalSpacing ? EditorGUIUtility.standardVerticalSpacing : 0);
            EditorGUI.indentLevel += IndentAmount;

            return true;
        }

        protected override void OnCleanupLabel(ref Rect drawRect, float labelWidth)
        {
            base.OnCleanupLabel(ref drawRect, labelWidth);

            EditorGUI.indentLevel -= IndentAmount;
        }
    }

    /// <summary>
    /// A standard type of <see cref="PropertyLabel"/>, which draws on top of other items.
    /// </summary>
    public class FoldoutPropertyLabel : VerticalPropertyLabel
    {
        /// <summary>If true, the foldout is open.</summary>
        public bool IsOpen { get; protected set; }

        public FoldoutPropertyLabel(GUIContent label = null, GUIStyle style = null, bool onlyWidth = false, float widthRatio = 0, float fixedWidth = 0)
            : base(label, style, onlyWidth, widthRatio, fixedWidth)
        {
            IndentAmount = 1;
        }

        public override bool CanDrawFurtherElements()
        {
            return IsOpen;
        }

        protected override bool OnDrawLabel(ref Rect drawRect, float labelWidth)
        {
            if (!Label.IsValidGUIContent())
                return false;

            IsOpen = EditorGUI.Foldout(drawRect, IsOpen, Label, true);

            drawRect.y += EditorGUIUtility.singleLineHeight + (ApplyVerticalSpacing ? EditorGUIUtility.standardVerticalSpacing : 0);
            EditorGUI.indentLevel += IsOpen ? IndentAmount : 0;

            return true;
        }

        protected override void OnCleanupLabel(ref Rect drawRect, float labelWidth)
        {
            EditorGUI.indentLevel -= IsOpen ? IndentAmount : 0;
        }
    }
}
#endif