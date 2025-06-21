// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base class for a curve instance. Instances can either return based on a runtime curve or an actual <see cref="curveObject"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type of the curve.</typeparam>
    /// <typeparam name="TData">The <see cref="CurveDataBase{TValue}"/> type of the object.</typeparam>
    [System.Serializable]
    public abstract class CurveInstanceBase<TValue, TData> where TData : CurveDataBase<TValue>, new()
    {
        /// <summary>The curve data.</summary>
        [SerializeReference] public TData curveData = new TData();

        /// <summary>An optional <see cref="curveObject"/>. If set, this curve is used instead of the runtime variant.</summary>
        public CurveObject<TValue, TData> curveObject;

        /// <summary>
        /// Evaluates the curve and returns a value.
        /// </summary>
        /// <param name="time">The time of the curve (X).</param>
        /// <returns>Returns the appropriate value (Y)</returns>
        public TValue Evaluate(float time)
        {
            if (curveObject)
                return curveObject.Evaluate(time);

            return curveData != null ? curveData.Evaluate(time) : default;
        }
    }
}
