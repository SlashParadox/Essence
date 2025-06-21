using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SlashParadox.Essence.Gameplay.Skills
{
    public delegate void StatModDirtyDelegate();

    public enum StatModType
    {
        Add,
        Multiply,
        Divide,
        Override
    }

    [System.Serializable]
    public abstract class StatMod
    {
        [SerializeField] private StatModType modType;

        public StatModType ModType { get { return modType; } }

        [SerializeField] private int modChannel = 0;

        [SerializeField] private int modPriority = 0;

        public int ModChannel { get { return modChannel; } }

        public int ModPriority { get { return modPriority; } }

        public virtual float CalculateMagnitude(StatModContext context)
        {
            return modType is StatModType.Multiply or StatModType.Divide ? 1.0f : 0.0f;
        }

        public virtual List<CapturedStat> GatherRequiredStats()
        {
            return null;
        }

        public virtual List<CapturedStat> GatherOptionalStats()
        {
            return null;
        }
    }

    [System.Serializable]
    public struct StatModItem
    {
        [SerializeField] private StatModObject modObject;

        [SerializeReference] [Instanced] private StatMod mod;

        public StatModItem(StatMod inMod)
        {
            modObject = null;
            mod = inMod;
        }

        public StatModItem(StatModObject inObject)
        {
            modObject = inObject;
            mod = null;
        }

        public StatMod GetStatMod()
        {
            return modObject ? modObject.GetStatMod() : mod;
        }

        public static implicit operator StatModItem(StatModObject inObject)
        {
            return new StatModItem(inObject);
        }

        public static implicit operator StatModItem(StatMod inMod)
        {
            return new StatModItem(inMod);
        }
    }

    internal abstract class StatModHandler
    {
        public bool IsDirty { get; protected set; }
    }

    public struct ActiveStatModOld
    {
        public StatModItem Mod;

        public StatModContext Context;

        public bool isDirty;

        public ActiveStat.ActiveStatDelegate updateAction;

        public ActiveStatModOld(StatModItem inMod, StatModContext inContext)
        {
            Mod = inMod;
            Context = inContext;
            isDirty = true;
            updateAction = null;
        }

        public bool IsValid()
        {
            return Mod.GetStatMod() != null;
        }

        public float CalculateMagnitude()
        {
            return Mod.GetStatMod().CalculateMagnitude(Context);
        }
    }
}
