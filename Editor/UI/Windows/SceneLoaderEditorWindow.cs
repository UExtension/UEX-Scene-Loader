using UExtension.Bootstrap.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UExtension.SceneLoader.Editor.UI
{
    public class SceneLoaderEditorWindow : EditorWindow, IUExtensionTabFactory
    {
        [field: SerializeField] public VisualTreeAsset VisualTreeAsset { get; private set; }
        public string Name => "Scene Loader";
        public int Order => 0;

        [MenuItem("UExtension/Scene Loader/Editor Window")]
        public static void ShowExample()
        {
            var wnd = GetWindow<SceneLoaderEditorWindow>();
            wnd.titleContent = new GUIContent(wnd.Name);
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            VisualTreeAsset.CloneTree(rootVisualElement);
        }
    }
}