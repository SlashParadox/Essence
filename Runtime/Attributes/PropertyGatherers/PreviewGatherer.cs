// Copyright (c) Craig Williams, SlashParadox

#if UNITY_64
using System.Reflection;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A <see cref="PropertyGatherer{T}"/> for the <see cref="PreviewAttribute"/>. These are normally unseen
    /// properties that are viewable as basic readonly properties.
    /// </summary>
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class PreviewGatherer : PropertyGatherer<PreviewAttribute>
    {
        public PreviewGatherer(string group = null)
            : base(group) { }

#if UNITY_EDITOR
        protected override bool IsMemberValid(MemberInfo info)
        {
            FieldInfo fieldInfo = info as FieldInfo;
            PropertyInfo propertyInfo = info as PropertyInfo;

            // Variables only.
            if (fieldInfo == null && propertyInfo == null)
                return false;
            
            // We only want to show items not showing in the inspector.
            bool requiresHideAttribute = info.GetCustomAttribute<SerializeField>() != null;
            
            // We don't want to show serialized fields that are visible in inspector
            requiresHideAttribute |= fieldInfo != null && fieldInfo.IsPublic;

            return !requiresHideAttribute || info.GetCustomAttribute<HideInInspector>() != null;
        }
#endif
    }
}
#endif