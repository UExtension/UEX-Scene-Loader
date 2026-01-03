using UExtension.SceneLoader.ScriptableObjects;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace UExtension.SceneLoader.Editor.UI.Atoms
{
    public class SceneContainerAtom : VisualElement
    {
        private SceneContainer _sceneContainer;

        public SceneContainer SceneContainer
        {
            get => _sceneContainer;
            set
            {
                _sceneContainer = value;
                if (NameLabel != null) NameLabel.text = value.name;
            }
        }

        public Label NameLabel { get; }

        public SceneContainerAtom()
        {
            AddToClassList("scene-container");
            NameLabel = new Label();
            Add(NameLabel);

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("scene-container__buttons");

            var loadButton = new Button
            {
                text = "Load"
            };
            loadButton.RegisterCallback<ClickEvent>(HandleLoadRoute);
            loadButton.AddToClassList("route-factory__load-button");
            buttonContainer.Add(loadButton);

            Add(buttonContainer);
        }

        public SceneContainerAtom(SceneContainer sceneContainer) : this()
        {
            SceneContainer = sceneContainer;
        }

        private void HandleLoadRoute(ClickEvent evt)
        {
            EditorSceneManager.OpenScene(SceneContainer.Path);
        }
    }
}