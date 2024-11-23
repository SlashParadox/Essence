// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;

namespace SlashParadox.Essence
{
    public delegate void BehaviourEvent(MonoBehaviour behaviour);

    public delegate void BehaviourEnabledEvent(MonoBehaviour behaviour, bool enabled);
    
    /// <summary>
    /// An enhanced <see cref="MonoBehaviour"/> script.
    /// </summary>
    public class EssenceBehaviour : MonoBehaviour
    {
        /// <summary>An event invoked upon starting.</summary>
        public event BehaviourEvent OnStarted;

        /// <summary>An event invoked upon being enabled or disabled.</summary>
        public event BehaviourEnabledEvent OnEnabledUpdated;

        /// <summary>An event invoked upon destruction.</summary>
        public event BehaviourEvent OnDestroyed;

        protected virtual void Start()
        {
            OnStarted?.Invoke(this);
        }

        protected virtual void OnEnable()
        {
            OnEnabledUpdated?.Invoke(this, true);
        }

        protected virtual void OnDisable()
        {
            OnEnabledUpdated?.Invoke(this, false);
        }

        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }

        /// <summary>
        /// Finds the first valid component, if the given component is null.
        /// </summary>
        /// <param name="inComponent">The component to test.</param>
        /// <typeparam name="T">The type of <see cref="Component"/>.</typeparam>
        /// <returns>Returns if <paramref name="inComponent"/> is valid.</returns>
        protected bool FindComponentIfNull<T>(T inComponent) where T : Component
        {
            if (inComponent)
                return true;

            inComponent = GetComponent<T>();
            return inComponent != null;
        }
    }
}