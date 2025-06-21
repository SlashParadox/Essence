
using System;
using System.Collections.Generic;

namespace SlashParadox.Essence.Gameplay.Skills
{
    public class ActiveSkillEffect : IDisposable
    {
        private readonly struct ActiveSkillEffectModHandle
        {
            public readonly Stat TargetStat;

            public readonly ID.SharedHandle Handle;

            public ActiveSkillEffectModHandle(Stat inTarget, ID.SharedHandle inHandle)
            {
                TargetStat = inTarget;
                Handle = inHandle;
            }
        }

        public ID.Handle EffectHandle;

        public SkillEffectContext Context;

        public StatModSpec Spec;

        public SkillEffectMode ModeInstance;

        public bool IsDisposed { get; private set; }

        private readonly List<ActiveSkillEffectModHandle> _activeModHandles = new List<ActiveSkillEffectModHandle>();

        public void AddActiveModHandle(Stat target, ID.SharedHandle handle)
        {
            if (target == null || handle == null)
                return;

            _activeModHandles.Add(new ActiveSkillEffectModHandle(target, handle));
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            OnDispose();

            ModeInstance?.OnSkillEffectRemoved(Context.TargetSystem as SkillsSystem);

            if (Context.TargetSystem)
            {
                foreach (ActiveSkillEffectModHandle modHandle in _activeModHandles)
                {
                    ActiveStat stat = Context.TargetSystem.FindActiveStat(modHandle.TargetStat);
                    if (stat == null)
                        continue;

                    StatModAggregator aggregator = stat.GetCurrentValueAggregator();
                    aggregator?.RemoveMod(modHandle.Handle);
                }
            }

            Spec?.Dispose();

            Context = null;
            Spec = null;
            ModeInstance = null;
            _activeModHandles.Clear();
        }

        protected virtual void OnDispose() { }
    }
}
