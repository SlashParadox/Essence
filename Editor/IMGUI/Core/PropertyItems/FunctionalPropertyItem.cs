using System;
using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    public sealed class InvokePropertyItem : PropertyItem
    {
        public delegate void PropertyDelegate(ref Rect rect, SerializedProperty property);
        
        private readonly PropertyDelegate _propertyAction;

        private readonly SerializedProperty _trackedProperty;

        public InvokePropertyItem(PropertyDelegate action) : this(null, action, null, null) { }
        
        public InvokePropertyItem(SerializedProperty property, PropertyDelegate action) : this(property, action, null, null) { }

        public InvokePropertyItem(SerializedProperty property, PropertyDelegate action, PropertyLabel label) : base(label)
        {
            _trackedProperty = property;
            _propertyAction = action;
        }

        public InvokePropertyItem(SerializedProperty property, PropertyDelegate action, GUIContent label, GUIStyle overrideStyle = null) : base(label, overrideStyle)
        {
            _trackedProperty = property;
            _propertyAction = action;
        }
        
        protected override object OnDraw(ref Rect drawRect)
        {
            _propertyAction?.Invoke(ref drawRect, _trackedProperty);
            return null;
        }
    }
    
    public sealed class InvokePropertyValueItem<T> : PropertyValueItem<T>
    {
        public delegate T ValueDelegate(ref Rect rect, EditorValue<T> value);
        
        private readonly ValueDelegate _propertyAction;

        public InvokePropertyValueItem(ValueDelegate action) : this(null, action, GUIContent.none) { }
        
        public InvokePropertyValueItem(EditorValue<T> value, ValueDelegate action) : this(value, action, GUIContent.none) { }

        public InvokePropertyValueItem(EditorValue<T> value, ValueDelegate action, PropertyLabel label) : base(value, label)
        {
            _propertyAction = action;
        }

        public InvokePropertyValueItem(EditorValue<T> value, ValueDelegate action, GUIContent label) : base(value, label)
        {
            _propertyAction = action;
        }
        
        protected override object OnDraw(ref Rect drawRect)
        {
            return _propertyAction != null ? _propertyAction.Invoke(ref drawRect, Value) : default(T);
        }
    }
    
    
    public class FunctionalPropertyItem<T> : PropertyItem
    {
        private Func<Rect, T> _drawFunc;
        
        public FunctionalPropertyItem(Func<Rect, T> drawFunc)
        {
            _drawFunc = drawFunc;
        }

        protected override object OnDraw(ref Rect drawRect)
        {
            return _drawFunc != null ? _drawFunc.Invoke(drawRect) : null;
        }
    }
}
