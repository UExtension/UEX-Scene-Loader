using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UExtension.SceneLoader
{
    public static class SceneLoader
    {
        public static bool Logging { get; set; } = true;

        // =========================    EVENTS    =========================
        // Individual scene events
        public static event Action<SceneContainer> OnSceneLoaded = delegate { };
        public static event Action<SceneContainer> OnSceneUnloaded = delegate { };

        // Group scene events
        public static readonly List<IBeforeSceneUnLoad> BeforeGroupSceneUnLoads = new();
        public static readonly List<IAfterSceneUnLoad> AfterGroupSceneUnLoads = new();
        public static readonly List<IBeforeSceneLoad> BeforeGroupSceneLoads = new();
        public static readonly List<IAfterSceneLoad> AfterGroupSceneLoads = new();

        // ================================================================
        private static readonly HashSet<SceneContainer> RegisteredSceneContainers = new();
        private static readonly Dictionary<int, SceneContainer> BuildIndexSceneContainers = new();
        private static readonly HashSet<Scene> LoadedScenes = new();

        /// <summary>
        /// A dictionary that maintains a collection of loaded <see cref="SceneContainer"/> instances
        /// and their respective load counts within the scene loading system.
        /// </summary>
        /// <remarks>
        /// This variable is used to track which scenes have been loaded and how many times each scene has been loaded.
        /// It is updated when scenes are loaded or unloaded with the <see cref="SceneLoader"/> class.
        /// </remarks>
        private static readonly Dictionary<SceneContainer, int> LoadedSceneContainers = new();

        /// <summary>
        /// Initializes and sets up event listeners for Unity's scene management events,
        /// and automatically loads scene groups marked for loading on application start.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneLoaded += HandleUnitySceneLoaded;
            SceneManager.sceneUnloaded += HandleUnitySceneUnloaded;
            SceneManager.activeSceneChanged += HandleActiveSceneChanged;

            if (Logging)
            {
                Debug.Log($"Initializing {nameof(SceneLoader)}...");
            }

            var firstScene = SceneManager.GetActiveScene();

            foreach (var container in RegisteredSceneContainers)
            {
                if (Logging)
                {
                    Debug.Log($"Registering {nameof(SceneContainer)} {container.Name}:{container.BuildIndex}");
                }

                if (container.LoadOnApplicationStart)
                {
                    // Skip if already loaded
                    if (container.BuildIndex == firstScene.buildIndex) continue;
                    LoadScene(container);
                }
            }
        }

        /// <summary>
        /// Registers a given scene container to be managed by the scene loader.
        /// </summary>
        /// <param name="container">The scene container to be registered.</param>
        public static void RegisterSceneContainer(SceneContainer container)
        {
            RegisteredSceneContainers.Add(container);
            BuildIndexSceneContainers[container.BuildIndex] = container;
        }

        /// <summary>
        /// Unregisters a given scene container from the list of managed scene containers.
        /// </summary>
        /// <param name="container">The scene container to be unregistered.</param>
        public static void UnregisterSceneContainer(SceneContainer container)
        {
            RegisteredSceneContainers.Remove(container);
            BuildIndexSceneContainers.Remove(container.BuildIndex);
        }

        #region SceneManagement

        /// <summary>
        /// Loads scenes asynchronously and manages the associated lifecycle events.
        /// </summary>
        /// <param name="scenesToLoad">A list of <see cref="SceneContainer"/> to load.</param>
        /// <returns>A task representing the asynchronous operation of loading scenes.</returns>
        public static async UniTask SetActiveSceneContainers(ICollection<SceneContainer> scenesToLoad, IProgress<float> loadProgress = null, IProgress<float> unloadProgress = null)
        {
            // FilteredSceneToLoad = SceneToLoad - LoadedScenes
            var filteredScenesToLoad = RemoveAll(scenesToLoad, LoadedSceneContainers.Keys.ToList());

            if (filteredScenesToLoad.Count > 0)
            {
                // Load group's scenes
                await LoadScenes(filteredScenesToLoad, loadProgress);

                // Wait for next frame to complete scene loading
                await UniTask.NextFrame();
            }

            // FilteredSceneToUnload = LoadedScenes - ScenesToLoad
            var filteredScenesToUnload = RemoveAll(LoadedSceneContainers.Keys.ToList(), scenesToLoad);

            if (filteredScenesToUnload.Count > 0)
            {
                // Unload unused scenes
                await UnloadScenes(filteredScenesToUnload, unloadProgress);

                // Wait for next frame to complete scene unloading
                await UniTask.NextFrame();
            }
        }

        /// <summary>
        /// Loads scenes asynchronously and manages the associated lifecycle events.
        /// </summary>
        /// <param name="scenesToLoad">A list of <see cref="SceneContainer"/> to load.</param>
        /// <returns>A task representing the asynchronous operation of loading scenes.</returns>
        private static async UniTask LoadScenes(List<SceneContainer> scenesToLoad, IProgress<float> progress = null)
        {
            await UniTask.WhenAll(BeforeGroupSceneLoads.Select(action => action.DoBeforeSceneLoad(scenesToLoad)));

            var operationGroup = new GroupProgress(scenesToLoad.Count, progress);
            await UniTask.WhenAll(scenesToLoad.Select(s => LoadScene(s, operationGroup)));

            await CheckInternalSceneProgression(scenesToLoad);

            await UniTask.WhenAll(AfterGroupSceneLoads.Select(action => action.DoAfterSceneLoad(scenesToLoad)));
        }

        /// <summary>
        /// Unloads scenes asynchronously and manages the associated lifecycle events.
        /// </summary>
        /// <param name="scenesToUnload">A list of <see cref="SceneContainer"/> to unload.</param>
        /// <returns>A task representing the asynchronous operation of unloading scenes.</returns>
        private static async UniTask UnloadScenes(List<SceneContainer> scenesToUnload, IProgress<float> progress = null)
        {
            await UniTask.WhenAll(BeforeGroupSceneUnLoads.Select(action => action.DoBeforeSceneUnLoad(scenesToUnload)));

            var operationGroup = new GroupProgress(scenesToUnload.Count, progress);
            await UniTask.WhenAll(scenesToUnload.Select(s => UnloadScene(s, operationGroup)));

            Resources.UnloadUnusedAssets();

            await UniTask.WhenAll(AfterGroupSceneUnLoads.Select(action => action.DoAfterSceneUnLoad(scenesToUnload)));
        }

        /// <summary>
        /// Loads a scene asynchronously and manages the associated lifecycle events.
        /// </summary>
        /// <param name="sceneContainer">The <see cref="SceneContainer"/> object containing details about the scene to load.</param>
        /// <returns>An <see cref="AsyncOperation"/> representing the asynchronous loading operation of the scene.</returns>
        private static UniTask LoadScene(SceneContainer sceneContainer, IProgress<AsyncOperation> progress = null)
        {
            var operation = SceneManager.LoadSceneAsync(sceneContainer.BuildIndex, LoadSceneMode.Additive);
            return progress == null
                ? operation.ToUniTask()
                : operation.ToUniTask(progress: Progress.CreateOnlyValueChanged<float>(_ => progress.Report(operation)));
        }

        /// <summary>
        /// Unloads a scene asynchronously and manages the associated lifecycle events.
        /// </summary>
        /// <param name="sceneContainer">The <see cref="SceneContainer"/> instance to unload.</param>
        /// <returns>An <see cref="AsyncOperation"/> representing the asynchronous unloading operation of the scene.</returns>
        private static UniTask UnloadScene(SceneContainer sceneContainer, IProgress<AsyncOperation> progress = null)
        {
            // Skip if only one scene is loaded
            if (LoadedScenes.Count <= 1)
            {
                Debug.LogWarning($"Cannot unload scene: {sceneContainer.Name}. Only one scene is loaded.");
                return UniTask.CompletedTask;
            }

            // Skip when DontDestroyOnUnload
            if (sceneContainer.DontDestroyOnUnload) return UniTask.CompletedTask;

            var operation = SceneManager.UnloadSceneAsync(sceneContainer.BuildIndex);
            return progress == null
                ? operation.ToUniTask()
                : operation.ToUniTask(progress: Progress.CreateOnlyValueChanged<float>(_ => progress.Report(operation)));
        }

        #endregion

        #region Accessors

        public static Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }

        public static bool SetActiveScene(SceneContainer container)
        {
            return SetActiveScene(container.Scene);
        }

        public static bool SetActiveScene(Scene scene)
        {
            return SceneManager.SetActiveScene(scene);
        }

        public static void SetBakingSet(SceneContainer container)
        {
            SetBakingSet(container.Scene);
        }

        public static void SetBakingSet(Scene scene)
        {
#if UNITY_6000
            ProbeReferenceVolume.instance.SetActiveScene(scene);
#endif
        }

        public static HashSet<Scene> GetLoadedScenes()
        {
            return new HashSet<Scene>(LoadedScenes);
        }

        /// <summary>
        /// A dictionary that maintains a collection of loaded <see cref="SceneContainer"/> instances
        /// and their respective load counts within the scene loading system.
        /// </summary>
        public static Dictionary<SceneContainer, int> GetLoadedSceneContainers()
        {
            return new Dictionary<SceneContainer, int>(LoadedSceneContainers);
        }

        #endregion

        #region Utils

        public static bool TryGetSceneContainer(Scene scene, out SceneContainer sceneContainer)
        {
            return BuildIndexSceneContainers.TryGetValue(scene.buildIndex, out sceneContainer);
        }

        private static List<T> RemoveAll<T>(ICollection<T> list, ICollection<T> toRemove)
        {
            var result = new List<T>(list);
            foreach (var element in toRemove)
            {
                result.Remove(element);
            }

            return result;
        }

        private static async UniTask CheckInternalSceneProgression(List<SceneContainer> scenesToLoad)
        {
            var scenes = scenesToLoad.Select(container => container.Scene).ToList();
            var progressions = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OfType<ISceneLoadProgression>()
                .ToList();

            // Wait for all progressions to reach 1
            await UniTask.WaitWhile(() => progressions.Any(NotReady));
            return;

            bool NotReady(ISceneLoadProgression progression)
            {
                var mono = progression as MonoBehaviour;
                return scenes.Contains(mono.gameObject.scene) && progression.Progress() < 1;
            }
        }

        #endregion


        private static void HandleActiveSceneChanged(Scene replacedScene, Scene activeScene)
        {
            if (Logging)
            {
                Debug.Log($"Active {nameof(Scene)} changed {replacedScene.name} => {activeScene.name}");
            }
        }

        private static void HandleUnitySceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Logging)
            {
                Debug.Log($"{nameof(Scene)} {scene.name} loaded");
            }

            LoadedScenes.Add(scene);

            if (TryGetSceneContainer(scene, out var sceneContainer))
            {
                if (LoadedSceneContainers.TryGetValue(sceneContainer, out var count))
                {
                    LoadedSceneContainers[sceneContainer] = count + 1;
                }
                else
                {
                    LoadedSceneContainers.Add(sceneContainer, 1);
                }

                OnSceneLoaded.Invoke(sceneContainer);
            }
            else
            {
                Debug.LogWarning($"Scene: {scene.name} has no {nameof(SceneContainer)} registered.");
            }
        }

        private static void HandleUnitySceneUnloaded(Scene scene)
        {
            if (Logging)
            {
                Debug.Log($"{nameof(Scene)} {scene.name} unloaded");
            }

            LoadedScenes.Remove(scene);

            if (TryGetSceneContainer(scene, out var sceneContainer))
            {
                if (LoadedSceneContainers.TryGetValue(sceneContainer, out var count))
                {
                    if (count == 1) LoadedSceneContainers.Remove(sceneContainer);
                    else LoadedSceneContainers[sceneContainer] = count - 1;
                }

                OnSceneUnloaded.Invoke(sceneContainer);
            }
            else
            {
                Debug.LogWarning($"Scene: {scene.name} has no {nameof(SceneContainer)} registered.");
            }
        }
    }
}