// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using System;
using UnityEngine;

namespace SlashParadox.Essence.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPropertyItemAttribute : PropertyAttribute
    {
        public Type RegisteredType { get; private set; }
        
        public bool UseForChildren { get; private set; }

        public CustomPropertyItemAttribute(Type type, bool useForChildren = false)
        {
            RegisteredType = type;
            UseForChildren = useForChildren;
        }
    }
}
#endif