// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger LogData = new Logger(Debug.unityLogger.logHandler);
        public static readonly Logger LogGameMode = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.GameFramework
{
    /// <summary>
    /// Initialization data for a <see cref="GameMode"/>. These can be temporarily overridden in editor in <see cref="SceneSettings"/>.
    /// </summary>
    [Serializable]
    public class GameModeSettings
    {
        /// <summary>See: <see cref="ModeController"/></summary>
        [SerializeField] private PlayerController modeController;

        /// <summary>See: <see cref="ModeHUD"/></summary>
        [SerializeField] private PlayerHUD modeHUD;

        /// <summary>See: <see cref="ModeDefaultControllable"/></summary>
        [SerializeField] private Controllable modeDefaultControllable;

        /// <summary>The <see cref="PlayerController"/> to create for each player while in the parent <see cref="GameMode"/>.</summary>
        public PlayerController ModeController { get { return modeController; } }

        /// <summary>The <see cref="PlayerHUD"/> to create for each local player while in the parent <see cref="GameMode"/>.</summary>
        public PlayerHUD ModeHUD { get { return modeHUD; } }

        /// <summary>An optional default <see cref="Controllable"/> to automatically create for each player.</summary>
        public Controllable ModeDefaultControllable { get { return modeDefaultControllable; } }
    }

    /// <summary>
    /// A base class for the active mode of gameplay. Only one can exist at any time, created by the <see cref="GameManager"/>.
    /// These are loaded via a <see cref="SceneSettings"/> instance in a scene, if that scene is loaded non-additively.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))] [RequireComponent(typeof(TimeUnit))]
    public abstract class GameMode : EssenceBehaviour
    {
        public delegate void GameModeDelegate(GameMode gameMode);

        public delegate void GameStateChangedDelegate(GameMode gameMode, GameState oldState);

        /// <summary>An event called when the <see cref="ActiveState"/> updates.</summary>
        public static event GameStateChangedDelegate GameStateChangedEvent;

        public static readonly GameState Initialization = new GameState(0, nameof(Initialization));
        public static readonly GameState PreGame = new GameState(0, nameof(PreGame));
        public static readonly GameState InGame = new GameState(0, nameof(InGame));
        public static readonly GameState PostGame = new GameState(0, nameof(PostGame));

        /// <summary>The basic settings of the mode.</summary>
        [SerializeField] private GameModeSettings settings;

        /// <summary>All <see cref="PlayerController"/>s in the game.</summary>
        private readonly List<PlayerController> _players = new List<PlayerController>();

        /// <summary>The active <see cref="SceneSettings"/>, obtained from the initially traveled scene.</summary>
        private SceneSettings _activeSceneSettings;

        /// <summary>The current <see cref="GameState"/>.</summary>
        public GameState ActiveState { get; private set; }

        protected virtual void Awake()
        {
            ActiveState = Initialization;
        }

        protected override void OnDestroy()
        {
            SceneUnit.TravelState.PrepareFramework.OnStart -= TravelCreateControllables;
            SceneUnit.TravelState.TravelFinishHolders.OnStart -= OnTravelLoadComplete;
            SceneUnit.TravelState.TravelFailed.OnStart -= OnTravelLoadComplete;

            base.OnDestroy();
        }

        /// <summary>
        /// Initializes the <see cref="GameMode"/>. This can only be called by the <see cref="GameManager"/> when creating the game mode.
        /// </summary>
        public void InitializeGameMode()
        {
            if (!GameManager.IsInitializingScene)
            {
                // Ya done messed up.
                LogKit.Log(EssenceLog.LogGameMode, LogType.Error, nameof(InitializeGameMode), "Attempted to initialize game mode when not initializing scene!", this);
                return;
            }

            CreateDefaultPlayerControllers();

            if (SceneUnit.IsTraveling())
            {
                SceneUnit.TravelState.PrepareFramework.OnStart += TravelCreateControllables;
                SceneUnit.TravelState.TravelFinishHolders.OnStart += OnTravelLoadComplete;
                SceneUnit.TravelState.TravelFailed.OnStart += OnTravelLoadComplete;
            }
            else
            {
                TravelCreateControllables(null);
                ActiveState = PreGame;
            }
        }

        /// <summary>
        /// Adds a local player and controller to the game, if possible.
        /// </summary>
        public bool AddLocalPlayer()
        {
            if (PlayerInputManager.instance && PlayerInputManager.instance.maxPlayerCount > 0 && PlayerInputManager.instance.playerCount > PlayerInputManager.instance.maxPlayerCount)
                return false;

            // Find the proper prefab to use.
            PlayerController controllerPrefab = GetControllerPrefab();
            if (!controllerPrefab)
                return false;

            PlayerController newController = Instantiate(controllerPrefab, transform);
            _players.Add(newController);

            return true;
        }

        /// <summary>
        /// Sets a new <see cref="GameState"/>.
        /// </summary>
        /// <param name="inState">The state to set.</param>
        /// <returns>Returns if the <see cref="ActiveState"/> was updated.</returns>
        /// <remarks>If the <paramref name="inState"/> does not have a <see cref="GameState.Parent"/>, it can freely override the current state. If it does have a parent,
        /// the state will only be set if the parent is the <see cref="ActiveState"/>.</remarks>
        public bool SetGameState(GameState inState)
        {
            if (inState == null)
                return false;

            if (inState.Parent != null && inState.Parent != ActiveState)
            {
                LogKit.Log(EssenceLog.LogGameMode, LogType.Error, nameof(SetGameState), $"Cannot set game state to {inState}, as it is not a child of active state {ActiveState}.", this);
                return false;
            }

            string reason = CanChangeGameState();
            if (!string.IsNullOrEmpty(reason))
            {
                LogKit.Log(EssenceLog.LogGameMode, LogType.Warning, nameof(SetGameState), reason, this);
                return false;
            }

            reason = CanChangeToGameState(inState);
            if (!string.IsNullOrEmpty(reason))
            {
                LogKit.Log(EssenceLog.LogGameMode, LogType.Warning, nameof(SetGameState), reason, this);
                return false;
            }

            GameState oldState = ActiveState;
            ActiveState = inState;

            GameStateChangedEvent?.Invoke(this, oldState);
            return true;
        }

        /// <summary>
        /// Gets the current <see cref="PlayerHUD"/> prefab to use.
        /// </summary>
        /// <returns>Returns the current <see cref="PlayerHUD"/> prefab to use.</returns>
        public PlayerHUD GetPlayerHUDPrefab()
        {
            GameModeSettings overrideSettings = FindSceneSettings()?.Data?.OverrideSettings;
            return overrideSettings?.ModeHUD ? overrideSettings.ModeHUD : settings?.ModeHUD;
        }

        /// <summary>
        /// Finds the active scene's <see cref="SceneSettings"/>. This does not check children.
        /// </summary>
        /// <returns>Returns the found <see cref="SceneSettings"/>.</returns>
        protected SceneSettings FindSceneSettings()
        {
            if (!_activeSceneSettings)
                _activeSceneSettings = SceneKit.GetFirstComponentInScene<SceneSettings>(SceneManager.GetActiveScene(), false);

            return _activeSceneSettings;
        }

        /// <summary>
        /// Gets the current <see cref="PlayerController"/> prefab to use.
        /// </summary>
        /// <returns>Returns the current <see cref="PlayerController"/> prefab to use.</returns>
        protected PlayerController GetControllerPrefab()
        {
            GameModeSettings overrideSettings = FindSceneSettings()?.Data?.OverrideSettings;
            return overrideSettings?.ModeController ? overrideSettings.ModeController : settings?.ModeController;
        }

        /// <summary>
        /// Gets the current <see cref="Controllable"/> prefab to use.
        /// </summary>
        /// <returns>Returns the current <see cref="Controllable"/> prefab to use.</returns>
        protected Controllable GetDefaultControllablePrefab()
        {
            GameModeSettings overrideSettings = FindSceneSettings()?.Data?.OverrideSettings;
            return overrideSettings?.ModeDefaultControllable ? overrideSettings.ModeDefaultControllable : settings?.ModeDefaultControllable;
        }

        /// <summary>
        /// Creates all default <see cref="PlayerController"/>s for active players. Called during initialization.
        /// </summary>
        private void CreateDefaultPlayerControllers()
        {
            if (_players.IsNotEmptyOrNull() || !ActiveState.IsOrIsSecondaryTo(Initialization))
                return;

            // TODO - Accurately update this to be the count of all local players. That would need to be cached elsewhere, like on the game manager.
            AddLocalPlayer();
        }

        /// <summary>
        /// Creates controllables for all active players, if possible. This is done after all scenes load, but before travel finishes.
        /// </summary>
        private void TravelCreateControllables(SceneUnit.TravelState state)
        {
            SceneUnit.TravelState.PrepareFramework.OnStart -= TravelCreateControllables;
            OnTravelCreateControllables();
        }

        /// <summary>
        /// Creates a default <see cref="Controllable"/> for the given <see cref="PlayerController"/>. of possible.
        /// </summary>
        /// <param name="inController">The controller to pair to a new <see cref="Controllable"/>.</param>
        private void CreateDefaultControllable(PlayerController inController)
        {
            if (!inController || !ShouldCreateControllableOnTravel(inController))
                return;

            Controllable controllablePrefab = GetDefaultControllablePrefab();
            if (!controllablePrefab)
                return;

            Controllable controllable = Instantiate(controllablePrefab);
            inController.PossessControllable(controllable, false);
        }

        /// <summary>
        /// An event called at the end of scene travel. This finishes initialization.
        /// </summary>
        /// <param name="inState"></param>
        private void OnTravelLoadComplete(SceneUnit.TravelState inState)
        {
            if (inState < SceneUnit.TravelState.TravelFinishHolders || inState != SceneUnit.TravelState.TravelFailed)
                return;

            if (!ActiveState.IsOrIsSecondaryTo(Initialization))
                return;

            SceneUnit.TravelState.TravelFinishHolders.OnStart -= OnTravelLoadComplete;
            SceneUnit.TravelState.TravelFailed.OnStart -= OnTravelLoadComplete;

            SetGameState(PreGame);
        }

        /// <summary>
        /// Checks if the <see cref="ActiveState"/> can be changed to a new state at all.
        /// </summary>
        /// <returns>Returns the reason, if any, why the <see cref="ActiveState"/> cannot be changed.</returns>
        protected virtual string CanChangeGameState()
        {
            if (SceneUnit.IsTraveling() && SceneUnit.CurrentTravelState < SceneUnit.TravelState.TravelFinishHolders)
                return $"Cannot change game state while on travel state {SceneUnit.CurrentTravelState}";

            return string.Empty;
        }

        /// <summary>
        /// Checks if the <see cref="ActiveState"/> can be changed to the <paramref name="inState"/>.
        /// </summary>
        /// <param name="inState">The <see cref="GameState"/> to change to.</param>
        /// <returns>Returns the reason, if any, why the <see cref="ActiveState"/> cannot be changed.</returns>
        protected virtual string CanChangeToGameState(GameState inState)
        {
            return string.Empty;
        }

        /// <summary>
        /// Creates controllables for all active players, if possible. This is done after all scenes load, but before travel finishes.
        /// </summary>
        protected virtual void OnTravelCreateControllables()
        {
            if (!ActiveState.IsOrIsSecondaryTo(Initialization))
                return;

            foreach (PlayerController player in _players)
            {
                if (!player)
                    continue;

                CreateDefaultControllable(player);
            }
        }

        /// <summary>
        /// Checks if a default <see cref="Controllable"/> should be made during initial travel.
        /// </summary>
        /// <param name="inController">The <see cref="PlayerController"/> that is being checked.</param>
        /// <returns>Returns if a default <see cref="Controllable"/> can be made.</returns>
        protected virtual bool ShouldCreateControllableOnTravel(PlayerController inController)
        {
            return true;
        }
    }
}
