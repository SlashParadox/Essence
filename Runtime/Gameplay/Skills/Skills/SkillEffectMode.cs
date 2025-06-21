// Copyright (c) Craig Williams, SlashParadox

namespace SlashParadox.Essence.Gameplay.Skills
{
    /// <summary>
    /// The value that a <see cref="SkillEffect"/> changes on a <see cref="Stat"/>.
    /// </summary>
    [System.Serializable]
    public enum SkillEffectTargetValue
    {
        Base,
        Current
    }

    /// <summary>
    /// The mode of operation for a <see cref="SkillEffect"/>. This controls how the effect applies, executes, and updates
    /// over the course of its lifetime.
    /// </summary>
    [System.Serializable]
    public abstract class SkillEffectMode
    {
#if UNITY_EDITOR
        public int debug;
#endif

        private SkillsSystem _system;

        private ID.Handle _activeHandle;

        protected void RequestExecution()
        {
            _system?.ExecuteSkillEffect(_activeHandle);
        }

        protected void RequestRemoval()
        {
            OnPreRemoved();
            _system?.RemoveActiveSkillEffectByHandle(_activeHandle);
        }

        internal void OnSkillEffectApplied(SkillsSystem target, ID.Handle activeHandle)
        {
            _system = target;
            _activeHandle = activeHandle;
            OnApplied();
        }

        internal void OnSkillEffectExecuted(SkillsSystem target)
        {
            OnExecuted();
        }

        internal void OnSkillEffectRemoved(SkillsSystem target)
        {
            OnRemoved();
        }

        public virtual SkillEffectTargetValue GetTargetedValue()
        {
            return SkillEffectTargetValue.Current;
        }

        protected virtual void OnApplied() { }

        protected virtual void OnExecuted() { }

        protected virtual void OnPreRemoved() { }

        protected virtual void OnRemoved() { }
    }
}
