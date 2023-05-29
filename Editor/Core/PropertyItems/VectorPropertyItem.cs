// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Vector2"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Vector2))]
    public class Vector2PropertyItem : PropertyValueItem<Vector2>
    {
        public Vector2PropertyItem() { }

        public Vector2PropertyItem(EditorValue<Vector2> value, GUIContent label)
            : base(value, label) { }

        public Vector2PropertyItem(EditorValue<Vector2> value, PropertyLabel label)
            : base(value, label) { }

        public Vector2PropertyItem(EditorValue<Vector2> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Vector2 result = EditorGUI.Vector2Field(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }

    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Vector2Int"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(Vector2Int))]
    public class Vector2IntPropertyItem : PropertyValueItem<Vector2Int>
    {
        public Vector2IntPropertyItem() { }

        public Vector2IntPropertyItem(EditorValue<Vector2Int> value, GUIContent label)
            : base(value, label) { }

        public Vector2IntPropertyItem(EditorValue<Vector2Int> value, PropertyLabel label)
            : base(value, label) { }

        public Vector2IntPropertyItem(EditorValue<Vector2Int> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Vector2Int result = EditorGUI.Vector2IntField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Vector3"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Vector3))]
    public class Vector3PropertyItem : PropertyValueItem<Vector3>
    {
        public Vector3PropertyItem() { }

        public Vector3PropertyItem(EditorValue<Vector3> value, GUIContent label)
            : base(value, label) { }

        public Vector3PropertyItem(EditorValue<Vector3> value, PropertyLabel label)
            : base(value, label) { }

        public Vector3PropertyItem(EditorValue<Vector3> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Vector3 result = EditorGUI.Vector3Field(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }

    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Vector3Int"/>s.
    /// </summary>
    [CustomPropertyItem(typeof(Vector3Int))]
    public class Vector3IntPropertyItem : PropertyValueItem<Vector3Int>
    {
        public Vector3IntPropertyItem() { }

        public Vector3IntPropertyItem(EditorValue<Vector3Int> value, GUIContent label)
            : base(value, label) { }

        public Vector3IntPropertyItem(EditorValue<Vector3Int> value, PropertyLabel label)
            : base(value, label) { }

        public Vector3IntPropertyItem(EditorValue<Vector3Int> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Vector3Int result = EditorGUI.Vector3IntField(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }
    
    /// <summary>
    /// A <see cref="PropertyValueItem{T}"/> for <see cref="Vector4"/>.
    /// </summary>
    [CustomPropertyItem(typeof(Vector4))]
    public class Vector4PropertyItem : PropertyValueItem<Vector4>
    {
        public Vector4PropertyItem() { }

        public Vector4PropertyItem(EditorValue<Vector4> value, GUIContent label)
            : base(value, label) { }

        public Vector4PropertyItem(EditorValue<Vector4> value, PropertyLabel label)
            : base(value, label) { }

        public Vector4PropertyItem(EditorValue<Vector4> value, bool bMakeLabelFromValue = true)
            : base(value, bMakeLabelFromValue) { }

        protected override object OnDraw(ref Rect drawRect)
        {
            Vector4 result = EditorGUI.Vector4Field(drawRect, Label.ConditionalLabel, Value.GetCurrentValue());
            Value.SetLatestResult(result);
            return result;
        }
    }
}
#endif