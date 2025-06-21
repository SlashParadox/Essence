// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for numeric types.
    /// </summary>
    /// <typeparam name="T">The stored numeric type.</typeparam>
    public abstract class NumericPropertyItem<T> : PropertyValueItem<T>
    {
        /// <summary>The default style to use when the label does not have one.</summary>
        protected static GUIStyle DefaultStyle { get { return EditorStyles.numberField; } }

        /// <summary>See: <see cref="Style"/></summary>
        private GUIStyle _style;

        public override GUIStyle Style { get { return _style ?? DefaultStyle; } set { _style = value; } }

        protected NumericPropertyItem() : base(null) { }

        protected NumericPropertyItem(EditorValue<T> value, PropertyLabel label)
            : base(value, label) { }

        protected NumericPropertyItem(EditorValue<T> value, GUIContent label)
            : base(value, label) { }

        protected NumericPropertyItem(EditorValue<T> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="byte"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(byte))]
    public class BytePropertyItem : NumericPropertyItem<byte>
    {
        public BytePropertyItem() : base(null) { }

        public BytePropertyItem(EditorValue<byte> value, PropertyLabel label)
            : base(value, label) { }

        public BytePropertyItem(EditorValue<byte> value, GUIContent label)
            : base(value, label) { }

        public BytePropertyItem(EditorValue<byte> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            byte startValue = Value.GetCurrentValue();
            int baseResult = EditorGUI.IntField(drawRect, Label?.ConditionalLabel, startValue, Style);
            byte result = (byte)System.Math.Clamp(baseResult, byte.MinValue, byte.MaxValue);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="sbyte"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(sbyte))]
    public class SBytePropertyItem : NumericPropertyItem<sbyte>
    {
        public SBytePropertyItem() : base(null) { }

        public SBytePropertyItem(EditorValue<sbyte> value, PropertyLabel label)
            : base(value, label) { }

        public SBytePropertyItem(EditorValue<sbyte> value, GUIContent label)
            : base(value, label) { }

        public SBytePropertyItem(EditorValue<sbyte> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            sbyte startValue = Value.GetCurrentValue();
            int baseResult = EditorGUI.IntField(drawRect, Label?.ConditionalLabel, startValue, Style);
            sbyte result = (sbyte)System.Math.Clamp(baseResult, sbyte.MinValue, sbyte.MaxValue);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="short"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(short))]
    public class ShortPropertyItem : NumericPropertyItem<short>
    {
        public ShortPropertyItem() : base(null) { }

        public ShortPropertyItem(EditorValue<short> value, PropertyLabel label)
            : base(value, label) { }

        public ShortPropertyItem(EditorValue<short> value, GUIContent label)
            : base(value, label) { }

        public ShortPropertyItem(EditorValue<short> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            short startValue = Value.GetCurrentValue();
            int baseResult = EditorGUI.IntField(drawRect, Label?.ConditionalLabel, startValue, Style);
            short result = (short)System.Math.Clamp(baseResult, short.MinValue, short.MaxValue);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="ushort"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(ushort))]
    public class UShortPropertyItem : NumericPropertyItem<ushort>
    {
        public UShortPropertyItem() : base(null) { }

        public UShortPropertyItem(EditorValue<ushort> value, PropertyLabel label)
            : base(value, label) { }

        public UShortPropertyItem(EditorValue<ushort> value, GUIContent label)
            : base(value, label) { }

        public UShortPropertyItem(EditorValue<ushort> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            ushort startValue = Value.GetCurrentValue();
            int baseResult = EditorGUI.IntField(drawRect, Label?.ConditionalLabel, startValue, Style);
            ushort result = (ushort)System.Math.Clamp(baseResult, ushort.MinValue, ushort.MaxValue);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="int"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(int))]
    public class IntPropertyItem : NumericPropertyItem<int>
    {
        public IntPropertyItem() : base(null) { }

        public IntPropertyItem(EditorValue<int> value, PropertyLabel label)
            : base(value, label) { }

        public IntPropertyItem(EditorValue<int> value, GUIContent label)
            : base(value, label) { }

        public IntPropertyItem(EditorValue<int> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            int startValue = Value.GetCurrentValue();
            int result = EditorGUI.IntField(drawRect, Label?.ConditionalLabel, startValue, Style);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="uint"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(uint))]
    public class UIntPropertyItem : NumericPropertyItem<uint>
    {
        public UIntPropertyItem() : base(null) { }

        public UIntPropertyItem(EditorValue<uint> value, PropertyLabel label)
            : base(value, label) { }

        public UIntPropertyItem(EditorValue<uint> value, GUIContent label)
            : base(value, label) { }

        public UIntPropertyItem(EditorValue<uint> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            uint startValue = Value.GetCurrentValue();
            long baseResult = EditorGUI.LongField(drawRect, Label?.ConditionalLabel, startValue, Style);
            uint result = (uint)System.Math.Clamp(baseResult, uint.MinValue, uint.MaxValue);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="long"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(long))]
    public class LongPropertyItem : NumericPropertyItem<long>
    {
        public LongPropertyItem() : base(null) { }

        public LongPropertyItem(EditorValue<long> value, PropertyLabel label)
            : base(value, label) { }

        public LongPropertyItem(EditorValue<long> value, GUIContent label)
            : base(value, label) { }

        public LongPropertyItem(EditorValue<long> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            long startValue = Value.GetCurrentValue();
            long result = EditorGUI.LongField(drawRect, Label?.ConditionalLabel, startValue, Style);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="int"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(ulong))]
    public class ULongPropertyItem : NumericPropertyItem<ulong>
    {
        /// <summary>The hash code of a <see cref="ULongPropertyItem"/>. Used for drag control.</summary>
        private static readonly int Hash = "ULongItem".GetHashCode();

        /// <summary>The dragging sensitivity.</summary>
        private static readonly double DragSensitivity = 500.0;

        public ULongPropertyItem() : base(null) { }

        public ULongPropertyItem(EditorValue<ulong> value, PropertyLabel label)
            : base(value, label) { }

        public ULongPropertyItem(EditorValue<ulong> value, GUIContent label)
            : base(value, label) { }

        public ULongPropertyItem(EditorValue<ulong> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Rect dragRect = drawRect;
            dragRect.width = EditorGUIUtility.labelWidth;

            if (Label.WasDrawn)
                dragRect.x -= EditorGUIUtility.labelWidth;

            EditorGUI.BeginChangeCheck();
            ulong value = Value.GetCurrentValue();
            string input = EditorGUI.TextField(drawRect, Label.ConditionalLabel, value.ToString());
            bool changed = EditorGUI.EndChangeCheck();

            // Calculate a control ID and check if a drag is occuring in the drag zone.
            int controlID = GUIUtility.GetControlID(Hash, FocusType.Keyboard, dragRect);
            if (EditorKit.CheckDrag(dragRect, controlID))
            {
                // Update based on the drag sensitivity, and clamp to the ulong's range.
                double dValue = value;
                dValue += System.Math.Ceiling(DragSensitivity * System.Math.Sign(HandleUtility.niceMouseDelta));
                value = (ulong)System.Math.Clamp(dValue, ulong.MinValue, ulong.MaxValue);
            }
            else
            {
                // Otherwise, check the text input. If it can be parsed, parse it out.
                if (changed && ulong.TryParse(input, out ulong result))
                    value = result;
            }

            Value.SetLatestResult(value);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="float"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(float))]
    public class FloatPropertyItem : NumericPropertyItem<float>
    {
        public FloatPropertyItem() : base(null) { }

        public FloatPropertyItem(EditorValue<float> value, PropertyLabel label)
            : base(value, label) { }

        public FloatPropertyItem(EditorValue<float> value, GUIContent label)
            : base(value, label) { }

        public FloatPropertyItem(EditorValue<float> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            float startValue = Value.GetCurrentValue();
            float result = EditorGUI.FloatField(drawRect, Label?.ConditionalLabel, startValue, Style);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="double"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(double))]
    public class DoublePropertyItem : NumericPropertyItem<double>
    {
        public DoublePropertyItem() : base(null) { }

        public DoublePropertyItem(EditorValue<double> value, PropertyLabel label)
            : base(value, label) { }

        public DoublePropertyItem(EditorValue<double> value, GUIContent label)
            : base(value, label) { }

        public DoublePropertyItem(EditorValue<double> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            double startValue = Value.GetCurrentValue();
            double result = EditorGUI.DoubleField(drawRect, Label?.ConditionalLabel, startValue, Style);

            Value.SetLatestResult(result);
            return Value.LastResult;
        }
    }

    /// <summary>
    /// A <see cref="NumericPropertyItem{T}"/> for <see cref="decimal"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(decimal))]
    public class DecimalPropertyItem : NumericPropertyItem<decimal>
    {
        /// <summary>The hash code of a <see cref="ULongPropertyItem"/>. Used for drag control.</summary>
        private static readonly int Hash = "ULongItem".GetHashCode();

        /// <summary>The dragging sensitivity.</summary>
        private static readonly double DragSensitivity = 500.0;

        public DecimalPropertyItem() : base(null) { }

        public DecimalPropertyItem(EditorValue<decimal> value, PropertyLabel label)
            : base(value, label) { }

        public DecimalPropertyItem(EditorValue<decimal> value, GUIContent label)
            : base(value, label) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Rect dragRect = drawRect;
            dragRect.width = EditorGUIUtility.labelWidth;

            if (Label.WasDrawn)
                dragRect.x -= EditorGUIUtility.labelWidth;

            EditorGUI.BeginChangeCheck();
            decimal value = Value.GetCurrentValue();
            string input = EditorGUI.TextField(drawRect, Label.ConditionalLabel, value.ToString(CultureInfo.InvariantCulture));
            bool changed = EditorGUI.EndChangeCheck();

            // Calculate a control ID and check if a drag is occuring in the drag zone.
            int controlID = GUIUtility.GetControlID(Hash, FocusType.Keyboard, dragRect);
            if (EditorKit.CheckDrag(dragRect, controlID))
            {
                // Update based on the drag sensitivity, and clamp to the ulong's range.
                decimal dValue = value;
                dValue += (decimal)System.Math.Ceiling(DragSensitivity * System.Math.Sign(HandleUtility.niceMouseDelta));
                value = System.Math.Clamp(dValue, decimal.MinValue, decimal.MaxValue);
            }
            else
            {
                // Otherwise, check the text input. If it can be parsed, parse it out.
                if (changed && decimal.TryParse(input, out decimal result))
                    value = result;
            }

            Value.SetLatestResult(value);
            return Value.LastResult;
        }
    }
}
#endif