using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    public class SkillEffectContext : StatModContext
    {
        public ID.Handle EffectHandle;

        public SkillEffectContext(SkillEffect owningEffect) : base(owningEffect) { }

        public override bool IsValid()
        {
            if (!base.IsValid())
                return false;

            return SourceSystem is SkillsSystem && TargetSystem is SkillsSystem;
        }

        protected override StatModSpec GetUsedModSpec()
        {
            StatModSpec baseSpec = base.GetUsedModSpec();
            if (baseSpec != null)
                return baseSpec;

            SkillsSystem system = TargetSystem as SkillsSystem;
            if (system && EffectHandle.IsValid())
            {
                return system.FindEffectSpecByHandle(EffectHandle);
            }

            return null;
        }
    }
}
