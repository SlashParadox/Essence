using UnityEngine;
using UnityEngine.UI;

namespace SlashParadox.Essence.GameFramework
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler))] [DisallowMultipleComponent]
    public class PlayerHUD : EssenceBehaviour
    {
        private PlayerController _owner;

        /// <summary>
        /// Gets the owning <see cref="PlayerController"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="PlayerController"/> to cast to.</typeparam>
        /// <returns>Returns the <see cref="_owner"/>.</returns>
        public T GetOwningController<T>() where T : PlayerController
        {
            return (T)_owner;
        }
        
        /// <summary>
        /// Initializes the HUD upon creation. Only to be called by the <see cref="GameMode"/> when creating new <see cref="PlayerController"/>s.
        /// </summary>
        /// <param name="inOwner"></param>
        public void InitializeHUD(PlayerController inOwner)
        {
            if (_owner || !inOwner)
                return;

            _owner = inOwner;
        }
    }
}
