using System.Collections.Generic;
using UExtension.Bootstrap;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;

namespace UExtension.SceneLoader.Components
{
    public class SceneLoaderActions : ScriptableObject, IBootstrapService
    {
        [field: SerializeField, HideInInspector]
        public bool Logging { get; private set; } = true;

        [field: SerializeField, HideInInspector]
        public bool EnableLoadOnApplicationStart { get; private set; } = true;

        public void SetLogging(bool value)
        {
            Logging = value;
            SceneLoader.Logging = value;
        }

        public void SetEnableLoadOnApplicationStart(bool value)
        {
            EnableLoadOnApplicationStart = value;
        }

        public void LoadScenes(List<SceneContainer> containers)
        {
            _ = SceneLoader.SetActiveSceneContainers(containers);
        }

        public void Bootstrap()
        {
            SceneLoader.EnableLoadOnApplicationStart = EnableLoadOnApplicationStart;
            SceneLoader.Logging = Logging;
        }

        public int Priority => 0;
    }
}