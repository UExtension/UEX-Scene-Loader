using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace UExtension.SceneLoader.Components
{
    public class SceneLoaderEvents : MonoBehaviour, IBeforeSceneLoad, IAfterSceneLoad, IBeforeSceneUnLoad, IAfterSceneUnLoad
    {
        [field: SerializeField]
        [field: Header("Events")]
        public UnityEvent<SceneContainer> OnSceneLoaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<SceneContainer> OnSceneUnloaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnBeforeGroupSceneLoaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnAfterGroupSceneLoaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnBeforeGroupSceneUnloaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnAfterGroupSceneUnloaded { get; private set; } = new();

        private void Awake()
        {
            SceneLoader.OnSceneLoaded += HandleSceneLoaded;
            SceneLoader.OnSceneUnloaded += HandleSceneUnloaded;
            SceneLoader.BeforeGroupSceneLoads.Add(this);
            SceneLoader.AfterGroupSceneLoads.Add(this);
            SceneLoader.BeforeGroupSceneUnLoads.Add(this);
            SceneLoader.AfterGroupSceneUnLoads.Add(this);
        }

        private void OnDestroy()
        {
            SceneLoader.OnSceneLoaded -= HandleSceneLoaded;
            SceneLoader.OnSceneUnloaded -= HandleSceneUnloaded;
            SceneLoader.BeforeGroupSceneLoads.Remove(this);
            SceneLoader.AfterGroupSceneLoads.Remove(this);
            SceneLoader.BeforeGroupSceneUnLoads.Remove(this);
            SceneLoader.AfterGroupSceneUnLoads.Remove(this);
        }

        private void HandleSceneLoaded(SceneContainer sceneContainer) => OnSceneLoaded.Invoke(sceneContainer);

        private void HandleSceneUnloaded(SceneContainer sceneContainer) => OnSceneUnloaded.Invoke(sceneContainer);

        public UniTask DoBeforeSceneLoad(List<SceneContainer> scenesToLoad, CancellationToken cancellationToken)
        {
            OnBeforeGroupSceneLoaded.Invoke(scenesToLoad);
            return UniTask.CompletedTask;
        }

        public UniTask DoAfterSceneLoad(List<SceneContainer> scenesLoaded, CancellationToken cancellationToken)
        {
            OnAfterGroupSceneLoaded.Invoke(scenesLoaded);
            return UniTask.CompletedTask;
        }

        public UniTask DoBeforeSceneUnLoad(List<SceneContainer> scenesToUnload, CancellationToken cancellationToken)
        {
            OnBeforeGroupSceneUnloaded.Invoke(scenesToUnload);
            return UniTask.CompletedTask;
        }

        public UniTask DoAfterSceneUnLoad(List<SceneContainer> scenesUnloaded, CancellationToken cancellationToken)
        {
            OnAfterGroupSceneUnloaded.Invoke(scenesUnloaded);
            return UniTask.CompletedTask;
        }
    }
}