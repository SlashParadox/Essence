// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    public class StatModGroup
    {

        public event StatModDirtyDelegate OnDirtyDelegate;

        public readonly List<ActiveStatMod> Mods = new List<ActiveStatMod>();

        private float? _groupValue;

        public bool IsDirty()
        {
            return _groupValue == null;
        }

        public void UpdateGroupValue(float inValue)
        {
            _groupValue = inValue;
        }

        public float GetGroupValue()
        {
            return _groupValue ?? 0.0f;
        }

        public bool AddMod(ActiveStatMod activeMod)
        {
            if (activeMod == null)
                return false;

            activeMod.InitializeActiveMod();
            activeMod.OnDirtyDelegate += OnStatModDirty;

            int priority = activeMod.Mod.ModPriority;
            for (int i = 0; i < Mods.Count; ++i)
            {
                StatMod mod = Mods[i].Mod;
                if (priority >= mod.ModPriority)
                    continue;

                Mods.Insert(i, activeMod);
                OnStatModDirty();
                return true;
            }

            Mods.Add(activeMod);
            OnStatModDirty();
            return true;
        }

        public bool RemoveModFromGroup(ID.SharedHandle modHandle)
        {
            if (modHandle == null)
                return false;

            for (int i = 0; i < Mods.Count; ++i)
            {
                ActiveStatMod mod = Mods[i];
                if (mod == null || mod.Handle != modHandle)
                    continue;

                Mods.RemoveAt(i);
                OnStatModDirty();
                return true;
            }

            return false;
        }

        private int IndexOfMod(StatMod inMod)
        {
            return Mods.FindIndex(mod => mod.Mod == inMod);
        }

        private void OnStatModDirty()
        {
            _groupValue = null;
            OnDirtyDelegate?.Invoke();
        }
    }

    public class StatModChannel
    {
        public event StatModDirtyDelegate OnDirtyDelegate;

        public readonly Dictionary<StatModType, StatModGroup> modGroups = new Dictionary<StatModType, StatModGroup>();
        public int ChannelIndex;

        private float? _channelMagnitude;

        public StatModChannel(int channel)
        {
            ChannelIndex = channel;

            int count = EnumKit.GetValueCount<StatModType>();
            for (int i = 0; i < count; ++i)
            {
                StatModGroup group = new StatModGroup();
                group.OnDirtyDelegate += OnModGroupDirty;

                modGroups.Add((StatModType)i, group);
            }
        }

        public bool IsDirty()
        {
            return _channelMagnitude == null;
        }

        public bool AddModToChannel(ActiveStatMod activeMod)
        {
            if (activeMod == null)
                return false;

            StatModType modType = activeMod.Mod.ModType;
            return modGroups[modType].AddMod(activeMod);
        }

        public bool RemoveModFromChannel(ID.SharedHandle modHandle)
        {
            foreach (KeyValuePair<StatModType, StatModGroup> group in modGroups)
            {
                if (group.Value.RemoveModFromGroup(modHandle))
                    return true;
            }

            return false;
        }

        public float CalculateValue(float baseValue)
        {
            //if (_channelMagnitude != null)
            //    return _channelMagnitude.Value;

            // First, see if there are any overrides. The highest-priority override always wins.
            if (modGroups[StatModType.Override].Mods.IsNotEmptyOrNull())
                return FindBestOverride(baseValue);

            // Otherwise, proceed with standard calculation.
            float value = (baseValue + AddMods(modGroups[StatModType.Add])) * MultiplyMods(modGroups[StatModType.Multiply]) / MultiplyMods(modGroups[StatModType.Divide]);

            _channelMagnitude = value;
            return value;
        }

        private float FindBestOverride(float baseValue)
        {
            StatModGroup overrideGroup = modGroups[StatModType.Override];
            if (overrideGroup.Mods.IsEmptyOrNull())
                return baseValue;

            if (!overrideGroup.IsDirty())
                return overrideGroup.GetGroupValue();

            float magnitude = overrideGroup.Mods.LastElement().CalculateMagnitude();
            overrideGroup.UpdateGroupValue(magnitude);
            return magnitude;
        }

        private void OnModGroupDirty()
        {
            _channelMagnitude = null;
            OnDirtyDelegate?.Invoke();
        }

        private float AddMods(StatModGroup inGroup)
        {
            float magnitude = 0.0f;

            if (inGroup == null)
                return magnitude;

            if (!inGroup.IsDirty())
                return inGroup.GetGroupValue();

            foreach (ActiveStatMod mod in inGroup.Mods)
            {
                magnitude += mod.CalculateMagnitude();
            }

            inGroup.UpdateGroupValue(magnitude);
            return magnitude;
        }

        private float MultiplyMods(StatModGroup inGroup)
        {
            float magnitude = 1.0f;

            if (inGroup == null)
                return magnitude;

            if (!inGroup.IsDirty())
                return inGroup.GetGroupValue();

            foreach (ActiveStatMod mod in inGroup.Mods)
            {
                magnitude *= mod.CalculateMagnitude();
            }

            inGroup.UpdateGroupValue(magnitude);
            return magnitude;
        }
    }

    public class StatModAggregator
    {
        public event StatModDirtyDelegate OnDirtyDelegate;

        private readonly List<StatModChannel> _modChannels = new List<StatModChannel>();
        private readonly Dictionary<int, StatModChannel> _channelIndexes = new Dictionary<int, StatModChannel>();

        private float? _aggregatorMagnitude;

        private ID.Generator _modHandleGenerator = new ID.Generator();

        /// <summary>
        /// Adds a new <see cref="StatMod"/> to the aggregator.
        /// </summary>
        /// <param name="mod">The <see cref="StatMod"/> to add.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ID.SharedHandle AddNewMod(StatMod mod, StatModContext context)
        {
            if (mod == null || context == null)
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(AddNewMod), "Cannot add invalid mod/mod context to stat mod aggregator!");
                return null;
            }

            // Verify that all required stats can be captured.
            List<CapturedStat> requiredStats = mod.GatherRequiredStats();
            if (requiredStats.IsNotEmptyOrNull())
            {
                foreach (CapturedStat stat in requiredStats)
                {
                    StatsSystem system = stat.OriginSystem;
                    if (!system)
                        system = stat.origin == CapturedStatOrigin.Source ? context.SourceSystem : context.TargetSystem;

                    if (!system)
                        return null;

                    if (system.FindActiveStat(stat.targetStat) == null)
                        return null;
                }
            }

            // Create a new active mod and add it to the appropriate mod channel.
            ActiveStatMod activeMod = new ActiveStatMod(mod, context, _modHandleGenerator.GetSharedID());
            StatModChannel channel = GetOrCreateModChannel(mod.ModChannel);
            if (!channel.AddModToChannel(activeMod))
                return null;

            return activeMod.Handle;
        }

        public float CalculateValue(ActiveStat inStat)
        {
            if (inStat == null)
                return 0.0f;

            // if (_aggregatorMagnitude.HasValue)
            //     return _aggregatorMagnitude.Value;

            float currentValue = inStat.BaseValue;

            // Iterate through all mod channels and apply all mods.
            foreach (StatModChannel channel in _modChannels)
            {
                currentValue = channel.CalculateValue(currentValue);
            }

            _aggregatorMagnitude = currentValue;
            return _aggregatorMagnitude.Value;
        }

        public bool RemoveMod(ID.SharedHandle modHandle)
        {
            // Removes a mod with the given handle.
            foreach (StatModChannel channel in _modChannels)
            {
                if (channel.RemoveModFromChannel(modHandle))
                    return true;
            }

            return false;
        }

        private StatModChannel GetOrCreateModChannel(int index)
        {
            if (_channelIndexes.TryGetValue(index, out StatModChannel modChannel))
                return modChannel;

            StatModChannel channel = new StatModChannel(index);
            channel.OnDirtyDelegate += OnModChannelDirty;

            _channelIndexes.Add(index, channel);

            for (int i = 0; i < _modChannels.Count; ++i)
            {
                if (_modChannels[i].ChannelIndex > index)
                {
                    _modChannels.Insert(i, channel);
                    return channel;
                }
            }

            _modChannels.Add(channel);
            return channel;
        }

        private void OnModChannelDirty()
        {
            _aggregatorMagnitude = null;
            OnDirtyDelegate?.Invoke();
        }
    }
}
