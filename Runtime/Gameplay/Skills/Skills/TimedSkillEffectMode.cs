using SlashParadox.Essence.GameFramework;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [System.Serializable]
    public class TimedSkillEffectMode : SkillEffectMode
    {
        [SerializeField] private float duration;

        private ID.SharedHandle _timerHandle;

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnApplied()
        {
            if (duration <= 0.0f)
                return;

            TimeUnit.CreateTimer(ref _timerHandle, duration, OnDurationComplete);
        }

        private void OnDurationComplete()
        {
            _timerHandle = null;
            RequestRemoval();
        }

        protected override void OnRemoved()
        {
            TimeUnit.RemoveTimer(ref _timerHandle);
        }

        public override SkillEffectTargetValue GetTargetedValue()
        {
            return SkillEffectTargetValue.Current;
        }
    }
}
