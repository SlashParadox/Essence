using SlashParadox.Essence.Gameplay.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// Data on a <see cref="StatMod"/> that is actively applied to a <see cref="StatsSystem"/>.
    /// </summary>
    public class StatModSpec : IDisposable
    {
        /// <summary>All data on <see cref="CapturedStat"/>s. These can be used to find stat values from other <see cref="StatsSystem"/>s.</summary>
        private readonly List<CapturedStatData> _capturedStats = new List<CapturedStatData>();

        /// <summary><see cref="StatModAggregator"/> that apply to the base value of <see cref="Stat"/>s.</summary>
        private readonly Dictionary<Stat, StatModAggregator> _baseModAggregators = new Dictionary<Stat, StatModAggregator>();

        /// <summary>
        /// Adds a stat to capture and grabs its current value. This only captures a stat if it has NOT been captured yet.
        /// </summary>
        /// <param name="stat">The data of the stat to capture and from where.</param>
        /// <param name="allowUpdates">If false, this forces all captured stats to be static. Usually this way for instant modifications.</param>
        /// <returns>Returns the <paramref name="stat"/>'s data. Bindings can be done here.</returns>
        public CapturedStatData AddCapturedStat(in CapturedStat stat, bool allowUpdates = true)
        {
            if (!stat.OriginSystem || stat.OriginSystem.FindActiveStat(stat.targetStat) == null)
                return null;

            CapturedStatData data = FindCapturedStat(in stat);
            if (data != null)
                return data;

            data = new CapturedStatData(stat, allowUpdates);
            _capturedStats.Add(data);

            return data;
        }

        internal CapturedStatData FindCapturedStat(in CapturedStat stat)
        {
            foreach (CapturedStatData statData in _capturedStats)
            {
                if (statData != null && statData.BackingStat == stat)
                    return statData;
            }

            return null;
        }

        public bool FindCapturedStatValue(in CapturedStat stat, out float value)
        {
            value = 0.0f;

            CapturedStatData data = FindCapturedStat(in stat);
            if (data == null)
                return false;

            value = data.CapturedValue;
            return true;

        }

        /// <summary>
        /// Updates the captured value of the given <see cref="ActiveStat"/>, if it was ever requested to be captured.
        /// </summary>
        /// <param name="stat">The stat to update.</param>
        /// <remarks>This will update both a base and curren value capture, but only if they were requested at all.</remarks>
        public void UpdateCapturedStat(ActiveStat stat)
        {
            // Build out a captured stat. We only are updating anything that CAN have an update, and we may as well update both values.
            CapturedStat capturedStat = new CapturedStat();
            capturedStat.OriginSystem = stat.OwningSystem;
            capturedStat.targetStat = stat.OwningStat;
            capturedStat.allowUpdates = true;
        }

        public ref readonly Dictionary<Stat, StatModAggregator> GetModAggregators()
        {
            return ref _baseModAggregators;
        }

        internal StatModAggregator GetOrAddModAggregator(Stat inStat)
        {
            if (!inStat)
                return null;

            _baseModAggregators.TryGetValue(inStat, out StatModAggregator aggregator);
            if (aggregator != null)
                return aggregator;

            _baseModAggregators.Add(inStat, new StatModAggregator());
            return _baseModAggregators[inStat];
        }

        public void Dispose()
        {
            foreach (CapturedStatData statData in _capturedStats)
            {
                statData?.Cleanup();
            }

            _baseModAggregators.Clear();
            _capturedStats.Clear();


        }
    }
}
