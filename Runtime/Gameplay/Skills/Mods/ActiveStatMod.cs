using SlashParadox.Essence.Kits;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    public sealed class ActiveStatMod
    {
        public event StatModDirtyDelegate OnDirtyDelegate;

        public StatMod Mod { get; private set; }

        public StatModContext Context { get; private set; }

        public ID.SharedHandle Handle { get; private set; }

        private bool _isInitialized;

        private float? _modMagnitude;

        public ActiveStatMod(StatMod inMod, StatModContext inContext, ID.SharedHandle inHandle)
        {
            Mod = inMod;
            Context = inContext;
            Handle = inHandle;
        }

        public void InitializeActiveMod()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _modMagnitude = null;
            InitializeCapturedStats();
        }

        public float CalculateMagnitude()
        {
            if (_modMagnitude.HasValue)
                return _modMagnitude.Value;

            _modMagnitude = Mod?.CalculateMagnitude(Context);
            return _modMagnitude ?? 0.0f;
        }

        private void InitializeCapturedStats()
        {
            if (Mod == null)
                return;

            InitializeCapturedStats(Mod.GatherRequiredStats());
            InitializeCapturedStats(Mod.GatherOptionalStats());
        }

        private void InitializeCapturedStats(List<CapturedStat> stats)
        {
            StatModSpec spec = Context?.GetStatModSpec<StatModSpec>();
            if (spec == null || stats.IsEmptyOrNull())
                return;

            for (int i = 0; i < stats.Count; ++i)
            {
                CapturedStat stat = stats[i];
                if (!stat.OriginSystem)
                    stat.OriginSystem = stat.origin == CapturedStatOrigin.Source ? Context.SourceSystem : Context.TargetSystem;

                CapturedStatData data = spec.AddCapturedStat(in stat, true);
                if (data == null)
                    return;

                data.OnDirtyDelegate += OnCapturedStatUpdated;
            }
        }

        private void OnCapturedStatUpdated()
        {
            _modMagnitude = null;
            OnDirtyDelegate?.Invoke();
        }
    }
}
