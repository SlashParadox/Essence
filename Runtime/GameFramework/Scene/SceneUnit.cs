// Copyright (c) Craig Williams, SlashParadox

using SlashParadox.Essence.Kits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Load - Adds scenes additively
// Transit - Transitions scene group, but does not recreate game mode or controllers. Takes preservation tags.
// Travel - Fully transitions scene group, with a new game mode. Takes preservation tags.

namespace SlashParadox.Essence
{
    public static partial class EssenceLog
    {
        public static readonly Logger LogScene = new Logger(Debug.unityLogger.logHandler);
    }
}

namespace SlashParadox.Essence.GameFramework
{
    /// <summary>
    /// A game singleton unit for loading and transitioning between scenes.
    /// </summary>
    /// <remarks>
    /// This unit works on the principle of different types of load behaviours.<br/>
    /// 1. Load: Scenes are loaded additively to the current scenes.<br/>
    /// 2. Transition: The old scenes are all unloaded, and new scenes are displayed. All game mode and controller objects persist.<br/>
    /// 3. Travel: Similar to Transition, except a new game mode is loaded. Only items with the right preservation tags persist.<br/>
    /// </remarks>
    public class SceneUnit : SingletonBehaviour<SceneUnit>
    {
        /// <summary>
        /// A representation of a state of scene traveling. Bind to the static states to be notified of certain processes.
        /// </summary>
        public sealed class TravelState
        {
            public delegate void TravelStateDelegate(TravelState state);

            public static readonly TravelState Initialization = new TravelState(0, nameof(Initialization));
            public static readonly TravelState LoadMainScene = new TravelState(1, nameof(LoadMainScene));
            public static readonly TravelState LoadExtraScenes = new TravelState(2, nameof(LoadExtraScenes));
            public static readonly TravelState LoadHolders = new TravelState(3, nameof(LoadHolders));
            public static readonly TravelState PrepareFramework = new TravelState(4, nameof(PrepareFramework));
            public static readonly TravelState TravelFinishHolders = new TravelState(5, nameof(TravelFinishHolders));
            public static readonly TravelState TravelComplete = new TravelState(6, nameof(TravelComplete));
            public static readonly TravelState TravelFailed = new TravelState(7, nameof(TravelFailed));

            /// <summary>The maximum level. Use to know how many <see cref="TravelState"/>s there are.</summary>
            public static int MaxLevel { get; private set; }

            /// <summary>An event called at the start of the state.</summary>
            public event TravelStateDelegate OnStart;

            /// <summary>An event called at the end of the state.</summary>
            public event TravelStateDelegate OnEnd;

            /// <summary>The name of the <see cref="TravelState"/>.</summary>
            public readonly string StateName;

            /// <summary>The level of the <see cref="TravelState"/>. This helps indicate how far along the process is.</summary>
            public readonly int Level;

            private TravelState(int inLevel, string inName)
            {
                StateName = inName;
                Level = inLevel;

                if (MaxLevel < Level)
                    MaxLevel = Level;
            }

            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || (obj is TravelState other && Equals(other));
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(StateName, Level);
            }

            public override string ToString()
            {
                return StateName;
            }

            private bool Equals(TravelState other)
            {
                return StateName == other.StateName && Level == other.Level;
            }

            internal void InvokeStart()
            {
                CurrentTravelState = this;
                OnStart?.Invoke(this);
            }

            internal void InvokeEnd()
            {
                OnEnd?.Invoke(this);
            }

            public static bool operator >(TravelState a, TravelState b)
            {
                if (a == null)
                    return false;

                return b == null || a.Level > b.Level;
            }

            public static bool operator <(TravelState a, TravelState b)
            {
                if (a == null)
                    return false;

                return b != null && a.Level < b.Level;
            }

            public static bool operator >=(TravelState a, TravelState b)
            {
                if (a == null)
                    return b == null;

                return b == null || a.Level >= b.Level;
            }

            public static bool operator <=(TravelState a, TravelState b)
            {
                if (a == null)
                    return true;

                return b != null && a.Level <= b.Level;
            }

            public static bool operator ==(TravelState a, TravelState b)
            {
                if (ReferenceEquals(a, null))
                    return ReferenceEquals(b, null);

                return !ReferenceEquals(b, null) && a.Level == b.Level;
            }

            public static bool operator !=(TravelState a, TravelState b)
            {
                return !(a == b);
            }
        }

        /// <summary>The currently active <see cref="TravelState"/>.</summary>
        public static TravelState CurrentTravelState { get; private set; }

        /// <summary>Scenes that were asked to be loaded during travel.</summary>
        private readonly HashSet<string> _travelAdditiveScenes = new HashSet<string>();

        /// <summary>The primary scene to load during travel.</summary>
        private string _travelTargetScene;

        /// <summary>
        /// Begins travel to a new scene. This indicates a complete wipe of current game mode
        /// </summary>
        /// <param name="inTargetScene"></param>
        public static void BeginTravel(string inTargetScene)
        {
            if (CurrentSingleton)
                CurrentSingleton.OnBeginTravel(inTargetScene);
            else
                LogKit.ThrowNullIf(false, $"There is no active {nameof(SceneUnit)}! Cannot begin traveling to scene {inTargetScene}!");
        }

