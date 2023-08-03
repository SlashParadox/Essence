// Copyright (c) Craig Williams, SlashParadox

// ReSharper disable StaticMemberInGenericType

#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A singleton-pattern behavior base class. These can be used as always-accessible, single instances of a manager class.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="SingletonBehaviour{T}"/>.</typeparam>
    public abstract class SingletonBehaviour<T> : EssenceBehaviour where T : SingletonBehaviour<T>
    {
        /// <summary>The inner current instance of the <see cref="SingletonBehaviour{T}"/>.</summary>
        private static T _currentSingleton;

        /// <summary>The current instance of the <see cref="SingletonBehaviour{T}"/>.</summary>
        protected static T CurrentSingleton { get { return _currentSingleton; } }

        /// <summary>If true, this behaviour can be replaced when a new one is made.</summary>
        protected virtual bool AllowReplacement { get { return false; } }

        /// <summary>If true, this behaviour is moved to not destroy on load.</summary>
        protected virtual bool ShouldNotDestroyOnLoad { get { return false; } }

        /// <summary>If true, this component is destroyed when replaced as the <see cref="_currentSingleton"/>.</summary>
        protected virtual bool DestroyOnReplacement { get { return false; } }

        /// <summary>If true, this singleton is destroyed when it fails to be set.</summary>
        protected virtual bool DestroyOnFailedSet { get { return false; } }

        /// <summary>If true, destroying this <see cref="SingletonBehaviour{T}"/> destroys the parent <see cref="GameObject"/>.</summary>
        protected virtual bool DestroyParentObject { get { return false; } }

        protected virtual void Awake()
        {
            AssignSingleton((T)this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_currentSingleton != this)
                return;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif

            _currentSingleton = null;

            if (DestroyParentObject)
                Destroy(gameObject);
        }

        /// <summary>
        /// Marks a given <see cref="SingletonBehaviour{T}"/> instance as the new <see cref="_currentSingleton"/>.
        /// </summary>
        /// <param name="instance">The wanted <see cref="SingletonBehaviour{T}"/> instance.</param>
        /// <param name="replaceIfAllowed">If true, attempts replacement if the singleton type allows for it.</param>
        /// <returns></returns>
        protected bool AssignSingleton(T instance, bool replaceIfAllowed = false)
        {
            // The instance must be valid.
            if (instance == null)
                return false;

            if (instance == _currentSingleton)
                return true;

            // If there is already an instance, check if replacement is allowed.
            if (_currentSingleton != null)
            {
                if (replaceIfAllowed && AllowReplacement)
                {
                    _currentSingleton.RemoveSingletonStatus(DestroyOnReplacement);
                }
                else
                {
                    if (DestroyOnFailedSet)
                        Destroy(instance);

                    return false;
                }
            }

            _currentSingleton = instance;

            // Mark as no-destroy if required.
            if (ShouldNotDestroyOnLoad)
                DontDestroyOnLoad(_currentSingleton);

            return true;
        }

        /// <summary>
        /// Removes the <see cref="_currentSingleton"/>.
        /// </summary>
        /// <param name="destroy">If true, the <see cref="_currentSingleton"/> instance is also destroyed.</param>
        protected void RemoveSingletonStatus(bool destroy = false)
        {
            if (_currentSingleton != this)
                return;

            _currentSingleton = null;

            if (destroy)
                Destroy(this);
        }
    }

    /// <summary>
    /// A <see cref="SingletonBehaviour{T}"/> with public access.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="PublicSingletonBehavior{T}"/>.</typeparam>
    public abstract class PublicSingletonBehavior<T> : SingletonBehaviour<T> where T : PublicSingletonBehavior<T>
    {
        /// <summary>
        /// Gets the current instance of the <see cref="PublicSingletonBehavior{T}"/>.
        /// </summary>
        /// <typeparam name="TSingleton">The type of the <see cref="PublicSingletonBehavior{T}"/>.</typeparam>
        /// <returns>Returns the <see cref="SingletonBehaviour{T}.CurrentSingleton"/>.</returns>
        public static TSingleton Get<TSingleton>() where TSingleton : T
        {
            return (TSingleton)CurrentSingleton;
        }
    }
}
#endif