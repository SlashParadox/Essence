// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlashParadox.Essence
{
    [DisallowMultipleComponent]
    public class SceneSettings : EssenceBehaviour
    {
        [SerializeField] private SceneData sceneData;

        [SerializeField] [SceneReference] private string[] additiveScenes;
        
        public SceneData Data { get { return sceneData; } }
        
        public string[] AdditiveScenes { get { return additiveScenes; } }

#if UNITY_EDITOR
        [MenuItem("GameObject/Essence/Scene/Scene Settings", false, 10)]
        private static void CreateSceneSettings(MenuCommand menuCommand)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneSettings existingSettings = SceneKit.GetFirstComponentInScene<SceneSettings>(scene, true);
            if (existingSettings)
            {
                Debug.LogWarning($"Could not create new {nameof(SceneSettings)} for Scene {scene.name}, as one was already found on {existingSettings.gameObject.name}.");
                return;
            }

            GameObject settings = new GameObject("SceneSettings");
            settings.AddComponent<SceneSettings>();
            
            GameObjectUtility.SetParentAndAlign(settings, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(settings, $"Create {settings.name}");
            Selection.activeObject = settings;
        }
#endif
    }
}