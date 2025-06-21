// Copyright (c) Craig Williams, SlashParadox

using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A kit of utility methods for animations.
    /// </summary>
    public static class AnimKit
    {
        /// <summary>
        /// Sets the <see cref="AnimationUtility.TangentMode"/> of an entire <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="curve">The <see cref="AnimationCurve"/> to modify.</param>
        /// <param name="mode">The new <see cref="AnimationUtility.TangentMode"/>.</param>
        /// <remarks>Only call this after all keyframes have been added to the curve.</remarks>
        public static void SetCurveTangentMode(this AnimationCurve curve, AnimationUtility.TangentMode mode)
        {
            for (int i = 0; i < curve.length; ++i)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
            }
        }
    }
}
