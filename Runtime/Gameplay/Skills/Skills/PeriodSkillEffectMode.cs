using SlashParadox.Essence.GameFramework;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [System.Serializable]
    public sealed class PeriodSkillEffectMode : TimedSkillEffectMode
    {
        [SerializeField] private float period;

        private ID.SharedHandle _periodHandle;

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnApplied()
        {
            base.OnApplied();

            if (period <= 0.0f)
            {
                OnPeriodComplete();
                return;
            }

            TimeUnit.CreateTimer(ref _periodHandle, period, OnPeriodComplete, 0);
        }

        private void OnPeriodComplete()
        {
            RequestExecution();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            TimeUnit.RemoveTimer(ref _periodHandle);
        }

        protected override void OnPreRemoved()
        {
            if (TimeUnit.WillTimerCompleteThisFrame(_periodHandle))
                OnPeriodComplete();
        }

        public override SkillEffectTargetValue GetTargetedValue()
        {
            return SkillEffectTargetValue.Base;
        }
    }
}
