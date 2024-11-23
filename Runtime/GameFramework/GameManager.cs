// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using SlashParadox.Essence.GameUnits;
using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence
{
    /// <summary>A delegate relating to <see cref="GameManager"/>.</summary>
    public delegate void GameManagerDelegate(GameManager manager);
    
    /// <summary>
    /// An always-existing object that can be accessed as a way of getting lifetime-persistant data. Only one <see cref="GameManager"/>
    /// can exist at a time, and is created before any scene loads.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class GameManager : PublicSingletonBehavior<GameManager>
    {
        protected override bool ShouldNotDestroyOnLoad { get { return true; } }

        protected override bool DestroyOnFailedSet { get { return true; } }

        /// <summary>An event called when the <see cref="GameManager"/> is created. Bind to this to ensure the singleton is ready.</summary>
        public static event GameManagerDelegate OnGameManagerCreated;

        /// <summary>The <see cref="GameManagerData"/> related to this manager. An easy way of passing important game data around.</summary>
        private GameManagerData _gameManagerData;

        private bool _hasInitialized;

        private SceneSettings _currentSceneSettings;

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

            MemberType<GameManager> managerClass = settings.GameManagerClass;
            LogKit.ThrowNullIf(managerClass == null, $"{nameof(EssenceProjectSettings.GameManagerClass)} is null!");

            if (managerClass == null)
                return;

            string managerName = $"Essence_GameManager_{managerClass.Type.Name}";
            GameObject managerObj = new GameObject(managerName, new[]{managerClass, typeof(SceneUnit)});
            managerObj.name = managerName;

            if (!CurrentSingleton)
                return;
            
            CurrentSingleton._hasInitialized = false;
            CurrentSingleton.InitializeGameManager();

            CurrentSingleton._gameManagerData = settings.SafeGetGameManagerData(CurrentSingleton);
            CurrentSingleton.OnGameDataSet();
            
            CurrentSingleton._hasInitialized = true;
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

        private void InitializeGameManager()
        {
            if (_hasInitialized)
                return;
            
            InitializeWithSceneManager();
            
            OnInitialized();
        }

        private void InitializeWithSceneManager()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene previous, Scene current)
        {
            SceneSettings sceneSettings = SceneKit.GetFirstComponentInScene<SceneSettings>(current, true);
            if (!LogKit.LogIfFalse(sceneSettings, $"Active Scene [{current.name}] has no {nameof(SceneSettings)}!"))
            {
                if (previous.isSubScene)
                    Debug.LogWarning($"Continuing to use {nameof(SceneSettings)} from previously active scene [{previous.name}]! If this is intentional, ignore this warning.");
                else
                    _currentSceneSettings = null;

                return;
            }
            
            _currentSceneSettings = sceneSettings;
            LoadAdditiveScenes();
        }

        private void LoadAdditiveScenes()
        {
            if (_currentSceneSettings == null || _currentSceneSettings.AdditiveScenes.Length <= 0)
            {
                InitializeSceneSettings();
                return;
            }
            
            SceneUnit.LoadAdditiveScenes(_currentSceneSettings.AdditiveScenes, InitializeSceneSettings);
        }

        private void InitializeSceneSettings()
        {
            if (!_currentSceneSettings || !_currentSceneSettings.Data || !SceneManager.GetActiveScene().IsValid())
                return;

            Destroy(CurrentGameMode);
            CurrentGameMode = null;
            return;

            GameMode gameModePrefab = _currentSceneSettings.Data.GameModePrefab;
            LogKit.ThrowNullIf(gameModePrefab == null, $"The {nameof(SceneData.GameModePrefab)} of {nameof(SceneData)} [{_currentSceneSettings.Data.name}] is null!");

            CurrentGameMode = Instantiate(gameModePrefab, transform);
            CurrentGameMode.name = $"GameMode_{CurrentGameMode.name}";
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