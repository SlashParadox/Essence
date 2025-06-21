// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using UnityEngine;

/*
 * BUGS
 * - Right now, Unity's SerializedReference does not work well with generic properties. This causes issues with specifically references in
 * SerializedObjects. This will produce a warning every time the table's inspector is drawn, and means that reorganizing the dictionary will cause invalid data.
 */

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base class for a table that displays forms of an <see cref="AnimationCurve"/>.
    /// Use this as a base class to solidify types that can store curves for different values.
    /// </summary>
    /// <typeparam name="TValue">The value type of the curve.</typeparam>
    /// <typeparam name="TCurveType">The type of the <see cref="CurveDataBase{TValue}"/>.</typeparam>
    /// <typeparam name="TInstanceType">The type of the <see cref="CurveInstanceBase{TValue,TData}"/>.</typeparam>
    public abstract class CurveTableBase<TValue, TCurveType, TInstanceType> : EssenceScriptableObject
        where TCurveType : CurveDataBase<TValue>, new()
        where TInstanceType : CurveInstanceBase<TValue, TCurveType>, new()
    {
        /// <summary>A <see cref="HashMap{TKey,TValue}"/> for the table's curves.</summary>
        [SerializeField] private HashMap<string, TInstanceType> curves = new HashMap<string, TInstanceType>();

        /// <summary>
        /// Evaluates the desired curve and returns a value.
        /// </summary>
        /// <param name="curveKey">The key of the curve to evaluate.</param>
        /// <param name="time">The time of the curve (X).</param>
        /// <returns>Returns the appropriate value (Y)</returns>
        public TValue Evaluate(string curveKey, float time)
        {
            if (!string.IsNullOrEmpty(curveKey) && curves.TryGetValue(curveKey, out TInstanceType curve))
                return curve.Evaluate(time);

            LogKit.Log(EssenceLog.LogData, LogType.Warning, nameof(Evaluate), $"Could not find curve with key {curveKey}.", this);
            return default;

        }
    }
}
