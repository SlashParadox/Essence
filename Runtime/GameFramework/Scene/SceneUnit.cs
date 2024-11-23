// Copyright (c) Craig Williams, SlashParadox

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlashParadox.Essence.Kits;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Load - Adds scenes additively
// Transit - Transitions scene group, but does not recreate game mode or controllers. Takes preservation tags.
// Travel - Fully transitions scene group, with a new game mode. Takes preservation tags.

namespace SlashParadox.Essence.GameUnits
{
    public class SceneLoadParams
    {
        public bool setFirstSceneAsActive;
        
        public Action<bool> onCompleted;
    }


    /// <summary>
    /// A game singleton unit for loading and transitioning between scenes.
    /// </summary>
    /// <remarks>
    /// This unit works on the principle of different types of load behaviours.<br/>
    /// 1. Load: Scenes are loaded additively to the current scenes.<br/>
    /// 2. Transition: The old scenes are all unloaded, and new scenes are displayed. All game mode and controller objects persist.<br/>
    /// 3. Travel: Similar to Transition, except a new game mode is loaded. Only items with the right preservation tags persist.<br/>
    /// </remarks>
    public class SceneUnit : PublicSingletonBehavior<SceneUnit>
    {
        public static async void LoadAdditiveScenes(IList<string> scenes, Action completionEvent)
        {
            foreach (string scene in scenes)
            {
                AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                await sceneLoad;
            }

            completionEvent?.Invoke();
        }
        
        public async Task<bool> LoadSceneGroupAsync(AssetReferenceT<SceneGroup> sceneGroupRef, SceneLoadParams loadParams)
        {
            Task<bool> result = LoadSceneGroupAsync(sceneGroupRef, loadParams.setFirstSceneAsActive);
            await result;

            loadParams.onCompleted?.Invoke(result.Result);
            return result.Result;
        }

        public async Task<bool> LoadSceneGroupAsync(SceneGroup sceneGroup, SceneLoadParams loadParams)
        {
            Task<bool> result = LoadSceneGroupAsync(sceneGroup, loadParams.setFirstSceneAsActive);
            await result;

            loadParams.onCompleted?.Invoke(result.Result);
            return result.Result;
        }

        private async Task<bool> LoadSceneGroupAsync(AssetReferenceT<SceneGroup> sceneGroupRef, bool setFirstSceneAsActive)
        {
            if (sceneGroupRef == null || this.ShouldCancelTasks())
                return false;

            AsyncOperationHandle<SceneGroup> handle = sceneGroupRef.LoadAssetAsync<SceneGroup>();
            await handle.Task;

            SceneGroup group = handle.Result;
            if (group == null || this.ShouldCancelTasks())
                return false;

            Task<bool> result = LoadSceneGroupAsync(group, setFirstSceneAsActive);
            await result;
            return result.Result;
        }

        private async Task<bool> LoadSceneGroupAsync(SceneGroup sceneGroup, bool setFirstSceneAsActive)
        {
            if (sceneGroup == null || this.ShouldCancelTasks())
                return false;

            string[] scenes = sceneGroup.CopyScenePaths();
            if (scenes.IsEmptyOrNull())
            {
                LogKit.Log(Loggers.LogScene, LogType.Error, nameof(LoadSceneGroupAsync), $"Cannot load any scenes from {nameof(SceneGroup)} [{sceneGroup}]! No scenes found!");
                return false;
            }

            Task<bool> pathResult = LoadScenePathsAsync(scenes, setFirstSceneAsActive);
            await pathResult;
            return pathResult.Result;
        }

        private async Task<bool> LoadScenePathsAsync(ICollection<string> scenes, bool setFirstSceneAsActive)
        {
            if (scenes.IsEmptyOrNull() || this.ShouldCancelTasks())
                return false;
            
            bool bSuccess = true;
            Scene? firstLoadedScene = null;
            foreach (string scene in scenes)
            {
                if (scene == null)
                    continue;

                if (this.ShouldCancelTasks())
                {
                    LogKit.Log(Loggers.LogScene, LogType.Warning, nameof(LoadScenePathsAsync), $"Scene loading canceled! Stopped loading scene [{scene}].");
                    bSuccess = false;
                    break;
                }

                Task<Scene> loadTask = LoadSceneAsync(scene);
                await loadTask;
                
                if (!loadTask.Result.IsValid())
                    continue;

                firstLoadedScene ??= loadTask.Result;
            }

            if (!setFirstSceneAsActive || firstLoadedScene == null)
                return bSuccess;
            
            SceneManager.SetActiveScene(firstLoadedScene.Value);
            LogKit.Log(Loggers.LogScene, nameof(LoadScenePathsAsync), $"[{firstLoadedScene.Value.name}] set as active scene.");

            return bSuccess;
        }
        
