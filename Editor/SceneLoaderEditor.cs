using System.Linq;
using UExtension.SceneLoader.Components;
using UnityEditor;
using UnityEngine;

namespace UExtension.SceneLoader.Editor
{
    public static class SceneLoaderEditor
    {
        [MenuItem("UExtension/SceneLoader/Toggle Logging")]
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

            Debug.Log($"{nameof(SceneLoader)} Logging: {(SceneLoader.Logging ? "ON" : "OFF")}");
        }
    }
}