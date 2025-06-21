// Copyright (c) Craig Williams, SlashParadox

using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A float that can optionally be scaled to a <see cref="CurveTableFloat"/>.
    /// </summary>
    [Serializable]
    public struct CurveScaledFloat
    {
        /// <summary>The base value. If there is no <see cref="curveTable"/>, this value is returned.</summary>
        public float baseValue;

        /// <summary>The key of the curve in the <see cref="curveTable"/>.</summary>
        public string curveKey;

        /// <summary>An optional <see cref="CurveTableFloat"/> to use. The <see cref="baseValue"/> is used as the time.</summary>
        [SerializeField] private CurveTableFloat curveTable;

        /// <summary>The current value, based on the <see cref="curveTable"/> and <see cref="baseValue"/>.</summary>
        public float CurrentValue { get { return curveTable ? curveTable.Evaluate(curveKey, baseValue) : baseValue; } }

        public CurveScaledFloat(CurveTableFloat table, string key, float value = 0.0f)
        {
            curveTable = table;
            curveKey = key;
            baseValue = value;
        }

        public CurveScaledFloat(float value)
        {
            curveTable = null;
            curveKey = string.Empty;
            baseValue = value;
        }
    }
}
