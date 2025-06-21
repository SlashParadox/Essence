// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    public static class ID
    {
        /// <summary>
        /// A basic handle for tracking data or objects.
        /// </summary>
        public struct Handle : IEquatable<Handle>
        {
            /// <summary>The value of an invalid ID.</summary>
            public const ulong InvalidID = 0;

            /// <summary>An always-invalid handle.</summary>
            public static readonly Handle InvalidHandle = new Handle(InvalidID);

            /// <summary>The internal ID of the handle.</summary>
            private ulong _id;

            public Handle(ulong id)
            {
                _id = id;
            }

            /// <summary>
            /// Invalidates the handle. It can no longer be used after this.
            /// </summary>
            public void Invalidate()
            {
                _id = InvalidID;
            }

            /// <summary>
            /// Checks if the handle is valid.
            /// </summary>
            /// <returns>Returns if the handle is valid.</returns>
            public bool IsValid()
            {
                return _id != InvalidID;
            }

            public static bool operator ==(Handle a, Handle b)
            {
                return a._id == b._id;
            }

            public static bool operator !=(Handle a, Handle b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                return obj is Handle handle && Equals(handle);
            }

            public bool Equals(Handle other)
            {
                return _id == other._id;
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }
        }

        /// <summary>
        /// A type of <see cref="Handle"/> that can be shared as a reference. Use this to be
        /// more sure that this handle matches another.
        /// </summary>
        public class SharedHandle
        {
            /// <summary>The internal handle.</summary>
            private readonly Handle _trackedHandle;

            public SharedHandle(Handle inHandle)
            {
                _trackedHandle = inHandle;
            }

            public override int GetHashCode()
            {
                return _trackedHandle.GetHashCode();
            }

            /// <summary>
            /// Checks if the handle is valid.
            /// </summary>
            /// <returns>Returns if the handle is valid.</returns>
            public bool IsValid()
            {
                return _trackedHandle.IsValid();
            }
        }

        /// <summary>
        /// A generator for <see cref="Handle"/>s and <see cref="SharedHandle"/>s.
        /// </summary>
        public class Generator
        {
            /// <summary>The counter for ID values.</summary>
            private ulong _idCounter;

            // ReSharper disable Unity.PerformanceAnalysis
            /// <summary>
            /// Gets a fresh <see cref="Handle"/> and increments the <see cref="_idCounter"/>.
            /// </summary>
            /// <returns>Returns a new <see cref="Handle"/>.</returns>
            public Handle GetID()
            {
                if (++_idCounter == Handle.InvalidID)
                {
                    Debug.Log("Congrats! You've overflowed an ID handle generator! Please touch grass!");
                    ++_idCounter;
                }

                return new Handle(_idCounter);
            }

            /// <summary>
            /// Gets a fresh <see cref="SharedHandle"/>.
            /// </summary>
            /// <returns>Returns a new <see cref="SharedHandle"/>.</returns>
            public SharedHandle GetSharedID()
            {
                return new SharedHandle(GetID());
            }

            /// <summary>
            /// Gets the current <see cref="Handle"/>.
            /// </summary>
            /// <returns>Returns the current <see cref="Handle"/> the generator is on.</returns>
            public Handle GetCurrentID()
            {
                return new Handle(_idCounter);
            }

            /// <summary>
            /// Resets the generator to the default values.
            /// </summary>
            public void ResetGenerator()
            {
                _idCounter = Handle.InvalidID;
            }
        }
    }
}
