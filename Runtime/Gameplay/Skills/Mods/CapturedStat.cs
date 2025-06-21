using System;
using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [Serializable]
    public enum CapturedStatOrigin
    {
        Source,
        Target
    }

    [Serializable]
    public enum CapturedStatValueType
    {
        Base,
        Current
    }

    [Serializable]
    public struct CapturedStat : IEquatable<CapturedStat>
    {
        /// <summary>The <see cref="Stat"/> to capture.</summary>
        public Stat targetStat;

        /// <summary>The <see cref="StatsSystem"/> to get the stat from.</summary>
        public CapturedStatOrigin origin;

        /// <summary>The value to capture.</summary>
        public CapturedStatValueType valueType;

        /// <summary>If true, the captured stat should update when the originating value changes.</summary>
        public bool allowUpdates;

        /// <summary>The <see cref="StatsSystem"/> to get the stat from. Can be set ahead of time to allow for custom origins.</summary>
        [NonSerialized] public StatsSystem OriginSystem;

        public static bool operator ==(CapturedStat a, CapturedStat b)
        {
            return a.targetStat == b.targetStat
                   && a.origin == b.origin
                   && a.valueType == b.valueType
                   && a.allowUpdates == b.allowUpdates;
        }

        public static bool operator !=(CapturedStat a, CapturedStat b)
        {
            return !(a == b);
        }

        public bool Equals(CapturedStat other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is CapturedStat other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(targetStat, (int)valueType, allowUpdates, OriginSystem ? OriginSystem.GetHashCode() : (int)origin);
        }
    }

    public class CapturedStatData
    {
        public event StatModDirtyDelegate OnDirtyDelegate;

        public readonly CapturedStat BackingStat;

        public float CapturedValue { get; private set; }

        public CapturedStatData(CapturedStat backingStat, bool allowUpdates)
        {
            BackingStat = backingStat;

            if (allowUpdates && BackingStat.allowUpdates)
            {
                ActiveStat foundStat = BackingStat.OriginSystem?.FindActiveStat(BackingStat.targetStat);
                if (foundStat != null)
                    foundStat.StatUpdatedEvent += OnCapturedStatUpdated;
            }

            CaptureStat();
        }

        public void Cleanup()
        {
            OnDirtyDelegate = null;

            if (!BackingStat.OriginSystem)
                return;

            ActiveStat foundStat = BackingStat.OriginSystem.FindActiveStat(BackingStat.targetStat);
            if (foundStat != null)
                foundStat.StatUpdatedEvent -= OnCapturedStatUpdated;
        }

        public void CaptureStat()
        {
            ActiveStat foundStat = BackingStat.OriginSystem?.FindActiveStat(BackingStat.targetStat);
            if (foundStat == null)
                return;

            float lastValue = CapturedValue;
            CapturedValue = BackingStat.valueType == CapturedStatValueType.Base ? foundStat.BaseValue : foundStat.CurrentValue;

            if (!Mathf.Approximately(lastValue, CapturedValue))
                OnDirtyDelegate?.Invoke();
        }

        private void OnCapturedStatUpdated(ActiveStat stat)
        {
            CaptureStat();
        }
    }
}
