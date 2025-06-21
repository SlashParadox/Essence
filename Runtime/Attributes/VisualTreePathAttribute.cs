// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEngine.UIElements;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An attribute for caching the path to a <see cref="VisualTreeAsset"/>.
    /// The <see cref="Path"/> can either be a full path to the asset, the name of the asset, or left blank,
    /// which defaults to the name of the associated class.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Class, Inherited = true)]
    public class VisualTreePathAttribute : Attribute
    {
        /// <summary>The path to (or name of) the <see cref="VisualTreeAsset"/> asset.</summary>
        public string Path { get; }

        public VisualTreePathAttribute()
        {
            Path = string.Empty;
        }

        public VisualTreePathAttribute(string inPath)
        {
            Path = inPath;
        }
    }
}
