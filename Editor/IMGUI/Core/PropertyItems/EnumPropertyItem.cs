// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Color"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Enum), true)]
    public class EnumPropertyItem : PropertyValueItem<Enum>
    {
        /// <summary>The default style to use when the item does not have one.</summary>
        protected static GUIStyle DefaultStyle { get { return EditorStyles.popup; } }

        /// <summary>An optional <see cref="Func{T1,TResult}"/> to see if a value is enabled.</summary>
        public Func<Enum, bool> CheckEnabledFunc = null;

        /// <summary>If true, obsolete values are added.</summary>
        public bool IncludeObsolete = false;

        /// <summary>See: <see cref="Style"/></summary>
        private GUIStyle _style;

        public override GUIStyle Style { get { return _style ?? DefaultStyle; } set { _style = value; } }

        public EnumPropertyItem() { }

        public EnumPropertyItem(EditorValue<Enum> value, GUIContent label)
            : base(value, label) { }

        public EnumPropertyItem(EditorValue<Enum> value, PropertyLabel label)
            : base(value, label) { }

        public EnumPropertyItem(EditorValue<Enum> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            if (Value == null)
                return null;

            Enum result = EditorGUI.EnumPopup(drawRect, Label?.ConditionalLabel, Value.GetCurrentValue(), CheckEnabledFunc, IncludeObsolete, Style);
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif