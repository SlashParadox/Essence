using SlashParadox.Essence.GameFramework;
using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    public class UIBehaviour : EssenceBehaviour
    {
        #if UNITY_EDITOR
        public PreviewGatherer preview;
        #endif

        [Preview] private PlayerHUD _owningHUD;

        protected virtual void Awake()
        {
            _owningHUD = GetComponentInParent<PlayerHUD>();
        }

        public T GetOwningHUD<T>() where T : PlayerHUD
        {
            return (T)_owningHUD;
        }
    }
}
