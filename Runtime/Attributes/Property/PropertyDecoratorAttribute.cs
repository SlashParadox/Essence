// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyDecoratorAttribute : PropertyAttribute
    {
        public PropertyDecoratorAttribute() : base(true)
        {
            order = int.MinValue;
        }
    }
}
