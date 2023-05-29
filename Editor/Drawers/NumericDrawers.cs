// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A base class for numeric <see cref="PropertyDrawer"/>s.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <remarks>Ensure that the <see cref="CustomPropertyDrawer"/> attribute is within a 'ESSENCE_USE_NUMERIC_DRAWERS'
    /// preprocessor. Also ensure that <see cref="MakeNewNumericItem"/> is overridden.</remarks>
    public abstract class NumericPropertyDrawer<T> : EssencePropertyDrawer
    {
        /// <summary>The created numeric item.</summary>
        private NumericPropertyItem<T> _numericItem;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _numericItem?.GetHeight() ?? 0.0f;
        }

        protected override void OnDrawerInitialized(SerializedProperty property, GUIContent label)
        {
            EditorValue<T> value = new EditorValue<T>(true, property);
            _numericItem = MakeNewNumericItem(value);

            if (_numericItem != null)
                _numericItem.Label.Label = label;
        }

        protected override void OnGUIDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            _numericItem?.Draw(ref position);
        }

        protected abstract NumericPropertyItem<T> MakeNewNumericItem(EditorValue<T> value);
    }

    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="byte"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(byte))]
#endif
    public class BytePropertyDrawer : NumericPropertyDrawer<byte>
    {
        protected override NumericPropertyItem<byte> MakeNewNumericItem(EditorValue<byte> value)
        {
            return new BytePropertyItem(value);
        }
    }

    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="sbyte"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(sbyte))]
#endif
    public class SBytePropertyDrawer : NumericPropertyDrawer<sbyte>
    {
        protected override NumericPropertyItem<sbyte> MakeNewNumericItem(EditorValue<sbyte> value)
        {
            return new SBytePropertyItem(value);
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="short"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(short))]
#endif
    public class ShortPropertyDrawer : NumericPropertyDrawer<short>
    {
        protected override NumericPropertyItem<short> MakeNewNumericItem(EditorValue<short> value)
        {
            return new ShortPropertyItem(value);
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="ushort"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(ushort))]
#endif
    public class UShortPropertyDrawer : NumericPropertyDrawer<ushort>
    {
        protected override NumericPropertyItem<ushort> MakeNewNumericItem(EditorValue<ushort> value)
        {
            return new UShortPropertyItem(value);
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="uint"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(uint))]
#endif
    public class UIntPropertyDrawer : NumericPropertyDrawer<uint>
    {
        protected override NumericPropertyItem<uint> MakeNewNumericItem(EditorValue<uint> value)
        {
            return new UIntPropertyItem(value);
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for <see cref="ulong"/>s.
    /// </summary>
#if ESSENCE_USE_NUMERIC_DRAWERS
    [CustomPropertyDrawer(typeof(ulong))]
#endif
    public class ULongPropertyDrawer : NumericPropertyDrawer<ulong>
    {
        protected override NumericPropertyItem<ulong> MakeNewNumericItem(EditorValue<ulong> value)
        {
            return new ULongPropertyItem(value);
        }
    }
}
#endif