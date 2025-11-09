using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UExtension.SceneLoader.ScriptableObjects;

namespace UExtension.SceneLoader
{
    public interface IBeforeSceneUnLoad
    {
        public UniTask DoBeforeSceneUnLoad(List<SceneContainer> scenesToUnload);
    }

    public interface IAfterSceneUnLoad
    {
        public UniTask DoAfterSceneUnLoad(List<SceneContainer> scenesUnloaded);
    }

    public interface IBeforeSceneLoad
    {
        public UniTask DoBeforeSceneLoad(List<SceneContainer> scenesToLoad);
    }

    public interface IAfterSceneLoad
    {
        public UniTask DoAfterSceneLoad(List<SceneContainer> scenesLoaded);
    }

    public interface ISceneLoadProgression
    {
        public float Progress();
    }
}