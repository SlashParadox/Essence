// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// A base system for managing <see cref="Stat"/>s. This can be subclassed to make ability systems, but by itself, it can be used for anything that just needs
    /// simple data collection for attributes.
    /// </summary>
    public class StatsSystem : EssenceBehaviour
    {
        /// <summary>Default stats to create. Earlier sheets have priority.</summary>
        [SerializeField] private StatSheetData defaultStats;

        /// <summary>All <see cref="ActiveStat"/>s owned by this system.</summary>
        private readonly Dictionary<Stat, ActiveStat> _ownedStats = new Dictionary<Stat, ActiveStat>();

        protected virtual void Awake()
        {
            defaultStats.AppendActiveStats(this, _ownedStats);
        }

        /// <summary>
        /// Finds an <see cref="ActiveStat"/> based on the given <see cref="Stat"/>.
        /// </summary>
        /// <param name="originalStat">The <see cref="Stat"/> to find.</param>
        /// <returns>Returns the found <see cref="ActiveStat"/>.</returns>
        public ActiveStat FindActiveStat(Stat originalStat)
        {
            _ownedStats.TryGetValue(originalStat, out ActiveStat outStat);
            return outStat;
        }

        /// <summary>
        /// Applies the given <see cref="StatModAggregator"/> to the stat's <see cref="ActiveStat.BaseValue"/>.
        /// </summary>
        /// <param name="stat">The <see cref="Stat"/> to apply to.</param>
        /// <param name="aggregator">The <see cref="StatModAggregator"/> to apply.</param>
        public void ApplyModAggregatorToStat(Stat stat, StatModAggregator aggregator)
        {
            if (aggregator == null)
                return;

            ActiveStat activeStat = FindActiveStat(stat);
            activeStat?.ApplyModAggregator(aggregator);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gets the <see cref="_ownedStats"/>.
        /// </summary>
        /// <returns>Returns the <see cref="_ownedStats"/>.</returns>
        public ref readonly Dictionary<Stat, ActiveStat> GetOwnedStatsInternal()
        {
            return ref _ownedStats;
        }
#endif
    }
}
