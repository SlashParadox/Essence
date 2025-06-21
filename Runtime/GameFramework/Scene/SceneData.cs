// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;

namespace SlashParadox.Essence.GameFramework
{
    [CreateAssetMenu(fileName = "NewSceneData", menuName = "Essence/Scene/Scene Data")]
    public class SceneData : EssenceScriptableObject
    {
        [SerializeField] private GameMode gameModePrefab;

#if UNITY_EDITOR
        [SerializeField] private GameModeSettings overrideSettings;
#endif

        internal GameMode GameModePrefab { get { return gameModePrefab; } }

        internal GameModeSettings OverrideSettings
        {
            get
            {
#if UNITY_EDITOR
                return overrideSettings;
#else
                return null;
#endif
            }
        }
    }
}