// Copyright (c) Craig Williams, SlashParadox

using System;
using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A very simple asset that contains a group of scenes that should be loaded together.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSceneData", menuName = "Essence/Scene/Scene Group")]
    public class SceneGroup : EssenceScriptableObject
    {
        /// <summary>Scenes to load together. The first scene path is set as the active scene, if requested.</summary>
        [SerializeField] [SceneReference] private string[] scenePaths;

        /// <summary>
        /// Checks if this group has any scenes. This does not validate the paths.
        /// </summary>
        /// <returns>Returns if there are any paths.</returns>
        public bool HasPaths()
        {
            return scenePaths.IsNotEmptyOrNull();
        }

        /// <summary>
        /// Checks if this group has any valid scenes.
        /// </summary>
        /// <returns>Returns if there are any paths.</returns>
        public bool HasValidPaths()
        {
            if (!HasPaths())
                return false;

            foreach (string path in scenePaths)
            {
                if (string.IsNullOrEmpty(path))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the number of scene paths.
        /// </summary>
        /// <returns>Returns the scene path count.</returns>
        public int GetScenePathCount()
        {
            return scenePaths.IsEmptyOrNull() ? 0 : scenePaths.Length;
        }
        
        /// <summary>
        /// Copies all <see cref="scenePaths"/> to a new array.
        /// </summary>
        /// <returns>Returns the copied array.</returns>
        public string[] CopyScenePaths()
        {
            string[] outPaths = { };
            
            if (!scenePaths.IsEmptyOrNull())
                Array.Copy(scenePaths, outPaths, scenePaths.Length);
            
            return outPaths;
        }

        /// <summary>
        /// Gets the scene path at the given index.
        /// </summary>
        /// <param name="index">The index of the scene.</param>
        /// <returns>Returns the string scene path.</returns>
        public string GetScenePathAt(int index)
        {
            if (scenePaths.IsEmptyOrNull() || !scenePaths.IsValidIndex(index))
                return string.Empty;

            return scenePaths[index];
        }
    }
}