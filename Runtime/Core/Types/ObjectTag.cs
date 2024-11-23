// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    [Serializable]
    public struct ObjectTag : IEquatable<ObjectTag>
    {
        /// <summary>An empty tag.</summary>
        public static ObjectTag None = new ObjectTag(string.Empty);

        /// <summary>The string ID of the tag. This is registered to the tag settings.</summary>
        [SerializeField] internal string tag;
        
        /// <summary>Returns if the tag is equivalent to <see cref="ObjectTag.None"/>.</summary>
        public bool IsNone { get { return this == None || tag == null; } }
        
        /// <summary>Checks if the tag is registered and valid.</summary>
        public bool IsValid { get { return EssenceTagSettings.DoesTagExist(tag);}}

        /// <summary>
        /// Finds the registered tag, if it exists. Check <see cref="ObjectTag.IsValid"/> afterwards.
        /// </summary>
        /// <param name="tag">The tag to find.</param>
        /// <returns>Returns the found tag. Returns <see cref="ObjectTag.None"/> if not found.</returns>
        public static ObjectTag FindTag(string tag)
        {
            EssenceTagSettings.FindTag(tag, out ObjectTag outTag);
            return outTag;
        }

        internal ObjectTag(string tagName)
        {
            tag = tagName;
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectTag other && Equals(other);
        }

        public override int GetHashCode()
        {
            return tag != null ? tag.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return tag;
        }

        public bool Equals(ObjectTag other)
        {
            return tag == other.tag;
        }

        public static bool operator ==(ObjectTag a, ObjectTag b)
        {
            return a.tag == b.tag;
        }

        public static bool operator !=(ObjectTag a, ObjectTag b)
        {
            return !(a == b);
        }

        public readonly bool IsChildOf(ObjectTag inTag, bool isDirectChild = true)
        {
            return EssenceTagSettings.IsTagChildOfTag(tag, inTag.tag, isDirectChild);
        }

        public ObjectTagGroup ToGroup()
        {
            ObjectTagGroup group = new ObjectTagGroup();
            group.AddTag(this);
            return group;
        }
    }

    [Serializable]
    public struct ObjectTagGroup
    {
        [SerializeField] internal List<ObjectTag> tags;

        public List<ObjectTag> GetTags()
        {
            List<ObjectTag> returnedTags = new List<ObjectTag>();
            
            if (tags != null)
                returnedTags.AddRange(tags);
            return returnedTags;
        }

        public bool HasTag(ObjectTag inTag, bool checkAsParent = false, bool checkAsDirectParent = false)
        {
            if (tags == null)
                return false;
            
            if (tags.Contains(inTag))
                return true;

            if (!checkAsParent)
                return false;

            foreach (ObjectTag tag in tags)
            {
                if (EssenceTagSettings.IsTagChildOfTag(tag.tag, inTag.tag, checkAsDirectParent))
                    return true;
            }

            return false;
        }

        public void AddTag(ObjectTag inTag)
        {
            tags ??= new List<ObjectTag>();

            tags.AddUnique(inTag);
        }

        public void AddTags(List<ObjectTag> inTags)
        {
            tags ??= new List<ObjectTag>();

            if (inTags == null)
                return;

            foreach (ObjectTag inTag in inTags)
            {
                tags.AddUnique(inTag);
            }
        }

        public void RemoveTag(ObjectTag inTag, bool removeChildren = false)
        {
            if (tags == null)
                return;
            
            tags.RemoveSingleSwap(inTag);

            if (!removeChildren)
                return;

            for (int i = 0; i < tags.Count; ++i)
            {
                if (!EssenceTagSettings.IsTagChildOfTag(tags[i].tag, inTag.tag, false))
                    continue;

                tags.RemoveAtSwap(i--);
            }
        }
        
#if UNITY_EDITOR
        public void SortTags()
        {
            if (tags.IsEmptyOrNull())
                return;
            
            tags.Sort(((a, b) => String.Compare(a.tag, b.tag, StringComparison.Ordinal)));
        }
#endif
    }
}