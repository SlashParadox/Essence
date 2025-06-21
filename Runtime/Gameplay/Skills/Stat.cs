// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// A basic object that represents a stat in the skill system. This is completely empty under
    /// normal circumstances, used to reference the relevant <see cref="ActiveStat"/> during gameplay.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStat", menuName = "Essence/Gameplay/Skills/Stat")]
    public class Stat : EssenceScriptableObject
    {
        /// <summary>An optional parent <see cref="Stat"/>. If set, the base value is affected by the parent's current value.</summary>
        [SerializeField] private Stat parent;

        /// <summary>See: <see cref="parent"/></summary>
        public Stat Parent { get { return parent; } }
    }

    /// <summary>
    /// An instance of a <see cref="Stat"/>. This contains all active data on the stat, and is what is directly referenced in a skill system.
    /// </summary>
    public sealed class ActiveStat
    {
        public delegate void ActiveStatDelegate(ActiveStat stat);

        /// <summary>An event called when the stat's value updates. This is called for either <see cref="BaseValue"/> or <see cref="CurrentValue"/>.</summary>
        public event ActiveStatDelegate StatUpdatedEvent;

        /// <summary>An aggregator for <see cref="StatMod"/>s that change the <see cref="CurrentValue"/>.</summary>
        private readonly StatModAggregator _modAggregator = new StatModAggregator();

        /// <summary>The absolute range of values the stat can be. This is used to clamp both the <see cref="BaseValue"/> and <see cref="CurrentValue"/>.</summary>
        private Vector2? _range;

        /// <summary>The <see cref="ActiveStat"/> of the <see cref="OwningStat"/>'s parent.</summary>
        private ActiveStat _parentStat;

        /// <summary>An optional <see cref="ActiveStat"/> that affects this stat from anywhere. This can even be a stat in a different system.</summary>
        private ActiveStat _rootStat;

        /// <summary>The internal base value, without any parental modification.</summary>
        private float _internalBaseValue;

        /// <summary>The originating <see cref="Stat"/>.</summary>
        public Stat OwningStat { get; private set; }

        /// <summary>The <see cref="StatsSystem"/> that holds the <see cref="Stat"/>.</summary>
        public StatsSystem OwningSystem { get; private set; }

        /// <summary>The current, temporary value of the stat.</summary>
        public float CurrentValue { get; private set; }

        /// <summary>The base, permanent value of the stat.</summary>
        public float BaseValue { get; private set; }

        public ActiveStat(StatsSystem owner, Stat stat)
        {
            OwningSystem = owner;
            OwningStat = stat;
            _range = null;

            _modAggregator.OnDirtyDelegate += OnCurrentAggregatorDirty;
        }

        /// <summary>
        /// Checks if the <see cref="ActiveStat"/> is initialized.
        /// </summary>
        /// <returns>Returns if the stat is initialized. Stats are initialized from <see cref="StatSheetEntry"/>s.</returns>
        public bool IsInitialized()
        {
            return _range != null;
        }

        /// <summary>
        /// Initializes the stat from a <see cref="StatSheetEntry"/>. Only called by the <see cref="SkillsSystem"/>.
        /// </summary>
        /// <param name="entry">The <see cref="StatSheetEntry"/> to use. It must match the <see cref="OwningStat"/>.</param>
        public void InitializeFromSheet(StatSheetEntry entry)
        {
            if (entry == null)
                return;

            if (OwningStat && OwningStat != entry.stat)
                return;

            OwningStat = entry.stat;
            _range = entry.range;

            _internalBaseValue = System.Math.Clamp(entry.initialValue, _range.Value.x, _range.Value.y);
            RecalculateBaseValue();
        }

        public void ApplyModAggregator(StatModAggregator aggregator)
        {
            if (aggregator == null)
                return;

            float value = aggregator.CalculateValue(this);

            // TODO - Clamp the value based on the external class (attributeset)

            if (_range != null)
                value = System.Math.Clamp(value, _range.Value.x, _range.Value.y);

            _internalBaseValue = value;
            RecalculateBaseValue();
        }

        /// <summary>
        /// Recalculates the <see cref="BaseValue"/>. If it updates, <see cref="CurrentValue"/> is also updated.
        /// </summary>
        private void RecalculateBaseValue()
        {
            float baseValue = BaseValue;

            // TODO - Manipulate with parent and root.
            BaseValue = _internalBaseValue;

            if (!Mathf.Approximately(BaseValue, baseValue))
                RecalculateCurrentValue();
        }

        /// <summary>
        /// Recalculates the <see cref="CurrentValue"/>.
        /// </summary>
        private void RecalculateCurrentValue()
        {
            float currentValue = CurrentValue;

            CurrentValue = _modAggregator.CalculateValue(this);

            if (!Mathf.Approximately(CurrentValue, currentValue))
                StatUpdatedEvent?.Invoke(this);
        }

        internal StatModAggregator GetCurrentValueAggregator()
        {
            return _modAggregator;
        }

        private void OnCurrentAggregatorDirty()
        {
            RecalculateCurrentValue();
        }
    }
}
