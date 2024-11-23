using System;
using UnityEngine;

namespace SlashParadox.Essence
{
    /// <summary>
    /// Parameters to go along with a <see cref="SceneLoadRequest"/> in the <see cref="SlashParadox.Essence.GameUnits.SceneUnit"/>.
    /// </summary>
    [Serializable]
    public struct SceneLoadParams
    {
        /// <summary>If true, all other scenes are unloaded. Overrides <see cref="UpdateActiveScene"/> as if it were true.</summary>
        public bool UnloadCurrentScenes;
        
        /// <summary>If true, the first scene in the <see cref="SceneLoadRequest.sceneList"/> will become the active scene.</summary>
        public bool UpdateActiveScene;

        public SceneLoadParams(bool unloadCurrentScenes, bool updateActiveScene)
        {
            UnloadCurrentScenes = unloadCurrentScenes;
            UpdateActiveScene = updateActiveScene;
        }
    }
    
    /// <summary>
    /// A request to load a group of scenes with the <see cref="SlashParadox.Essence.GameUnits.SceneUnit"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSceneRequest", menuName = "Essence/Scene/Scene Load Request")]
    public class SceneLoadRequest : EssenceScriptableObject
    {
        /// <summary>The list of scenes to load. If one is to become the active scene, it must be the first in the list.</summary>
        [SerializeField] [SceneReference] private string[] sceneList;

        /// <summary>Default <see cref="SceneLoadParams"/> to use if none are provided.</summary>
        [SerializeField] private SceneLoadParams defaultLoadParams;
        
        /// <summary>The list of scenes to load. If one is to become the active scene, it must be the first in the list.</summary>
        public string[] SceneList { get { return sceneList; } }
    }
}
