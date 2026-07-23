using System;
using Eflatun.SceneReference;
using UExtension.Bootstrap;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UExtension.SceneLoader.ScriptableObjects
{
    [CreateAssetMenu(menuName = "UExtension/Scenes/Scene Container", fileName = "SceneContainer")]
    public class SceneContainer : ScriptableObject, IEquatable<SceneContainer>, IBootstrapService
    {
        [field: SerializeField]
        public SceneReference Reference { get; set; }

        [field: SerializeField]
        public bool LoadOnApplicationStart { get; private set; }

        [field: SerializeField]
        public bool DontDestroyOnUnload { get; internal set; }

        public string Name => Reference.Name;
        public string Path => Reference.Path;
        public int BuildIndex => Reference.BuildIndex;
        public Scene Scene => Reference.LoadedScene;

        public bool IsReady => Reference.State == SceneReferenceState.Regular;

        public void Bootstrap()
        {
            SceneLoader.RegisterSceneContainer(this);
        }

        public int Priority => 0;

        public bool Equals(SceneContainer other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return BuildIndex == other.BuildIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneContainer other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (IsReady ? BuildIndex.GetHashCode() : 0);
            }
        }
    }
}