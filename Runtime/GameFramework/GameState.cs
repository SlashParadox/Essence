// Copyright (c) Craig Williams, SlashParadox

using System;

namespace SlashParadox.Essence.GameFramework
{
    /// <summary>
    /// A simple class representing a state of the <see cref="GameMode"/>. This is used to know how far along in the game mode one is. These tend to be free-form in how you wish
    /// to use them. Primary states are states without a <see cref="Parent"/>, and should progress linearly. Secondary states are states with a parent, and should freely cycle
    /// and change within that parent state.    
    /// </summary>
    public sealed class GameState
    {
        /// <summary>The name of the state.</summary>
        public readonly string Name;

        /// <summary>The level of the state. Not directly used, but can be an option to know if one state is intended to be later than another.</summary>
        public readonly int Level;

        /// <summary>The owning <see cref="GameState"/>. This allows cycling a secondary state within the primary state. You should not have another secondary state as a parent.</summary>
        public readonly GameState Parent;

        internal GameState() { }

        public GameState(int inLevel, string inName)
        {
            Name = inName;
            Level = inLevel;
        }

        public GameState(GameState parent, int inLevel, string inName) : this(inLevel, inName)
        {
            Parent = parent;
        }

        /// <summary>
        /// Checks if this state is or is related to a given <see cref="GameState"/>.
        /// </summary>
        /// <param name="inState">The other <see cref="GameState"/>.</param>
        /// <returns>Returns if this state is or is a child of the <paramref name="inState"/>.</returns>
        public bool IsOrIsSecondaryTo(GameState inState)
        {
            return this == inState || Parent == inState;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is GameState other && Equals(other));
        }

        public override int GetHashCode()
        {
            return Parent != null ? HashCode.Combine(Level, Name, Parent.GetHashCode()) : HashCode.Combine(Level, Name);
        }

        public override string ToString()
        {
            return Parent != null ? $"[{Parent}]::{Name}" : Name;
        }

        private bool Equals(GameState other)
        {
            return Level == other.Level && Name == other.Name;
        }

        public static bool operator >(GameState a, GameState b)
        {
            if (a == null)
                return false;

            if (b != null && a.Parent != b.Parent)
                return a.Parent > b.Parent;

            return b == null || a.Level > b.Level;
        }

        public static bool operator <(GameState a, GameState b)
        {
            if (a == null)
                return false;

            if (b != null && a.Parent != b.Parent)
                return a.Parent < b.Parent;

            return b != null && a.Level < b.Level;
        }

        public static bool operator >=(GameState a, GameState b)
        {
            if (a == null)
                return b == null;

            if (b != null && a.Parent != b.Parent)
                return a.Parent >= b.Parent;

            return b == null || a.Level >= b.Level;
        }

        public static bool operator <=(GameState a, GameState b)
        {
            if (a == null)
                return true;

            if (b != null && a.Parent != b.Parent)
                return a.Parent <= b.Parent;

            return b != null && a.Level <= b.Level;
        }

        public static bool operator ==(GameState a, GameState b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return !ReferenceEquals(b, null) && a.Parent == b.Parent && a.Level == b.Level;
        }

        public static bool operator !=(GameState a, GameState b)
        {
            return !(a == b);
        }
    }
}