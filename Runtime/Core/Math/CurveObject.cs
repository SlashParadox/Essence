using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An instance-able <see cref="CurveDataBase{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type of the curve.</typeparam>
    /// <typeparam name="TData">The <see cref="CurveDataBase{TValue}"/> type of the object.</typeparam>
    public abstract class CurveObject<TValue, TData> : EssenceScriptableObject
        where TData : CurveDataBase<TValue>, new()
    {
        /// <summary>The interior <see cref="CurveDataBase{TValue}"/>.</summary>
        [SerializeReference] private CurveDataBase<TValue> curveData = new TData();

        /// <summary>
        /// Evaluates the curve and returns a value.
        /// </summary>
        /// <param name="time">The time of the curve (X).</param>
        /// <returns>Returns the appropriate value (Y)</returns>
        public TValue Evaluate(float time)
        {
            return curveData != null ? curveData.Evaluate(time) : default;
        }
    }
}
