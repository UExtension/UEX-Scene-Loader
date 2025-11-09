using System.Collections.Generic;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace UExtension.SceneLoader.Components
{
    public class SceneLoaderEvents : MonoBehaviour
    {
        [field: SerializeField, Header("Events")]
        public UnityEvent<SceneContainer> OnSceneLoaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<SceneContainer> OnSceneUnloaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnGroupSceneLoaded { get; private set; } = new();

        [field: SerializeField]
        public UnityEvent<List<SceneContainer>> OnGroupSceneUnloaded { get; private set; } = new();

        private void Awake()
        {
            SceneLoader.OnSceneLoaded += HandleSceneLoaded;
            SceneLoader.OnSceneUnloaded += HandleSceneUnloaded;
            SceneLoader.OnGroupSceneLoaded += HandleGroupSceneLoaded;
            SceneLoader.OnGroupSceneUnloaded += HandleGroupSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneLoader.OnSceneLoaded -= HandleSceneLoaded;
            SceneLoader.OnSceneUnloaded -= HandleSceneUnloaded;
            SceneLoader.OnGroupSceneLoaded -= HandleGroupSceneLoaded;
            SceneLoader.OnGroupSceneUnloaded -= HandleGroupSceneUnloaded;
        }

        private void HandleSceneLoaded(SceneContainer sceneContainer)
        {
            OnSceneLoaded.Invoke(sceneContainer);
        }

        private void HandleSceneUnloaded(SceneContainer sceneContainer)
        {
            OnSceneUnloaded.Invoke(sceneContainer);
        }

        private void HandleGroupSceneUnloaded(List<SceneContainer> sceneContainers)
        {
            OnGroupSceneLoaded.Invoke(sceneContainers);
        }

        private void HandleGroupSceneLoaded(List<SceneContainer> sceneContainers)
        {
            OnGroupSceneUnloaded.Invoke(sceneContainers);
        }
    }
}