// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// Base project settings for the <see cref="SlashParadox.Essence"/> library.
    /// </summary>
    [ProjectSettings(nameof(ProjectSettingsType.Runtime), "Essence", "EssenceSettings.asset")]
    public class EssenceProjectSettings : PublicProjectSettingsObject<EssenceProjectSettings>
    {
        /// <summary>The <see cref="System.Type"/> to use for the <see cref="GameManager"/>.</summary>
        [Header("Game Data Settings")]
        [SerializeField] private MemberType<GameManager> gameManagerClass = new MemberType<GameManager>(false);

        /// <summary>The <see cref="GameManagerData"/> to attach to the <see cref="GameManager"/>.</summary>
        [SerializeField] private GameManagerData gameManagerData;

        /// <summary>The default scene to use when displaying a loading screen.</summary>
        [SerializeField] [SceneReference] private string defaultLoadScene;

        /// <summary>The <see cref="System.Type"/> to use for the <see cref="GameManager"/>.</summary>
        public MemberType<GameManager> GameManagerClass { get { return gameManagerClass; } }

#if UNITY_EDITOR
        [UnityEditor.SettingsProvider]
        private static UnityEditor.SettingsProvider GetProvider()
        {
            return ProjectSettingsProvider.GetProvider(GetOrCreateInstance());
        }
#endif

        /// <summary>
        /// Safely gets the <see cref="gameManagerData"/> by only giving it to an uninitialized <see cref="GameManager"/>.
        /// </summary>
        /// <param name="gameManager">The <see cref="GameManager"/> to give the data to.</param>
        /// <returns>Returns the <see cref="gameManagerData"/>.</returns>
        internal GameManagerData SafeGetGameManagerData(GameManager gameManager)
        {
            if (!LogKit.LogIfFalse(gameManager, "Null game manager given!"))
                return null;

            if (!LogKit.LogIfFalse(!gameManager.HasGameData(), "Given game manager already has game data!"))
                return null;

            return gameManagerData;
        }
    }
}