// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using SlashParadox.Essence.Kits;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>A delegate relating to <see cref="GameManager"/>.</summary>
    public delegate void GameManagerDelegate(GameManager manager);
    
    /// <summary>
    /// An always-existing object that can be accessed as a way of getting lifetime-persistant data. Only one <see cref="GameManager"/>
    /// can exist at a time, and is created before any scene loads.
    /// </summary>
    [DisallowMultipleComponent] [AddComponentMenu("")]
    public abstract class GameManager : PublicSingletonBehavior<GameManager>
    {
        protected override bool ShouldNotDestroyOnLoad { get { return true; } }

        protected override bool DestroyOnFailedSet { get { return true; } }

        /// <summary>An event called when the <see cref="GameManager"/> is created. Bind to this to ensure the singleton is ready.</summary>
        public static event GameManagerDelegate OnGameManagerCreated;

        /// <summary>The <see cref="GameManagerData"/> related to this manager. An easy way of passing important game data around.</summary>
        private GameManagerData _gameManagerData;

        /// <summary>
        /// An event used to create the <see cref="GameManager"/> before anything else loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLoaded()
        {
            // Should be impossible, but do it anyways.
            if (CurrentSingleton)
                return;

            EssenceProjectSettings settings = EssenceProjectSettings.Get<EssenceProjectSettings>();
            DebugKit.ThrowNullIf(!settings, $"Could not find {nameof(EssenceProjectSettings)}! Unable to create a {nameof(GameManager)}!");

            MemberType<GameManager> managerClass = settings.GameManagerClass;
            DebugKit.ThrowNullIf(managerClass == null, $"{nameof(EssenceProjectSettings.GameManagerClass)} is null!");

            if (managerClass == null)
                return;

            GameObject managerObj = new GameObject($"Essence_GameManager_{managerClass.Type.Name}", managerClass);
            managerObj.name = $"Essence_GameManager_{managerClass.Type.Name}";

            if (!CurrentSingleton)
                return;

            CurrentSingleton._gameManagerData = settings.SafeGetGameManagerData(CurrentSingleton);
            CurrentSingleton.OnGameDataSet();
            
            OnGameManagerCreated?.Invoke(CurrentSingleton);
        }

        /// <summary>
        /// Checks if this <see cref="GameManager"/> has <see cref="GameManagerData"/>.
        /// </summary>
        /// <returns>Returns if this <see cref="GameManager"/> has <see cref="GameManagerData"/>.</returns>
        public bool HasGameData()
        {
            return _gameManagerData;
        }

        /// <summary>
        /// Gets the <see cref="_gameManagerData"/> associated with this <see cref="GameManager"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="GameManagerData"/>.</typeparam>
        /// <returns>Returns the casted <see cref="_gameManagerData"/>.</returns>
        public T GetGameData<T>() where T : GameManagerData
        {
            return (T)_gameManagerData;
        }
        
        /// <summary>
        /// An event called when game data is properly set onto the <see cref="GameManager"/>.
        /// </summary>
        protected virtual void OnGameDataSet() { }
    }
}
#endif