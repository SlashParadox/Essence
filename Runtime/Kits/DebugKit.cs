using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
