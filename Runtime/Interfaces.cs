using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UExtension.SceneLoader.ScriptableObjects;

namespace UExtension.SceneLoader
{
    public interface IBeforeSceneUnLoad
    {
        UniTask DoBeforeSceneUnLoad(List<SceneContainer> scenesToUnload, CancellationToken token = default);
    }

    public interface IAfterSceneUnLoad
    {
        UniTask DoAfterSceneUnLoad(List<SceneContainer> scenesUnloaded, CancellationToken token = default);
    }

    public interface IBeforeSceneLoad
    {
        UniTask DoBeforeSceneLoad(List<SceneContainer> scenesToLoad, CancellationToken token = default);
    }

    public interface IAfterSceneLoad
    {
        UniTask DoAfterSceneLoad(List<SceneContainer> scenesLoaded, CancellationToken token = default);
    }

    public interface ISceneLoadProgression
    {
        float Progress();
    }
}