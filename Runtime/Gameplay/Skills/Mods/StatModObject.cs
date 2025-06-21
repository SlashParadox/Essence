// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;

namespace SlashParadox.Essence.Gameplay.Skills
{
    [CreateAssetMenu(fileName = "NewStatMod", menuName = "Essence/Gameplay/Stats/Mod Asset")]
    public sealed class StatModObject : EssenceScriptableObject
    {
        [SerializeReference] [Instanced] private StatMod mod;

        public StatMod GetStatMod()
        {
            return mod;
        }
    }
}
