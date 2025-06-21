// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="GatherableAttribute"/> for showing normally unseen properties in the editor
    /// as read-only basic properties.
    /// </summary>
    public class PreviewAttribute : GatherableAttribute
    {
        public PreviewAttribute()
            : base() { }
        
        public PreviewAttribute(int sortOrder, string group = null)
            : base(sortOrder, group) { }
    }
}
#endif