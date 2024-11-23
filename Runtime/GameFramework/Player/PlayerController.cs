using UnityEngine;
using UnityEngine.InputSystem;

namespace SlashParadox.Essence
{
    public class PlayerController : EssenceBehaviour
    {
        private Controllable _controllable;

        public T GetControllable<T>() where T : Controllable
        {
            return (T)_controllable;
        }
    }
}
