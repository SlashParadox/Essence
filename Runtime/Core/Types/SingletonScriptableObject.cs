// Copyright (c) Craig Williams, SlashParadox

#if UNITY_2019_1_OR_NEWER
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// An <see cref="Attribute"/> for declaring a <see cref="SingletonScriptableObject{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptableSingletonAttribute : Attribute
    {
        /// <summary>The path to the asset.</summary>
        protected string AssetPath;

        /// <summary>If true, an instance of this singleton can be created during runtime.</summary>
        public bool IsRuntimeCreatable { get; }

        public ScriptableSingletonAttribute(bool isRuntimeCreatable, string assetPath)
        {
            IsRuntimeCreatable = isRuntimeCreatable;
            AssetPath = assetPath;
        }

        /// <summary>
        /// Gets the full file path to the asset.
        /// </summary>
        /// <returns>Returns the full file path.</returns>
        public virtual string GetAssetPath()
        {
            return AssetPath;
        }
    }

    /// <summary>
    /// A type of singleton that is stored as an asset <see cref="ScriptableObject"/>.
    /// Especially useful for settings.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="SingletonScriptableObject{T}"/> class.</typeparam>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        /// <summary>The internal current instance of the <see cref="SingletonScriptableObject{T}"/>.</summary>
        private static T _currentSingleton;

        // ReSharper disable once StaticMemberInGenericType
        /// <summary>If true, an instance of this was created at runtime.</summary>
        private static bool _wasCreatedAtRuntime;

        /// <summary>The current instance of the <see cref="SingletonScriptableObject{T}"/>.</summary>
        protected static T CurrentSingleton { get { return GetOrCreateInstance(); } }

        protected virtual void Awake()
        {
            if (_currentSingleton)
                return;

            _currentSingleton = (T)this;

            if (_wasCreatedAtRuntime)
                _currentSingleton.hideFlags |= HideFlags.HideAndDontSave;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

        /// <summary>
        /// Gets the informative <see cref="ScriptableSingletonAttribute"/>, necessary for all
        /// singleton classes to have.
        /// </summary>
        /// <returns>Returns this class's <see cref="ScriptableSingletonAttribute"/>.</returns>
        protected static ScriptableSingletonAttribute GetSingletonAttribute()
        {
            return typeof(T).GetCustomAttribute<ScriptableSingletonAttribute>(true);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Gets the <see cref="_currentSingleton"/>, or attempts to create a new instance.
        /// </summary>
        /// <returns>Returns the <see cref="_currentSingleton"/>.</returns>
        protected static T GetOrCreateInstance()
        {
            // Return the instance if it already exists.
            if (_currentSingleton)
                return _currentSingleton;

            // The attribute needs to exist for this class!
            ScriptableSingletonAttribute attribute = GetSingletonAttribute();
            if (attribute == null)
            {
                Debug.Log($"{typeof(T)} is missing a {typeof(ScriptableSingletonAttribute)}!");
                return null;
            }

            // Attempt to load the asset at some path.
            string path = attribute.GetAssetPath();
            _currentSingleton = LoadInstanceFromFile(path);
            if (_currentSingleton)
                return _currentSingleton;

            // Attempt to find an asset that was possibly lost on a domain reload.
            T[] instances = Resources.FindObjectsOfTypeAll<T>();
            if (instances.Length > 0)
            {
                _currentSingleton = instances[0];
                return _currentSingleton;
            }

            // If not runtime-creatable, don't create at runtime.
            if (!attribute.IsRuntimeCreatable && Application.isPlaying)
                return null;

            // The last resort is to create a whole new scriptable object.
            _wasCreatedAtRuntime = Application.isPlaying;
            _currentSingleton = CreateInstance<T>();

#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(path) ?? string.Empty));
                UnityEditor.AssetDatabase.CreateAsset(_currentSingleton, path);
            }
#endif

            return _currentSingleton;
        }

        /// <summary>
        /// Attempts to load a <see cref="SingletonScriptableObject{T}"/> from a given file path.
        /// </summary>
        /// <param name="path">The path of the asset.</param>
        /// <returns>Returns the loaded instance, if found.</returns>
        private static T LoadInstanceFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            bool hasAssetFileType = path.EndsWith(Literals.AssetFileType);

            // Sanitize out the resource path.
            int resourceIndex = path.IndexOf(Literals.ResourcesDirectory, StringComparison.Ordinal);
            string resourcePath = resourceIndex < 0 ? path : path.Substring(resourceIndex + Literals.ResourcesDirectory.Length);

            if (hasAssetFileType)
                resourcePath = resourcePath.Remove(resourcePath.Length - Literals.AssetFileType.Length);

            T foundInstance = Resources.Load<T>(resourcePath);

#if UNITY_EDITOR
            if (!foundInstance)
            {
                if (!hasAssetFileType)
                    path += Literals.AssetFileType;

                foundInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
#endif

            return foundInstance;
        }

#if UNITY_EDITOR
        /// <summary>
        /// A special event for checking the play mode of the editor. Used to destroy runtime objects.
        /// </summary>
        /// <param name="state">The current play mode state.</param>
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (!_wasCreatedAtRuntime || state != UnityEditor.PlayModeStateChange.EnteredEditMode)
                return;

            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            DestroyImmediate(this);
        }
#endif
    }

    /// <summary>
    /// A <see cref="SingletonScriptableObject{T}"/> with public access.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="PublicSingletonScriptableObject{T}"/> class.</typeparam>
    public abstract class PublicSingletonScriptableObject<T> : SingletonScriptableObject<T> where T : PublicSingletonScriptableObject<T>
    {
        /// <summary>
        /// Gets the current instance of the <see cref="PublicSingletonScriptableObject{T}"/>.
        /// </summary>
        /// <typeparam name="TSingleton">The type of the <see cref="PublicSingletonScriptableObject{T}"/>.</typeparam>
        /// <returns>Returns the <see cref="SingletonScriptableObject{T}.CurrentSingleton"/>.</returns>
        public static TSingleton Get<TSingleton>() where TSingleton : T
        {
            return (TSingleton)CurrentSingleton;
        }
    }
}
#endif