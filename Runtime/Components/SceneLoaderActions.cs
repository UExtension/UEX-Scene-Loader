using System.Collections.Generic;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;

namespace UExtension.SceneLoader.Components
{
    public class SceneLoaderActions : ScriptableObject
    {
        [field: SerializeField, HideInInspector]
        public bool Logging { get; private set; } = true;

        public void SetLogging(bool value)
        {
            Logging = value;
            SceneLoader.Logging = value;
        }

        public void LoadScenes(List<SceneContainer> containers)
        {
            _ = SceneLoader.SetActiveSceneContainers(containers);
        }
    }
}