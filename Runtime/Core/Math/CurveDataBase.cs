using UnityEditor;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A base class for information of a type of curve. Curves map some time key to an outputted value.
    /// </summary>
    /// <typeparam name="TValue">The interior value of the curve.</typeparam>
    [System.Serializable]
    public abstract class CurveDataBase<TValue> : ISerializationCallbackReceiver
    {
        /// <summary>If true, the curve uses the <see cref="dataTable"/> to import data. If false, the curve is implied to be freely editable.</summary>
        [SerializeField] private bool tableMode;

        /// <summary>The tangent of the curve, when <see cref="tableMode"/> is true.</summary>
        [SerializeField] private AnimationUtility.TangentMode tableTangent;

        /// <summary>A <see cref="HashMap{TKey,TValue}"/> for the curve's keys. Used in <see cref="tableMode"/>.</summary>
        [SerializeField] private HashMap<float, TValue> dataTable;

        /// <summary>The tangent of the curve, when <see cref="tableMode"/> is true.</summary>
        protected AnimationUtility.TangentMode TableTangentMode { get { return tableTangent; } }

        public virtual void OnBeforeSerialize()
        {
            if (tableMode)
            {
                dataTable ??= new HashMap<float, TValue>();
                DeserializeFromTable(dataTable);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            if (tableMode)
            {
                dataTable ??= new HashMap<float, TValue>();
                DeserializeFromTable(dataTable);
            }
        }

        /// <summary>
        /// Evaluates the curve and returns a value.
        /// </summary>
        /// <param name="time">The time of the curve (X).</param>
        /// <returns>Returns the appropriate value (Y)</returns>
        public abstract TValue Evaluate(float time);

        /// <summary>
        /// Deserializes data from the <see cref="dataTable"/> when in <see cref="tableMode"/>.
        /// </summary>
        /// <param name="table">The <see cref="dataTable"/>.</param>
        protected abstract void DeserializeFromTable(HashMap<float, TValue> table);

        /// <summary>
        /// Serializes data to the <see cref="dataTable"/> when in <see cref="tableMode"/>.
        /// </summary>
        /// <param name="table">The <see cref="dataTable"/>.</param>
        protected abstract void SerializeToTable(HashMap<float, TValue> table);
    }
}
