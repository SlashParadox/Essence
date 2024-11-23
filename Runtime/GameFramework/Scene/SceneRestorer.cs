// Copyright (c) Craig Williams, SlashParadox

using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlashParadox.Essence
{
    /// <summary>
    /// A component that restores an object to its original scene when it is removed from a parent.
    /// This is extremely useful if you have additive scenes, and have to connect a player character to an object and back.
    /// </summary>
    public class SceneRestorer : EssenceBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private PreviewGatherer sceneRestorerPreview;
#endif

        /// <summary>If true, objects that were once marked to not destroy on scene load are marked again once they have a null parent.</summary>
        [SerializeField] private bool preserveDontDestroyOnLoad;

        /// <summary>The path of the original scene.</summary>
        [Preview] private string _originalScene;

        protected virtual void Awake()
        {
            _originalScene = gameObject.scene.path;
        }

        private void OnBeforeTransformParentChanged()
        {
            if (preserveDontDestroyOnLoad && gameObject.scene.name == Literals.DontDestroyOnLoadName)
                _originalScene = Literals.DontDestroyOnLoadName;
        }

        private void OnTransformParentChanged()
        {
            if (gameObject.transform.parent)
                return;

            if (_originalScene == Literals.DontDestroyOnLoadName)
            {
                DontDestroyOnLoad(gameObject);
                return;
            }
            
            // As a backup, say after a scene transition, if the original scene is invalid, move to the active scene instead.
            if (!SceneManager.GetSceneByPath(_originalScene).IsValid())
                _originalScene = SceneManager.GetActiveScene().path;

            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
    }
}