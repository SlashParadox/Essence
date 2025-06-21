// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
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