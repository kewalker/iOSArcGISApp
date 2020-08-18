using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using UIKit;

namespace iOSArcGISApp
{
    public partial class ViewController : UIViewController
    {
        MapViewModel _mapViewModel = new MapViewModel();
        MapView _mapView;

        public ViewController(IntPtr handle) : base(handle)
        {
            // Listen for changes on the view model
            _mapViewModel.PropertyChanged += MapViewModel_PropertyChanged;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create a new map view, set its map, and provide the coordinates for laying it out
            _mapView = new MapView()
            {
                Map = _mapViewModel.Map // Use the map from the view-model
            };

            _mapView.GeoViewTapped += _mapView_GeoViewTapped;

            // Add the MapView to the Subview
            View.AddSubview(_mapView);
        }

        private async void _mapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            // get the tap location in screen units
            var tapScreenPoint = e.Position;

            var pixelTolerance = 20;
            var returnPopupsOnly = false;
            var maxLayerResults = 12;

            // identify all layers in the MapView, passing the tap point, tolerance, types to return, and max results
            IReadOnlyList<IdentifyLayerResult> idLayerResults = await _mapView.IdentifyLayersAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxLayerResults);

            // iterate the results for each layer
            foreach (IdentifyLayerResult idResults in idLayerResults)
            {
                // get the layer identified and cast it to FeatureLayer
                FeatureLayer idLayer = idResults.LayerContent as FeatureLayer;

                // iterate each identified GeoElement in the results for this layer
                foreach (GeoElement idElement in idResults.GeoElements)
                {
                    // cast the result GeoElement to Feature
                    Feature idFeature = idElement as Feature;

                    // select this feature in the feature layer
                    idLayer.SelectFeature(idFeature);
                }
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            // Fill the screen with the map
            _mapView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);

            base.ViewDidLayoutSubviews();
        }

        private void MapViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update the map view with the view model's new map
            if (e.PropertyName == "Map" && _mapView != null)
                _mapView.Map = _mapViewModel.Map;
        }
    }
}