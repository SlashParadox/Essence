// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Color"/>.
    /// </summary>
    [CustomPropertyItem(typeof(string))]
    public class TextPropertyItem : PropertyValueItem<string>
    {
        /// <summary>The default style to use when the item does not have one.</summary>
        protected static GUIStyle DefaultStyle { get { return EditorStyles.textField; } }

        /// <summary>If this or <see cref="MaxLines"/> is greater than 0, uses a text area.</summary>
        public int MinLines;

        /// <summary>If this or <see cref="MinLines"/> is greater than 0, uses a text area.</summary>
        public int MaxLines;

        /// <summary>See: <see cref="Style"/></summary>
        private GUIStyle _style;

        /// <summary>The minimum number of lines actually used for the text area.</summary>
        private int _usedMinLines;

        /// <summary>The maximum number of lines actually used for the text area.</summary>
        private int _usedMaxLines;

        /// <summary>The width of the text area as drawn.</summary>
        private float _drawnTextAreaWidth;

        /// <summary>The last known scroll position of the text area.</summary>
        private Vector2 _scrollPosition;

        public override GUIStyle Style { get { return _style ?? DefaultStyle; } set { _style = value; } }

        public TextPropertyItem() { }

        public TextPropertyItem(EditorValue<string> value, GUIContent label, int minLines = 0, int maxLines = 0)
            : base(value, label)
        {
            MinLines = minLines;
            MaxLines = maxLines;
        }

        public TextPropertyItem(EditorValue<string> value, PropertyLabel label, int minLines = 0, int maxLines = 0)
            : base(value, label)
        {
            MinLines = minLines;
            MaxLines = maxLines;
        }

        public TextPropertyItem(EditorValue<string> value, bool bMakeLabelFromValue = true, int minLines = 0, int maxLines = 0)
            : base(value, bMakeLabelFromValue)
        {
            MinLines = minLines;
            MaxLines = maxLines;
        }

        public override float GetHeight()
        {
            float height = GetTextAreaSize(_usedMinLines, _usedMaxLines, _drawnTextAreaWidth);

            // Add a label line.
            if (Label != null && !Label.WasDrawn && Label.ConditionalLabel.IsValidGUIContent())
                height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            _usedMinLines = 1;
            _usedMaxLines = 1;
            _drawnTextAreaWidth = drawRect.width;

            if (Value == null)
                return null;

            string result;

            // Get the text area attribute if available.
            MemberInfo info = Value.GetMemberInfo();
            TextAreaAttribute textAreaAttribute = info != null ? info.GetCustomAttribute<TextAreaAttribute>() : null;
            bool useTextArea = textAreaAttribute != null || MinLines > 0 || MaxLines > 0;

            // If the line is a single text field, draw it simply and return.
            if (!useTextArea)
            {
                result = EditorGUI.TextField(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), Style);
                Value.SetLatestResult(result);
                return result;
            }

            // Calculate the minimum and maximum lives.
            if (textAreaAttribute != null)
                _usedMinLines = System.Math.Max(System.Math.Max(MinLines, textAreaAttribute.minLines), 1);
            else
                _usedMinLines = System.Math.Max(MinLines, 1);

            if (textAreaAttribute != null)
                _usedMaxLines = System.Math.Max(System.Math.Max(MaxLines, textAreaAttribute.maxLines), _usedMinLines);
            else
                _usedMaxLines = System.Math.Max(_usedMinLines, MaxLines);

            // Draw the label if it hasn't already.
            if (Label != null && !Label.WasDrawn && Label.ConditionalLabel.IsValidGUIContent())
            {
                Rect labelRect = drawRect;
                labelRect.height = EditorGUIUtility.singleLineHeight;
                labelRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(labelRect, Label.ConditionalLabel);
                drawRect.y += EditorGUIUtility.singleLineHeight;
            }

            // Draw the scrollable text area. This is a hidden function, so we get it via reflection.
            drawRect.height = GetTextAreaSize(_usedMinLines, _usedMaxLines, _drawnTextAreaWidth);
            result = EditorKit.ScrollableTextArea(drawRect, Value.GetCurrentValue(), ref _scrollPosition, EditorStyles.textArea);
            drawRect.height = EditorGUIUtility.singleLineHeight;

            Value.SetLatestResult(result);
            return result;
        }

        /// <summary>
        /// Gets the size of a text area.
        /// </summary>
        /// <param name="minLines">The minimum lines allowed.</param>
        /// <param name="maxLines">The maximum lines allowed.</param>
        /// <param name="areaWidth">The width of the area the text area was drawn in.</param>
        /// <returns>Returns the estimated text area size.</returns>
        protected float GetTextAreaSize(int minLines, int maxLines, float areaWidth)
        {
            float singleLine = EditorGUIUtility.singleLineHeight;

            float baseHeight = EditorStyles.textArea.CalcHeight(new GUIContent(Value.LastResult), areaWidth);
            int numLines = Mathf.CeilToInt(baseHeight / singleLine);
            numLines = Mathf.Clamp(numLines, minLines, maxLines);

            return numLines * singleLine;
        }
    }
}
#endif