// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A unique identifier. Internally, uses an integral value for fast comparisons over string comparison.
    /// The strings that make up the original data are case-insensitive. Do not rely on getting the same string back that made the value.
    /// </summary>
    [System.Serializable]
    public struct Alias : ISerializationCallbackReceiver, System.IEquatable<Alias>
    {
        public static readonly Alias None = new Alias();

        [SerializeField] private string internalName;

        private ulong _id;

        public Alias(string name)
        {
            internalName = string.Empty;
            _id = LabelManager.RegisterLabel(name);
        }

        public override bool Equals(object obj)
        {
            return obj is Alias other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public bool Equals(Alias other)
        {
            return _id == other._id;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _id = LabelManager.RegisterLabel(internalName);

#if !UNITY_EDITOR
            internalName = string.Empty;
#endif
        }

        public bool IsNone()
        {
            return this == None;
        }

        internal string GetInternalName()
        {
            return internalName;
        }

        public static implicit operator string(Alias alias)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(alias.internalName))
                return alias.internalName;
#endif

            return LabelManager.FindDisplayNameForLabel(alias._id);
        }

        public static explicit operator Alias(string name)
        {
            return new Alias(name);
        }

        public static bool operator ==(Alias a, Alias b)
        {
            return a._id == b._id;
        }

        public static bool operator !=(Alias a, Alias b)
        {
            return !(a == b);
        }
    }

    internal static class LabelManager
    {
        private const ulong InvalidLabelID = 0;

        private static readonly Dictionary<string, ulong> LabelDictionary = new Dictionary<string, ulong>();
        private static readonly Dictionary<ulong, string> IDDictionary = new Dictionary<ulong, string>();

        private static ulong _labelCounter;

        public static ulong RegisterLabel(string labelName)
        {
            if (string.IsNullOrEmpty(labelName))
                return InvalidLabelID;

            string ordinalName = labelName.ToLowerInvariant();
            if (LabelDictionary.TryGetValue(ordinalName, out ulong id))
                return id;

            LabelDictionary.Add(ordinalName, ++_labelCounter);
            IDDictionary.Add(_labelCounter, labelName);
            return _labelCounter;
        }

        public static string FindDisplayNameForLabel(ulong id)
        {
            IDDictionary.TryGetValue(id, out string value);
            return value;
        }
    }
}
