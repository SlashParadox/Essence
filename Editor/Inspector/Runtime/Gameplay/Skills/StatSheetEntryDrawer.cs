// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Gameplay.Skills;
using UnityEditor;

namespace SlashParadox.Essence.Editor
{
    /// <summary>
    /// A <see cref="PropertyDrawer"/> for a <see cref="StatSheetEntry"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(StatSheetEntry))] [VisualTreePath]
    public class StatSheetEntryDrawer : EssencePropertyDrawer { }
}
