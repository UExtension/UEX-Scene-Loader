using System.Collections.Generic;
using System.Linq;
using UExtension.SceneLoader.Editor.UI.Atoms;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UExtension.SceneLoader.Editor.UI.Views
{
    [UxmlElement]
    public partial class ScenesListView : ListView
    {
        public ScenesListView()
        {
            AddToClassList("scenes-list-view");
            makeItem = () =>
            {
                var routeContainer = new VisualElement();
                routeContainer.AddToClassList("scenes-list-view__item");
                var route = new SceneContainerAtom();
                routeContainer.Add(route);
                return routeContainer;
            };
            bindItem = (element, i) => { ((SceneContainerAtom)element[0]).SceneContainer = ((List<SceneContainer>)itemsSource)[i]; };
            showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            selectionChanged += HandleRouteFactorySelected;
        }


        private void HandleRouteFactorySelected(IEnumerable<object> selection)
        {
            var selectedObject = selection.FirstOrDefault() as Object;
            if (selectedObject != null)
            {
                Selection.activeObject = selectedObject;

                EditorGUIUtility.PingObject(selectedObject);
            }
        }
    }
}