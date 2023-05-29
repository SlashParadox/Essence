// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A wrapper to make it easy to display a specific piece of data in the editor.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    public class EditorValue<T>
    {
        /// <summary>If true, setting a result value automatically sets the value via C# Reflection.</summary>
        public bool AutoReflectValue;

        /// <summary>The target object for reflection, which owns the value.</summary>
        private object _targetObject;

        /// <summary>The path to the represented property value.</summary>
        private string[] _propertyPath;

        /// <summary>The innermost target object for reflection, which owns the value.</summary>
        private object _endPathObject;

        /// <summary>The <see cref="MemberInfo"/> of the property value. An alternative if you don't have the path.</summary>
        private MemberInfo _memberInfo;

        /// <summary>A general value to use when not representing a serializable property.</summary>
        private T _nonSerializedValue;

        /// <summary>The last result set to this value. Not necessarily the same as the currently serialized value.</summary>
        public T LastResult { get; private set; }

        /// <summary>An optional <see cref="SerializedProperty"/> to be tracking.</summary>
        public SerializedProperty SProperty { get; private set; }

        public EditorValue(EditorValue<object> genericValue)
        {
            System.Type genericType = genericValue.GetVariableType();
            if (genericType == null || !genericType.IsSubclassOrImplements(typeof(T)))
                return;

            object baseValue = genericValue.GetCurrentValue();
            AutoReflectValue = genericValue.AutoReflectValue;
            _nonSerializedValue = (T)(baseValue ?? default(T));

            // Apply remaining data as normal.
            if (genericValue.SProperty != null)
            {
                SetSerializedProperty(genericValue.SProperty);
            }
            else if (genericValue._targetObject != null)
            {
                if (genericValue._propertyPath.IsNotEmptyOrNull())
                    SetReflectionData(genericValue._targetObject, genericValue._propertyPath);
                else if (genericValue._memberInfo != null)
                    SetReflectionData(genericValue._targetObject, genericValue._memberInfo);
            }
        }

        public EditorValue(bool autoReflect = true, T nonSerializedValue = default)
        {
            AutoReflectValue = autoReflect;
            SetNonSerializedValue(nonSerializedValue);
        }

        public EditorValue(bool autoReflect, SerializedProperty property)
        {
            AutoReflectValue = autoReflect;
            SetSerializedProperty(property);
        }

        public EditorValue(bool autoReflect, object target, params string[] path)
        {
            AutoReflectValue = autoReflect;
            SetReflectionData(target, path);
        }

        public EditorValue(bool autoReflect, object target, MemberInfo memberInfo)
        {
            AutoReflectValue = autoReflect;
            SetReflectionData(target, memberInfo);
        }

        /// <summary>
        /// Sets the <see cref="SProperty"/>. Clears out any old reflection data.
        /// </summary>
        /// <param name="property">The <see cref="SerializedProperty"/> to track.</param>
        public void SetSerializedProperty(SerializedProperty property)
        {
            SetReflectionData(property?.serializedObject.targetObject, ReflectionKit.BreakReflectionPath(property?.propertyPath));
            SProperty = property;
        }

        /// <summary>
        /// Sets the reflection data to track. Clears out any old reflection data.
        /// </summary>
        /// <param name="target">The object that owns the value.</param>
        /// <param name="path">The path to the target value.</param>
        public void SetReflectionData(object target, params string[] path)
        {
            _targetObject = target;
            _propertyPath = target != null ? path : null;

            object startObj = _targetObject;
            _memberInfo = ReflectionKit.GetVariableInfo(ref startObj, out _endPathObject, ReflectionKit.DefaultFlags, _propertyPath);
            _endPathObject ??= _targetObject;

            _nonSerializedValue = default;
            SProperty = null;
            SetLatestResult(GetCurrentValue());
        }

        /// <summary>
        /// Sets the reflection data to track. Clears out any old reflection data.
        /// </summary>
        /// <param name="target">The object that owns the value.</param>
        /// <param name="memberInfo">The <see cref="MemberInfo"/> of the value.</param>
        public void SetReflectionData(object target, MemberInfo memberInfo)
        {
            _targetObject = target;
            _endPathObject = _targetObject;
            _memberInfo = memberInfo;
            _propertyPath = null;
            _nonSerializedValue = default;
            SProperty = null;
            SetLatestResult(GetCurrentValue());
        }

        /// <summary>
        /// Sets the non-serialized value to use, meaning no reflection data is used.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public void SetNonSerializedValue(T value)
        {
            SetSerializedProperty(null);
            _nonSerializedValue = value;
            SetLatestResult(_nonSerializedValue);
        }

        /// <summary>
        /// Gets the current reflected or non-serialized value. This is not necessarily the <see cref="LastResult"/>.
        /// </summary>
        /// <returns>Returns the current value this <see cref="EditorValue{T}"/> represents.</returns>
        public T GetCurrentValue()
        {
            if (_memberInfo != null)
            {
                return _memberInfo switch
                {
                    FieldInfo field => (T)field.GetValue(_endPathObject),
                    PropertyInfo property => (T)property.GetValue(_endPathObject),
                    _ => _nonSerializedValue
                };
            }

            if (_endPathObject != null && _propertyPath.IsNotEmptyOrNull())
            {
                if (ReflectionKit.GetVariableValue(_endPathObject, out T value, _propertyPath))
                    return value;
            }

            return _nonSerializedValue;
        }

        /// <summary>
        /// Sets the latest result, possibly automatically reflecting it.
        /// </summary>
        /// <param name="value">The new result value.</param>
        public void SetLatestResult(T value)
        {
            LastResult = value;

            if (AutoReflectValue)
                ApplyLatestResult();
        }

        /// <summary>
        /// Applies the latest result to the current value.
        /// </summary>
        public void ApplyLatestResult()
        {
            if (_memberInfo != null)
            {
                switch (_memberInfo)
                {
                    case FieldInfo field:
                        field.SetValue(_endPathObject, LastResult);
                        return;
                    case PropertyInfo property:
                        property.SetValue(_endPathObject, LastResult);
                        return;
                }
            }

            if (_endPathObject != null && _propertyPath.IsNotEmptyOrNull())
                ReflectionKit.SetVariableValue(_endPathObject, LastResult, _propertyPath);
            else
                _nonSerializedValue = LastResult;
        }

        /// <summary>
        /// Gets the type of the stored variable.
        /// </summary>
        /// <returns>Returns the stored type.</returns>
        public System.Type GetVariableType()
        {
            if (_memberInfo != null)
            {
                switch (_memberInfo)
                {
                    case FieldInfo field:
                        return field.FieldType;
                    case PropertyInfo property:
                        return property.PropertyType;
                }
            }

            object value = GetCurrentValue();
            return value != null ? value.GetType() : typeof(T);
        }

        /// <summary>
        /// Gets the variable's name.
        /// </summary>
        /// <returns>Returns the variable name.</returns>
        public string GetVariableName()
        {
            return _memberInfo != null ? _memberInfo.Name : "Current Value";
        }

        public MemberInfo GetMemberInfo()
        {
            return _memberInfo;
        }
    }
}
#endif