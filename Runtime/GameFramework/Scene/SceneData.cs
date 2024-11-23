using UnityEngine;
using UnityEngine.Serialization;

namespace SlashParadox.Essence
{
    [CreateAssetMenu(fileName = "NewSceneData", menuName = "Essence/Scene/Scene Data")]
    public class SceneData : EssenceScriptableObject
    {
        [SerializeField] private GameMode gameModePrefab;
        [SerializeField] private PlayerController playerControllerPrefab;
        [SerializeField] private Controllable defaultControllablePrefab;
        
        internal GameMode GameModePrefab { get { return gameModePrefab; } }
        
        internal PlayerController PlayerControllerPrefab { get { return playerControllerPrefab; } }
        
        internal Controllable DefaultControllablePrefab { get { return defaultControllablePrefab; } }
    }
}
