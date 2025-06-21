using Codice.Client.BaseCommands;
using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// Data on a <see cref="StatMod"/> a <see cref="SkillEffect"/> applies to a <see cref="Stat"/>.
    /// </summary>
    [System.Serializable]
    public struct SkillEffectModItem
    {
        /// <summary>The <see cref="Stat"/> to modify.</summary>
        [SerializeField] private Stat targetStat;

        /// <summary>The <see cref="StatMod"/> to apply.</summary>
        [SerializeField] private StatModItem modifier;

        public Stat GetTargetStat()
        {
            return targetStat;
        }

        public StatMod GetStatMod()
        {
            return modifier.GetStatMod();
        }

        public StatModItem GetStatModItem()
        {
            return modifier;
        }
    }

    public class SkillEffect : EssenceScriptableObject
    {
        [SerializeReference] [Instanced] private SkillEffectMode mode;

        [SerializeField] private List<SkillEffectModItem> modifiers;

        public ref readonly List<SkillEffectModItem> GetStatusModifiers()
        {
            return ref modifiers;
        }

        public bool IsValid()
        {
            if (mode == null)
                return false;

            return CheckIsValid();
        }

        protected virtual bool CheckIsValid()
        {
            return true;
        }

        public SkillEffectMode CreateEffectMode()
        {
            return mode?.DeepCopy();
        }

        public virtual SkillEffectSpec CreateEffectSpec()
        {
            return new SkillEffectSpec();
        }

        /// <summary>
        /// An event called when this effect is applied to someone. This should be treated like a static function, and not change any state
        /// on the effect object, as it is not instanced.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="spec"></param>
        internal void OnSkillEffectApplied(SkillEffectContext context, StatModSpec spec)
        {

        }

        internal void OnSkillEffectPreExecuted(SkillEffectContext context, StatModSpec spec)
        {

        }

        internal void OnSkillEffectPostExecuted(SkillEffectContext context, StatModSpec spec)
        {

        }

        internal void OnSkillEffectRemoved(SkillEffectContext context, StatModSpec spec)
        {

        }
    }
}
