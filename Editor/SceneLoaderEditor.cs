using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UExtension.SceneLoader.Editor
{
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/UExtension/Scenes/SceneLoader")]
        private static void CreateAsset()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(SceneLoader)}");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneLoader>(path);
                throw new Exception($"{nameof(SceneLoader)} already exists");
            }

            var asset = CreateInstance<SceneLoader>();
            ProjectWindowUtil.CreateAsset(asset, "SceneLoader.asset");

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("UExtension/SceneLoader/Toggle Logging")]
        public static void ToggleLogging()
        {
            var asset = AssetDatabase
                .FindAssets($"t:{nameof(SceneLoader)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneLoader>)
                .Single();

            asset.ActiveLogging = !asset.ActiveLogging;
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            Debug.Log($"{nameof(SceneLoader)} Logging: {(asset.ActiveLogging ? "ON" : "OFF")}");
        }
    }
}