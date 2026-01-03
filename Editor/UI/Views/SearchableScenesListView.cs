using System;
using System.Collections.Generic;
using System.Linq;
using UExtension.Bootstrap.UI;
using UExtension.SceneLoader.ScriptableObjects;
using UnityEditor;
using UnityEngine.UIElements;

namespace UExtension.SceneLoader.Editor.UI.Views
{
    [UxmlElement]
    public partial class SearchableScenesListView : VisualElement, IUExtensionTabFactory
    {
        private readonly ScenesListView listView;
        private readonly List<SceneContainer> routeFactories = new();
        private readonly List<SceneContainer> filteredFactories = new();

        public SearchableScenesListView()
        {
            AddToClassList("searchable-scenes-list-view");
            var searchField = CreateSearchField();
            Add(searchField);

            routeFactories.Clear();
            routeFactories.AddRange(FindSceneContainers());

            filteredFactories.Clear();
            filteredFactories.AddRange(routeFactories);

            listView = new ScenesListView
            {
                itemsSource = filteredFactories
            };
            Add(listView);
        }

        private TextField CreateSearchField()
        {
            TextField searchField = new();
            searchField.AddToClassList("searchable-scenes-list-view__search-field");
            searchField.textEdition.placeholder = "Search";
            searchField.RegisterValueChangedCallback(HandleSearchFieldChanged);

            return searchField;
        }

        private void HandleSearchFieldChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue.StartsWith(evt.previousValue, StringComparison.InvariantCultureIgnoreCase))
            {
                filteredFactories.RemoveAll(routeFactory =>
                    !routeFactory.Name.ToLower().Contains(evt.newValue.ToLower()));
            }
            else
            {
                filteredFactories.Clear();
                filteredFactories.AddRange(
                    routeFactories.Where(routeFactory => routeFactory.Name.ToLower().Contains(evt.newValue.ToLower()))
                );
            }

            listView.RefreshItems();
        }

        public static List<SceneContainer> FindSceneContainers()
        {
            var assets = new List<SceneContainer>();
            var guids = AssetDatabase.FindAssets($"t:{nameof(SceneContainer)}");

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<SceneContainer>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public int Order => 0;

        public Tab Create()
        {
            var tab = new Tab("Scenes");
            tab.AddToClassList("navigation-tab");
            tab.Add(new SearchableScenesListView());
            return tab;
        }
    }
}