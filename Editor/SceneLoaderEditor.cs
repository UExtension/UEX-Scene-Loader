using System.Linq;
using UExtension.SceneLoader.Components;
using UnityEditor;
using UnityEngine;

namespace UExtension.SceneLoader.Editor
{
    public static class SceneLoaderEditor
    {
        [MenuItem("UExtension/Scene Loader/Toggle Logging")]
        public static void ToggleLogging()
        {
            var asset = AssetDatabase
                .FindAssets($"t:{nameof(SceneLoaderActions)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneLoaderActions>)
                .Single();

            asset.SetLogging(!asset.Logging);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            Debug.Log($"{nameof(SceneLoader)} Logging: {(asset.Logging ? "ON" : "OFF")}");
        }
        
        [MenuItem("UExtension/Scene Loader/Toggle LoadOnApplicationStart")]
        public static void ToggleLoadOnApplicationStart()
        {
            var asset = AssetDatabase
                .FindAssets($"t:{nameof(SceneLoaderActions)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneLoaderActions>)
                .Single();

            asset.SetEnableLoadOnApplicationStart(!asset.EnableLoadOnApplicationStart);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            Debug.Log($"{nameof(SceneLoader)} LoadOnApplicationStart: {(asset.EnableLoadOnApplicationStart ? "ON" : "OFF")}");
        }
    }
}