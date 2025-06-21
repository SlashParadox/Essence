// Copyright (c) Craig Williams, SlashParadox

using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// An entry on a <see cref="StatSheet"/>.
    /// </summary>
    [System.Serializable]
    public class StatSheetEntry
    {
        /// <summary>The <see cref="Stat"/> to add.</summary>
        public Stat stat;

        /// <summary>The initial value of the <see cref="stat"/>.</summary>
        public float initialValue;

        /// <summary>The range of values allowed for the stat. X is the minimum, and Y is the maximum.</summary>
        public Vector2 range = new Vector2(0.0f, 100.0f);
    }

    /// <summary>
    /// Data variable for <see cref="StatSheet"/> initialization.
    /// </summary>
    [System.Serializable]
    public struct StatSheetData
    {
        /// <summary><see cref="Stat"/>s owned by the sheet. These are added before stats from <see cref="parentSheets"/>.</summary>
        [SerializeField] private List<StatSheetEntry> stats;

        /// <summary>Extra <see cref="StatSheet"/>s to also append the data of. These are added in order, after the <see cref="stats"/>. If a <see cref="Stat"/> does not exist, it is added through this sheet.</summary>
        [SerializeField] private List<StatSheet> parentSheets;

        /// <summary>
        /// Appends the stored <see cref="Stat"/>s by creating fresh <see cref="ActiveStat"/>s.
        /// </summary>
        /// <param name="owner">The owning <see cref="StatsSystem"/> manager.</param>
        /// <param name="outStats">The finalized <see cref="Stat"/> container.</param>
        public void AppendActiveStats(StatsSystem owner, Dictionary<Stat, ActiveStat> outStats)
        {
            if (outStats == null || stats == null)
                return;

            // First, append any stats that are new or overriding in this sheet.
            foreach (StatSheetEntry entry in stats)
            {
                TryAddNewStat(entry.stat, entry, owner, outStats);
            }

            if (parentSheets == null)
                return;

            // Second, append any stats only found in parent sheets.
            foreach (StatSheet sheet in parentSheets)
            {
                if (sheet == null)
                    continue;

                sheet.AppendActiveStats(owner, outStats);
            }
        }

        /// <summary>
        /// Recursively appends <see cref="Stat"/>s to a data map.
        /// </summary>
        /// <param name="stat">The <see cref="Stat"/> to add.</param>
        /// <param name="entry">The <see cref="StatSheetEntry"/> to use for initialization. This might not exist when adding a parent stat.</param>
        /// <param name="owner">The owning <see cref="StatsSystem"/> manager.</param>
        /// <param name="outStats">The finalized <see cref="Stat"/> container.</param>
        private void TryAddNewStat(Stat stat, StatSheetEntry entry, StatsSystem owner, Dictionary<Stat, ActiveStat> outStats)
        {
            if (stat == null)
                return;

            if (entry != null && stat != entry.stat)
                return;

            // Try to add a parent stat. Do this ahead of time so that the stat will be guaranteed to exist.
            TryAddNewStat(stat.Parent, null, owner, outStats);

            // If the stat is already in the sheet, try to initialize it if it hasn't been already.
            if (outStats.TryGetValue(stat, out ActiveStat outStat))
            {
                if (!outStat.IsInitialized())
                    outStat.InitializeFromSheet(entry);

                return;
            }

            // Otherwise, add the stat fresh.
            outStats.Add(stat, new ActiveStat(owner, stat));
            outStats[stat].InitializeFromSheet(entry);
        }
    }

    /// <summary>
    /// A container for <see cref="Stat"/>s to add to a <see cref="StatsSystem"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStatSheet", menuName = "Essence/Gameplay/Stats/Stat Sheet")]
    public class StatSheet : EssenceScriptableObject
    {
        /// <summary>The <see cref="Stat"/>s owned by this sheet.</summary>
        [SerializeField] private StatSheetData stats;

        /// <summary>
        /// Appends the stored <see cref="Stat"/>s by creating fresh <see cref="ActiveStat"/>s.
        /// </summary>
        /// <param name="owner">The owning <see cref="StatsSystem"/> manager.</param>
        /// <param name="outStats">The finalized <see cref="Stat"/> container.</param>
        public void AppendActiveStats(StatsSystem owner, Dictionary<Stat, ActiveStat> outStats)
        {
            stats.AppendActiveStats(owner, outStats);
        }
    }
}
