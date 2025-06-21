using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger LogController = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.GameFramework
{
    public class PlayerController : EssenceBehaviour
    {
        private PlayerHUD _playerHUD;
        
        private Controllable _controllable;

        private bool _isControllerInitialized;

        protected virtual void Awake()
        {
            InitializePlayerController();
        }

        protected override void OnDestroy()
        {
            if (_playerHUD)
                Destroy(_playerHUD.gameObject);

            _playerHUD = null;
            
            base.OnDestroy();
        }

        public bool PossessControllable(Controllable inControllable, bool allowSwapping)
        {
            if (!inControllable)
            {
                LogKit.Log(EssenceLog.LogController, LogType.Warning, nameof(PossessControllable), $"Cannot possess a null controllable. Use UnpossessControllable instead.");
                return false;
            }
            
            if (_controllable && !allowSwapping)
            {
                LogKit.Log(EssenceLog.LogController, LogType.Warning, nameof(PossessControllable), $"Cannot possess {inControllable.name}. Already possessing {_controllable.name}");
                return false;
            }
            
            // Unpossess the previous controller.
            UnpossessControllable();

            bool success = inControllable.SetController(this);
            if (!success)
            {
                LogKit.Log(EssenceLog.LogController, LogType.Warning, nameof(PossessControllable), $"Failed to possess {inControllable.name}!");
                return false;
            }

            _controllable = inControllable;
            _controllable.AcknowledgeController();
            return true;
        }

        public bool UnpossessControllable()
        {
            if (!_controllable)
                return false;

            _controllable.RemoveController(this);
            _controllable = null;
            return true;
        }

        public T GetControllable<T>() where T : Controllable
        {
            return (T)_controllable;
        }

        private void InitializePlayerController()
        {
            if (_isControllerInitialized)
                return;
            
            CreatePlayerHUD();
        }

        protected void CreatePlayerHUD()
        {
            if (_playerHUD || !CanCreatePlayerHUD())
                return;

            PlayerHUD prefab = GameManager.Get<GameManager>()?.CurrentGameMode?.GetPlayerHUDPrefab();
            if (!prefab)
                return;

            _playerHUD = Instantiate(prefab);
            DontDestroyOnLoad(_playerHUD);
            
            _playerHUD.InitializeHUD(this);
        }

        protected virtual bool CanCreatePlayerHUD()
        {
            return true;
        }
    }
}
