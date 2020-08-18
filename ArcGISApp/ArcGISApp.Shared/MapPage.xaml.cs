using System.Collections.Generic;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Xamarin.Forms;

namespace ArcGISApp
{
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();

            mapView.GeoViewTapped += MapView_GeoViewTapped;
        }

        private async void MapView_GeoViewTapped(object sender, Esri.ArcGISRuntime.Xamarin.Forms.GeoViewInputEventArgs e)
        {
            // get the tap location in screen units
            var tapScreenPoint = e.Position;

            var pixelTolerance = 20;
            var returnPopupsOnly = false;
            var maxLayerResults = 12;

            // identify all layers in the MapView, passing the tap point, tolerance, types to return, and max results
            IReadOnlyList<IdentifyLayerResult> idLayerResults = await mapView.IdentifyLayersAsync(tapScreenPoint, pixelTolerance, returnPopupsOnly, maxLayerResults);

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

        // Map initialization logic is contained in MapViewModel.cs
    }
}
