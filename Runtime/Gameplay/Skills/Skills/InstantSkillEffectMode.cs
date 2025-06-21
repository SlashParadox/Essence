using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [System.Serializable]
    public sealed class InstantSkillEffectMode : SkillEffectMode
    {
        public override SkillEffectTargetValue GetTargetedValue()
        {
            // Instant effects should always target the base value.
            return SkillEffectTargetValue.Base;
        }

        protected override void OnApplied()
        {
            base.OnApplied();

            RequestExecution();
        }

        protected override void OnExecuted()
        {
            RequestRemoval();
        }
    }
}
