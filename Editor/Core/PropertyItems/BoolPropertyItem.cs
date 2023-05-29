// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="bool"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(bool))]
    public class BoolPropertyItem : PropertyValueItem<bool>
    {
        /// <summary>The default style to use when the label does not have one.</summary>
        protected static GUIStyle DefaultStyle { get { return EditorStyles.toggle; } }

        /// <summary>See: <see cref="Style"/></summary>
        private GUIStyle _style;

        /// <summary>If true, the toggle should be on the left.</summary>
        protected bool LeftToggle { get; }

        public override GUIStyle Style { get { return _style ?? DefaultStyle; } set { _style = value; } }

        public BoolPropertyItem()
        {
            LeftToggle = false;
        }

        public BoolPropertyItem(EditorValue<bool> value, GUIContent label, bool bLeft = false)
            : base(value, label)
        {
            LeftToggle = bLeft;
        }

        public BoolPropertyItem(EditorValue<bool> value, PropertyLabel label, bool bLeft = false)
            : base(value, label)
        {
            LeftToggle = bLeft;
        }

        public BoolPropertyItem(EditorValue<bool> value, bool bLeft = false, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue)
        {
            LeftToggle = bLeft;
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            bool result;

            if (LeftToggle)
                result = EditorGUI.ToggleLeft(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), Style);
            else
                result = EditorGUI.Toggle(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), Style);

            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif