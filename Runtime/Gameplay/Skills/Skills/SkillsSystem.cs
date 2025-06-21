// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger Skills = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.Gameplay.Skills
{
    public class SkillsSystem : StatsSystem
    {
        /// <summary>A map of all <see cref="ActiveSkillEffect"/>s to their <see cref="ID.Handle"/>. Stores any mods actively applied to stats.</summary>
        private readonly Dictionary<ID.Handle, ActiveSkillEffect> _activeEffects = new Dictionary<ID.Handle, ActiveSkillEffect>();

        /// <summary>A <see cref="ID.Handle"/> generator for <see cref="_activeEffects"/>.</summary>
        private readonly ID.Generator _activeEffectGenerator = new ID.Generator();

        /// <summary>
        /// Applies the given <see cref="SkillEffectContext"/> to its target <see cref="SkillsSystem"/>.
        /// </summary>
        /// <param name="context">The <see cref="SkillEffectContext"/> to apply.</param>
        /// <returns>Returns the <see cref="ID.Handle"/> of the applied effect.</returns>
        /// <remarks>If no <see cref="SkillEffectContext.SourceSystem"/> or <see cref="SkillEffectContext.TargetSystem"/> is set, it defaults to this system.
        /// <see cref="SkillEffect"/>s can only apply to <see cref="SkillsSystem"/>s, not <see cref="StatsSystem"/>.</remarks>
        public ID.Handle ApplySkillEffect(SkillEffectContext context)
        {
            if (context == null || !(SkillEffect)context.Owner)
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffect), "Passed in effect context is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            // Default the system.
            if (!context.TargetSystem)
                context.SetTargetSystem(this);

            return ApplySkillEffectToTarget(context, context.TargetSystem as SkillsSystem);
        }

        /// <summary>
        /// Applies the given <see cref="SkillEffectContext"/> to the given target <see cref="SkillsSystem"/>.
        /// </summary>
        /// <param name="context">The <see cref="SkillEffectContext"/> to apply.</param>
        /// <param name="target">The <see cref="SkillsSystem"/> to apply the effect to.</param>
        /// <returns>Returns the <see cref="ID.Handle"/> of the applied effect.</returns>
        /// <remarks>If no <see cref="SkillEffectContext.SourceSystem"/> is set, it defaults to this system.
        /// <see cref="SkillEffect"/>s can only apply to <see cref="SkillsSystem"/>s, not <see cref="StatsSystem"/>.</remarks>
        public ID.Handle ApplySkillEffectToTarget(SkillEffectContext context, SkillsSystem target)
        {
            if (context == null || !(SkillEffect)context.Owner)
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToTarget), "Passed in effect context is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            if (!target)
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToTarget), "Passed in effect target is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            // Default the system.
            if (!context.SourceSystem)
                context.SetSourceSystem(this);

            context.SetTargetSystem(target);

            return target.ApplySkillEffectToSelf(context);
        }

        /// <summary>
        /// Applies the given <see cref="SkillEffectContext"/> to this <see cref="SkillsSystem"/>.
        /// </summary>
        /// <param name="context">The <see cref="SkillEffectContext"/> to apply.</param>
        /// <returns>Returns the <see cref="ID.Handle"/> of the applied effect.</returns>
        /// <remarks>If no <see cref="SkillEffectContext.SourceSystem"/> or <see cref="SkillEffectContext.TargetSystem"/> is set, it defaults to this system.
        /// <see cref="SkillEffect"/>s can only apply to <see cref="SkillsSystem"/>s, not <see cref="StatsSystem"/>.</remarks>
        public ID.Handle ApplySkillEffectToSelf(SkillEffectContext context)
        {
            if (context == null)
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToSelf), $"Passed in effect context is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            SkillEffect effect = context?.Owner as SkillEffect;
            if (!effect || !effect.IsValid())
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToSelf), $"Passed in effect context (Owner [{context.Owner}]) is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            // Deep-copy the context to ensure it cannot be updated later.
            SkillEffectContext newContext = context.DeepCopy();
            if (newContext == null)
                return ID.Handle.InvalidHandle;

            // Ensure that a mod spec was created. If one was already set, it was deep-copied.
            StatModSpec effectSpec = context.GetStatModSpec<StatModSpec>();
            if (effectSpec == null)
            {
                effectSpec = effect.CreateEffectSpec();
                if (effectSpec == null)
                {
                    LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToSelf), $"Skill Effect {effect.name} failed to create a spec!", this);
                    return ID.Handle.InvalidHandle;
                }
            }

            // Ensure system sources are set.
            newContext.SetTargetSystem(this);
            if (!newContext.SourceSystem)
                newContext.SetSourceSystem(this);

            if (!newContext.IsValid())
            {
                LogKit.Log(EssenceLog.Skills, LogType.Error, nameof(ApplySkillEffectToSelf), $"Passed in effect context (Owner [{context?.Owner}]) is invalid!", this);
                return ID.Handle.InvalidHandle;
            }

            // Create an active effect container. This registers the effect.
            ActiveSkillEffect activeEffect = new ActiveSkillEffect();
            activeEffect.Context = newContext;
            activeEffect.Spec = effectSpec;
            activeEffect.ModeInstance = effect.CreateEffectMode();
            activeEffect.EffectHandle = _activeEffectGenerator.GetID();
            newContext.EffectHandle = activeEffect.EffectHandle;

            _activeEffects.Add(activeEffect.EffectHandle, activeEffect);

            // Handle modifier aggregation now.
            AggregateEffectMods(activeEffect);

            // The effect has been applied. Inform the effect mode, which might execute immediately.
            effect.OnSkillEffectApplied(activeEffect.Context, activeEffect.Spec);
            activeEffect.ModeInstance.OnSkillEffectApplied(this, activeEffect.EffectHandle);

            // If the effect already finished executing and was removed, don't return anything.
            return activeEffect.IsDisposed ? ID.Handle.InvalidHandle : activeEffect.EffectHandle;
        }

        /// <summary>
        /// Executes an <see cref="ActiveSkillEffect"/>. This only works for <see cref="SkillEffect"/>s that
        /// target the base value.
        /// </summary>
        /// <param name="inHandle">The handle of the effect.</param>
        /// <returns>Returns if the effect was successfully executed.</returns>
        internal bool ExecuteSkillEffect(ID.Handle inHandle)
        {
            if (!_activeEffects.TryGetValue(inHandle, out ActiveSkillEffect activeEffect))
                return false;

            // Current-value mods do not execute, as they are always applied and update only when needed.
            if (activeEffect.ModeInstance.GetTargetedValue() != SkillEffectTargetValue.Base)
                return false;

            SkillEffect effect = activeEffect.Context.Owner as SkillEffect;
            if (!effect)
                return false;

            effect.OnSkillEffectPreExecuted(activeEffect.Context, activeEffect.Spec);

            Dictionary<Stat, StatModAggregator> aggregators = activeEffect.Spec.GetModAggregators();
            foreach (KeyValuePair<Stat, StatModAggregator> aggregator in aggregators)
            {
                activeEffect.Context.TargetSystem.ApplyModAggregatorToStat(aggregator.Key, aggregator.Value);
            }

            effect.OnSkillEffectPostExecuted(activeEffect.Context, activeEffect.Spec);
            activeEffect.ModeInstance?.OnSkillEffectExecuted(this);

            return true;
        }

        public StatModSpec FindEffectSpecByHandle(ID.Handle inHandle)
        {
            _activeEffects.TryGetValue(inHandle, out ActiveSkillEffect value);
            return value.Spec;
        }

        public bool RemoveActiveSkillEffectByHandle(ID.Handle inHandle)
        {
            if (!_activeEffects.TryGetValue(inHandle, out ActiveSkillEffect activeEffect))
                return false;

            _activeEffects.Remove(inHandle);

            SkillEffect effect = activeEffect.Context.Owner as SkillEffect;
            if (!effect)
                return false;

            activeEffect.Dispose();

            return true;
        }

        /// <summary>
        /// Takes an <see cref="ActiveSkillEffect"/> and positions its <see cref="StatMod"/>s into the appropriate <see cref="StatModAggregator"/>s.
        /// </summary>
        /// <param name="activeEffect">The <see cref="ActiveSkillEffect"/> to set up.</param>
        private void AggregateEffectMods(ActiveSkillEffect activeEffect)
        {
            SkillEffect effect = activeEffect.Context.Owner as SkillEffect;
            if (!effect)
                return;

            // Base value mods aggregate to the spec. Current value mods aggregate to the stat.
            bool isBaseValue = activeEffect.ModeInstance.GetTargetedValue() == SkillEffectTargetValue.Base;
            ref readonly List<SkillEffectModItem> modifiers = ref effect.GetStatusModifiers();

            foreach (SkillEffectModItem modifier in modifiers)
            {
                ActiveStat stat = FindActiveStat(modifier.GetTargetStat());
                if (stat == null)
                    continue;

                StatMod mod = modifier.GetStatMod();
                if (mod == null)
                    continue;

                StatModAggregator aggregator = isBaseValue ? activeEffect.Spec.GetOrAddModAggregator(stat.OwningStat) : stat.GetCurrentValueAggregator();
                ID.SharedHandle handle = aggregator?.AddNewMod(modifier.GetStatMod(), activeEffect.Context);

                // Only current value mods need their handles tracked. Base value mods all get cleared at the same time.
                if (!isBaseValue && handle != null)
                    activeEffect.AddActiveModHandle(modifier.GetTargetStat(), handle);
            }
        }
    }
}