        /// <summary>
        /// Loads a single path to a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        /// <returns>Returns the loaded <see cref="Scene"/>. This is empty if nothing loaded.</returns>
        private async Task<Scene> LoadSceneAsync(string scene)
        {
            if (string.IsNullOrEmpty(scene))
            {
                LogKit.Log(Loggers.LogScene, LogType.Error, nameof(LoadSceneAsync), "Cannot load null scene!");
                return new Scene();
            }
            
            AsyncOperation handle = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            await handle;
            
            Scene loadedScene = SceneManager.GetSceneByPath(scene);
            if (loadedScene.IsValid())
            {
                LogKit.Log(Loggers.LogScene, nameof(LoadSceneAsync), $"Scene Path [{scene}] load successfully as Scene [{loadedScene.name}].");
                return loadedScene;
            }
            
            LogKit.Log(Loggers.LogScene, LogType.Warning, nameof(LoadSceneAsync), $"Scene Path [{scene}] failed to load.");
            return new Scene();
        }

#if ESSENCE_ADDRESSABLES
        /// <summary>
        /// Asynchronously Loads a set of scenes through addressable references.
        /// </summary>
        /// <param name="scenes">The scenes to load.</param>
        /// <param name="setFirstSceneAsActive">If true, the first scene loaded is set as the active scene.</param>
        /// <returns>Returns the results of the load.</returns>
        private async Task<bool> LoadSceneAssetReferencesAsync(AssetReference[] scenes, bool setFirstSceneAsActive)
        {
            if (scenes.IsEmptyOrNull() || this.ShouldCancelTasks())
                return false;

            bool bSuccess = true;
            Scene? firstLoadedScene = null;
            foreach (AssetReference scene in scenes)
            {
                if (this.ShouldCancelTasks())
                {
                    LogKit.Log(Loggers.LogScene, LogType.Warning, nameof(LoadSceneAssetReferencesAsync), $"Scene loading canceled! Stopped loading scene [{scene}].");
                    bSuccess = false;
                    break;
                }

                Task<SceneInstance> loadTask = LoadSceneAssetReferenceAsync(scene);
                await loadTask;
                
                if (!loadTask.Result.Scene.IsValid())
                    continue;

                firstLoadedScene ??= loadTask.Result.Scene;
            }

            if (!setFirstSceneAsActive || firstLoadedScene == null)
                return bSuccess;
            
            SceneManager.SetActiveScene(firstLoadedScene.Value);
            LogKit.Log(Loggers.LogScene, nameof(LoadSceneAssetReferencesAsync), $"[{firstLoadedScene.Value.name}] set as active scene.");

            return bSuccess;
        }

        /// <summary>
        /// Loads a single <see cref="AssetReference"/> to a <see cref="Scene"/>.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        /// <returns>Returns the loaded <see cref="SceneInstance"/>. This is empty if nothing loaded.</returns>
        private async Task<SceneInstance> LoadSceneAssetReferenceAsync(AssetReference scene)
        {
            if (scene == null)
            {
                LogKit.Log(Loggers.LogScene, LogType.Error, nameof(LoadSceneAssetReferenceAsync), "Cannot load null scene!");
                return new SceneInstance();
            }
            
            AsyncOperationHandle<SceneInstance> handle = scene.LoadSceneAsync(LoadSceneMode.Additive);
            if (!handle.IsValid())
            {
                LogKit.Log(Loggers.LogScene, LogType.Warning, nameof(LoadSceneAssetReferencesAsync), $"{nameof(AssetReference)} [{scene}] failed to load. It may not be a scene.");
                return new SceneInstance();
            }

            await handle.Task;
            if (handle.Result.Scene.IsValid())
            {
                LogKit.Log(Loggers.LogScene, nameof(LoadSceneAssetReferencesAsync), $"{nameof(AssetReference)} [{scene}] load successfully as Scene [{handle.Result.Scene.name}].");
                return handle.Result;
            }
            
            LogKit.Log(Loggers.LogScene, LogType.Warning, nameof(LoadSceneAssetReferencesAsync), $"{nameof(AssetReference)} [{scene}] failed to load.");
            return new SceneInstance();
        }
#endif
    }
}