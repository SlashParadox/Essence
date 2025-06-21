using SlashParadox.Essence.Gameplay.Skills;
using SlashParadox.Essence.Kits;
using System.Collections;
using UnityEngine;

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger LogControllable = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.GameFramework
{
    public class Controllable : EssenceBehaviour
    {
        public delegate void ControllableDelegate(Controllable controllable);

        public event ControllableDelegate OnControllerSetDelegate;
        
        /// <summary>The current <see cref="PlayerController"/> possessing this <see cref="Controllable"/>.</summary>
        private PlayerController _activeController;

        public SkillEffect testEffect;
        public SkillEffect testEffectInstant;
        
        /// <summary>
        /// Sets the possessing <see cref="PlayerController"/>. Only to be called by an authoritative <see cref="PlayerController"/>.
        /// </summary>
        /// <param name="inController">The new active <see cref="PlayerController"/>.</param>
        /// <returns>Returns if the player controller was successfully set.</returns>
        public bool SetController(PlayerController inController)
        {
            if (!inController)
            {
                LogKit.Log(EssenceLog.LogControllable, LogType.Error, nameof(SetController), $"Cannot set null controller as a possessor.", this);
                return false;
            }
            
            if (_activeController)
            {
                LogKit.Log(EssenceLog.LogControllable, LogType.Error, nameof(SetController), $"Cannot set {inController.name} as possessor. Already possessed by {_activeController.name}", this);
                return false;
            }

            _activeController = inController;
            return true;
        }

        public bool RemoveController(PlayerController inController)
        {
            if (!_activeController)
            {
                LogKit.Log(EssenceLog.LogControllable, LogType.Warning, nameof(SetController), $"Cannot unpossess when this controllable is not possessed at all.", this);
                return true;
            }
            
            if (_activeController != inController)
            {
                LogKit.Log(EssenceLog.LogControllable, LogType.Error, nameof(SetController), $"{inController?.name} is not the possessing controller {_activeController.name}", this);
                return false;
            }

            _activeController = null;
            AcknowledgeController();
            return true;
        }

        /// <summary>
        /// Acknowledges that the controller has changed. Called manually on authority.
        /// </summary>
        /// <param name="lastController">The last controller that handled this pawn.</param>
        public void AcknowledgeController()
        {
            OnControllerSetDelegate?.Invoke(this);

            StartCoroutine(TestEffect());
        }

        private IEnumerator TestEffect()
        {
            if (!testEffect)
                yield break;

            //GetComponent<SkillsSystem>().ApplySkillEffectToSelf(new SkillEffectContext(testEffectInstant));
            ID.Handle handle = GetComponent<SkillsSystem>().ApplySkillEffectToSelf(new SkillEffectContext(testEffect));

            yield return new WaitForSeconds(5);
            GetComponent<SkillsSystem>().RemoveActiveSkillEffectByHandle(handle);
        }
    }
}