        /// <summary>
        /// Checks if the game is currently traveling to a new scene.
        /// </summary>
        /// <returns>Returns if traveling to a new scene.</returns>
        public static bool IsTraveling()
        {
            return CurrentTravelState != null;
        }

        /// <summary>
        /// Appends scenes to load additively after the <see cref="_travelTargetScene"/> finishes loading. Only callable between beginning travel and just after the initial scene loads.
        /// </summary>
        /// <param name="inScenes">The scenes to add.</param>
        /// <returns>Returns if the scenes were appended.</returns>
        public static bool AddTravelScenes(ICollection<string> inScenes)
        {
            if (inScenes.IsEmptyOrNull() || !CurrentSingleton || !IsTraveling() || CurrentTravelState.Level >= TravelState.LoadExtraScenes.Level)
                return false;

            foreach (string scene in inScenes)
            {
                CurrentSingleton._travelAdditiveScenes.Add(scene);
            }

            return true;
        }

        public static async Awaitable LoadAdditiveScenes(ICollection<string> scenes, Action completionEvent)
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
            Task<bool> result = LoadSceneGroupAsync(sceneGroupRef, loadParams.UpdateActiveScene);
            await result;

            // loadParams.onCompleted?.Invoke(result.Result);
            return result.Result;
        }

        public async Task<bool> LoadSceneGroupAsync(SceneGroup sceneGroup, SceneLoadParams loadParams)
        {
            Task<bool> result = LoadSceneGroupAsync(sceneGroup, loadParams.UpdateActiveScene);
            await result;

            //   loadParams.onCompleted?.Invoke(result.Result);
            return result.Result;
        }

        /// <summary>
        /// Begins traveling to a new scene.
        /// </summary>
        /// <param name="inTargetScene">The scene to travel to.</param>
        private async Awaitable OnBeginTravel(string inTargetScene)
        {
            if (IsTraveling())
            {
                LogKit.Log(EssenceLog.LogScene, LogType.Error, nameof(OnBeginTravel), $"Cannot travel to scene {inTargetScene}. Already traveling to scene {_travelTargetScene}!", this);
                return;
            }

            int sceneIndex = SceneUtility.GetBuildIndexByScenePath(inTargetScene);
            if (sceneIndex < 0)
            {
                LogKit.ThrowNullIf(false, $"Cannot begin travel to scene {inTargetScene}. The scene does not exist!");
                return;
            }

            if (destroyCancellationToken.IsCancellationRequested)
                return;

            _travelTargetScene = inTargetScene;

            TravelState.Initialization.InvokeStart();
            TravelState.Initialization.InvokeEnd();
            TravelState.LoadMainScene.InvokeStart();

            // Travel to the main scene.
            await SceneManager.LoadSceneAsync(_travelTargetScene, LoadSceneMode.Single);
            await Awaitable.NextFrameAsync(destroyCancellationToken);

            if (destroyCancellationToken.IsCancellationRequested)
            {
                CleanupTravel();
                return;
            }

            // Once the main scene is loaded, the game manager will handle creating game mode variables. After this, gather scene settings and load additive scenes.
            SceneSettings sceneSettings = GameManager.CurrentSingleton?.CurrentSceneSettings;
            if (sceneSettings)
                AddTravelScenes(sceneSettings.AdditiveScenes);

            TravelState.LoadMainScene.InvokeEnd();

            // Load any additive scenes that have been requested.
            TravelState.LoadExtraScenes.InvokeStart();
            await LoadAdditiveScenes(_travelAdditiveScenes, null);
            await Awaitable.NextFrameAsync(destroyCancellationToken);

            if (destroyCancellationToken.IsCancellationRequested)
            {
                CleanupTravel();
                return;
            }

            TravelState.LoadExtraScenes.InvokeEnd();
            TravelState.LoadHolders.InvokeStart();

            // TODO - Wait for any loading holders.

            if (destroyCancellationToken.IsCancellationRequested)
            {
                CleanupTravel();
                return;
            }

            TravelState.LoadHolders.InvokeEnd();

            TravelState.PrepareFramework.InvokeStart();
            TravelState.PrepareFramework.InvokeEnd();

            TravelState.TravelFinishHolders.InvokeStart();

            // TODO - Wait for any travel holders

            if (destroyCancellationToken.IsCancellationRequested)
            {
                CleanupTravel();
                return;
            }

            TravelState.TravelFinishHolders.InvokeEnd();

            TravelState.TravelComplete.InvokeStart();
            CleanupTravel();
            TravelState.TravelComplete.InvokeEnd();
        }

        /// <summary>
        /// Cleans up a scene travel at the end.
        /// </summary>
        private void CleanupTravel()
        {
            // Failure state.
            bool bFailed = CurrentTravelState != TravelState.TravelComplete;
            if (bFailed)
                LogKit.Log(EssenceLog.LogScene, LogType.Error, nameof(CleanupTravel), $"Failed to travel to scene [{_travelTargetScene}]!");
            
            CurrentTravelState = null;
            _travelTargetScene = null;
            _travelAdditiveScenes.Clear();
            
            if (bFailed)
            {
                TravelState.TravelFailed.InvokeStart();
                TravelState.TravelFailed.InvokeEnd();
            }
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