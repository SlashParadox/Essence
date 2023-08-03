// Copyright (c) Craig Williams, SlashParadox

using System;

namespace SlashParadox.Essence.Kits
{
    public static class DebugKit
    {
        /// <summary>
        /// Throws a <see cref="NullReferenceException"/> if true.
        /// </summary>
        /// <param name="statement">The statement to check.</param>
        /// <param name="message">An optional exception message.</param>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="statement"/> is true.</exception>
        public static void ThrowNullIf(bool statement, string message = null)
        {
            if (statement)
                throw new NullReferenceException(message);
        }

        public static bool LogIfFalse(bool statement, string message = null)
        {
            if (statement)
                return true;

#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.LogWarning(message);
#else
            if (!string.IsNullOrEmpty(message))
                System.Diagnostics.Debug.Print(message);
#endif

            return false;
        }
    }
}