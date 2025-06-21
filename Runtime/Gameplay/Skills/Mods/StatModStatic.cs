using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [System.Serializable]
    public class StatModStatic : StatMod
    {
        [SerializeField] private CurveScaledFloat coefficient;

        public override float CalculateMagnitude(StatModContext context)
        {
            return coefficient.CurrentValue;
        }

        public override List<CapturedStat> GatherRequiredStats()
        {
            return null;
        }

        public override List<CapturedStat> GatherOptionalStats()
        {
            return null;
        }
    }
}
