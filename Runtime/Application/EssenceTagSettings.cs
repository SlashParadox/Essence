// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using SlashParadox.Essence.Kits;
using UnityEditor;
#endif

namespace SlashParadox.Essence
{
    /// <summary>
    /// A manager for <see cref="ObjectTag"/>s. Tags have their information cached off early upon deserialization. Tags are added at editor time,
    /// and cannot be removed at runtime.
    /// </summary>
    [ProjectSettings(nameof(ProjectSettingsType.Runtime), "Essence Tags", "EssenceTagSettings.asset")]
    public partial class EssenceTagSettings : ProjectSettingsObject<EssenceTagSettings>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// A node structure for information on <see cref="ObjectTag"/>s, broken down into a parenting tree.
        /// </summary>
        private class ObjectTagData : DataTreeNode
        {
            /// <summary>The internal <see cref="ObjectTag"/>.</summary>
            public readonly ObjectTag InternalTag;

            public HashSet<string> CachedParentTags = new HashSet<string>();
            
#if UNITY_EDITOR
            /// <summary>Editor only. Used to know if the tag is displayed.</summary>
            public bool IsFoldoutOpen;
#endif

            public ObjectTagData(ObjectTag internalTag)
            {
                InternalTag = internalTag;
            }

            protected override bool MatchesValue<TValue>(TValue value)
            {
                switch (value)
                {
                    case string s:
                        return InternalTag.tag == s;
                    case ObjectTag tag:
                        return InternalTag.tag == tag.tag;
                    default:
                        return false;
                }
            }

            protected override bool CanAddChild(DataTreeNode inNode)
            {
                return inNode is ObjectTagData node && !node.InternalTag.IsNone;
            }
        }

        /// <summary>If true, the tags are initialized, and can be used for quick lookup.</summary>
        internal static bool IsInitialized { get { return CurrentSingleton && CurrentSingleton._isInitialized; } }

        /// <summary>The serialized list of tags. The tag map and tree are built out of this upon deserialization.</summary>
        [SerializeField] [HideInInspector] private List<string> serializedTags = new List<string>();

        /// <summary>A map of every <see cref="ObjectTag"/> string to its data. Used for quick lookup.</summary>
        private readonly Dictionary<string, ObjectTagData> _tagMap = new Dictionary<string, ObjectTagData>();

        /// <summary>The root <see cref="DataTreeNode"/> for all <see cref="ObjectTagData"/>s. Represented as a null tag.</summary>
        private readonly ObjectTagData _rootTag = new ObjectTagData(ObjectTag.None);

        /// <summary>If true, the tags are initialized, and can be used for quick lookup.</summary>
        private bool _isInitialized;

        public void OnBeforeSerialize()
        {
            serializedTags = _tagMap.Keys.ToList();
        }

        public void OnAfterDeserialize()
        {
            _isInitialized = false;
            
            serializedTags.Sort(string.CompareOrdinal);

            foreach (string tag in serializedTags)
            {
                AddTagToTree(tag);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Adds a new tag to the tree, if it hasn't already been added. Internal and editor use only.
        /// </summary>
        /// <param name="inTag">The tag to add.</param>
        private void AddTagToTree(string inTag)
        {
            string[] tagCategories = inTag.Split('.', StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> cachedParentTags = new HashSet<string>();

            ObjectTag currentTag = new ObjectTag(string.Empty);
            ObjectTagData currentData = _rootTag;
            foreach (string tagPart in tagCategories)
            {
                if (string.IsNullOrEmpty(currentTag.tag))
                    currentTag.tag = tagPart;
                else
                    currentTag.tag += $".{tagPart}";

                if (currentData.FindChild(currentTag) is ObjectTagData foundData)
                {
                    if (!currentData.InternalTag.IsNone)
                        cachedParentTags.Add(currentData.InternalTag.tag);
                    
                    currentData = foundData;
                    continue;
                }

                ObjectTagData newData = new ObjectTagData(currentTag);
                newData.CachedParentTags = cachedParentTags;
                currentData.AddChild(newData);
                currentData = newData;

                _tagMap.Add(currentTag.tag, newData);
            }
        }

        /// <summary>
        /// Removes a tag from the tree, if it exists Removes all children as well. Internal editor use only.
        /// </summary>
        /// <param name="inTag">The tag to remove.</param>
        private void RemoveTagFromTree(string inTag)
        {
            if (_rootTag?.FindChild(inTag) is not ObjectTagData removedNode)
                return;

            // Remove the node, and all of its children.
            removedNode.RemoveFromParent();
            removedNode.IterateChildren<ObjectTagData>(node =>
                                                       {
                                                           _tagMap.Remove(node.InternalTag.tag);
                                                           serializedTags.RemoveSingleSwap(node.InternalTag.tag);
                                                       }, true);

            _tagMap.Remove(inTag);
            serializedTags.RemoveSingleSwap(inTag);
        }

        internal static bool DoesTagExist(string inTag)
        {
            return CurrentSingleton && CurrentSingleton._tagMap.ContainsKey(inTag);
        }

        internal static bool FindTag(string tag, out ObjectTag outTag)
        {
            outTag = ObjectTag.None;
            
            if (!CurrentSingleton || !CurrentSingleton._tagMap.TryGetValue(tag, out ObjectTagData _))
                return false;

            outTag = new ObjectTag(tag);
            return true;
        }

        internal static bool IsTagChildOfTag(string tag, string parent, bool isDirectChild)
        {
            if (!CurrentSingleton)
                return false;

            if (!CurrentSingleton._tagMap.TryGetValue(tag, out ObjectTagData tagData) || tagData == null)
                return false;

            if (!CurrentSingleton._tagMap.TryGetValue(parent, out ObjectTagData parentData) || parentData == null)
                return false;

            bool isChild = tagData.GetParent<ObjectTagData>() == parentData;
            if (isDirectChild || isChild)
                return isChild;

            return tagData.CachedParentTags.Contains(parentData.InternalTag.tag);
        }

#if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider GetProvider()
        {
            return ProjectSettingsProvider.GetProvider(GetOrCreateInstance());
        }
#endif
    }
}