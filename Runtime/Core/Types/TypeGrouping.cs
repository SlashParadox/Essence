// Copyright (c) Craig Williams, SlashParadox

namespace SlashParadox.Essence
{
    /// <summary>
    /// The different ways to sort options of a <see cref="MemberType"/>.
    /// </summary>
    public enum TypeGrouping
    {
        /// <summary>There is no sorting. Everything is listed as is.</summary>
        None,

        /// <summary>Items are sorted by the class they inherit.</summary>
        ByInheritance,

        /// <summary>Items are sorted by the namespace they're apart of.</summary>
        ByNamespace,

        /// <summary>Items are sorted by type identity (class, struct, interface, enum)</summary>
        ByIdentity
    }
}
