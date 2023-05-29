// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="GatherableAttribute"/> for showing normally unseen properties in the editor
    /// as read-only basic properties.
    /// </summary>
    public class PreviewAttribute : GatherableAttribute
    {
        public PreviewAttribute(int sortOrder = 0, string group = null)
            : base(sortOrder, group) { }
    }
}
#endif