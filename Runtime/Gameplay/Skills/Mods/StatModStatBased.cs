using SlashParadox.Essence.Gameplay.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [System.Serializable]
    public class StatModStatBased : StatMod
    {
        [SerializeField] protected CapturedStat stat;

        public override float CalculateMagnitude(StatModContext context)
        {
            context.GetStatModSpec<StatModSpec>().FindCapturedStatValue(in stat, out float value);
            return value;
        }

        public override List<CapturedStat> GatherRequiredStats()
        {
            return new List<CapturedStat>() { stat };
        }
    }
}
