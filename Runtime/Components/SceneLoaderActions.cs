using System.Collections.Generic;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEngine;

namespace UExtension.SceneLoader.Components
{
    public class SceneLoaderActions : MonoBehaviour
    {
        public void LoadScenes(List<SceneContainer> containers)
        {
            _ = SceneLoader.SetActiveSceneContainers(containers);
        }
    }
}