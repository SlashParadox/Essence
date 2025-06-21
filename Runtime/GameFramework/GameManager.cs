// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger LogGameManager = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.GameFramework
{
    /// <summary>A delegate relating to <see cref="GameManager"/>.</summary>
    public delegate void GameManagerDelegate(GameManager manager);

    /// <summary>
    /// An always-existing object that can be accessed as a way of getting lifetime-persistant data. Only one <see cref="GameManager"/>
    /// can exist at a time, and is created before any scene loads.
    /// </summary>
    [DisallowMultipleComponent] [RequireComponent(typeof(SceneUnit))]
    public abstract class GameManager : PublicSingletonBehavior<GameManager>
    {
        /// <summary>An event called when the <see cref="GameManager"/> is created. Bind to this to ensure the singleton is ready.</summary>
        public static event GameManagerDelegate OnGameManagerCreated;

        /// <summary>If true, a new scene is being initialized and loaded.</summary>
        internal static bool IsInitializingScene { get; private set; }

        /// <summary>The <see cref="GameManagerData"/> related to this manager. An easy way of passing important game data around.</summary>
        private GameManagerData _gameManagerData;

        /// <summary>If true, the <see cref="GameManager"/> has done its game-start initialization.</summary>
        private bool _hasInitialized;

        /// <summary>The active <see cref="SceneSettings"/> object. This should live in the persistent scene that will always exist between additive scene swaps.</summary>
        public SceneSettings CurrentSceneSettings { get; private set; }

        protected override bool ShouldNotDestroyOnLoad { get { return true; } }

        protected override bool DestroyOnFailedSet { get { return true; } }

        /// <summary>The currently active <see cref="GameMode"/>.</summary>
        public GameMode CurrentGameMode { get; private set; }

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
            LogKit.ThrowNullIf(!settings, $"Could not find {nameof(EssenceProjectSettings)}! Unable to create a {nameof(GameManager)}!");

            GameManager prefab = settings.GameManagerPrefab;
            LogKit.ThrowNullIf(!prefab, $"{nameof(EssenceProjectSettings.GameManagerPrefab)} is null! One must be provided!");
            if (prefab)
            {
                GameManager createdManager = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                createdManager.gameObject.name = $"Essence_GameManager_{createdManager.GetType().Name}";
            }

            if (!CurrentSingleton)
                return;

            CurrentSingleton._hasInitialized = false;
            CurrentSingleton.InitializeGameManager();

            CurrentSingleton._gameManagerData = settings.SafeGetGameManagerData(CurrentSingleton);
            CurrentSingleton.OnGameDataSet();

            CurrentSingleton._hasInitialized = true;
            OnGameManagerCreated?.Invoke(CurrentSingleton);
        }

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnNewSceneLoaded;

            base.OnDestroy();
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
        /// Initializes the <see cref="GameManager"/>. Done before the first scene load.
        /// </summary>
        private void InitializeGameManager()
        {
            if (_hasInitialized)
                return;
            
            InitializeWithSceneManager();

            _hasInitialized = true;
            OnInitialized();
        }

        /// <summary>
        /// Registers the <see cref="GameManager"/> with the <see cref="SceneManager"/>.
        /// </summary>
        private void InitializeWithSceneManager()
        {
            if (_hasInitialized)
                return;
            
            SceneManager.sceneLoaded += OnNewSceneLoaded;
        }

        /// <summary>
        /// An event called whenever a new scene is loaded.
        /// </summary>
        /// <param name="newScene">The new scene.</param>
        /// <param name="loadMode">The method that the scene loaded.</param>
        private void OnNewSceneLoaded(Scene newScene, LoadSceneMode loadMode)
        {
            // This method only cares about finding scene settings in a brand new slate of scenes.
            if (loadMode == LoadSceneMode.Additive)
                return;

            // A unique case: The first scene of the game does not actually use either scene mode, but instead an unknown value at 4. Use this as a way to re-travel to the opening scene.
            if (loadMode != LoadSceneMode.Single)
            {
                SceneUnit.BeginTravel(newScene.name);
                return;
            }

            // Find the scene settings for the initial scene. All basic game framework is loaded based on this object.
            SceneSettings sceneSettings = SceneKit.GetFirstComponentInScene<SceneSettings>(newScene, true);
            if (!sceneSettings)
            {
                LogKit.Log(EssenceLog.LogGameManager, LogType.Warning, nameof(OnNewSceneLoaded), $"Active Scene [{newScene.name}] has no {nameof(SceneSettings)}!", this);
                CurrentSceneSettings = null;
                return;
            }

            CurrentSceneSettings = sceneSettings;
            InitializeSceneSettings();
        }

        /// <summary>
        /// Initializes the <see cref="CurrentSceneSettings"/> after a new scene load.
        /// </summary>
        private void InitializeSceneSettings()
        {
            if (!CurrentSceneSettings || !CurrentSceneSettings.Data || !SceneManager.GetActiveScene().IsValid())
                return;

            if (CurrentGameMode)
                Destroy(CurrentGameMode.gameObject);

            CurrentGameMode = null;

            GameMode gameModePrefab = CurrentSceneSettings.Data.GameModePrefab;
            LogKit.ThrowNullIf(gameModePrefab == null, $"The {nameof(SceneData.GameModePrefab)} of {nameof(SceneData)} [{CurrentSceneSettings.Data.name}] is null!");

            IsInitializingScene = true;
            CurrentGameMode = Instantiate(gameModePrefab, transform);
            CurrentGameMode.name = $"GameMode_{gameModePrefab.name}";
            CurrentGameMode.InitializeGameMode();
            IsInitializingScene = false;
        }

        /// <summary>
        /// An event called when the manager is created, but before the <see cref="_gameManagerData"/> is set. No scene has been loaded at this point.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// An event called when game data is properly set onto the <see cref="GameManager"/>.
        /// </summary>
        protected virtual void OnGameDataSet() { }
    }
}
#endif