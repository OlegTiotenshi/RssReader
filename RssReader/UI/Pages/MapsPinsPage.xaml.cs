using Refit;
using RssReader.BL.ViewModels;
using RssReader.Helpers;
using RssReader.UI.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace RssReader.UI.Pages
{
    public partial class MapsPinsPage : BasePage
    {
        public MapsPinsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<MessageBus, Position>(this, "initPositions", (obj, data) =>
            {
                InitPositions(data);
            });

            //MessagingCenter.Subscribe<MessageBus>(this, "isPolylinesReady", (obj) =>
            //{
            //    DrawPolylines();
            //});
        }

        private void Map_CameraIdled(object sender, CameraIdledEventArgs e)
        {
            var region = map.Region;

            var viewModel = (MapsPinsViewModel)BaseViewModel;

            var list = viewModel.Position.Where(p => (p.Longitude >= region.FarLeft.Longitude) &&
                                                        (p.Longitude <= region.FarRight.Longitude) &&
                                                        (p.Latitude >= region.NearLeft.Latitude) &&
                                                        (p.Latitude <= region.FarRight.Latitude));

            var setToRemove = new HashSet<Position>(list);
            viewModel.Position.RemoveAll(x => setToRemove.Contains(x));

            var polyline = new Polyline
            {
                StrokeColor = Color.Blue,
                StrokeWidth = 2
            };
            foreach (var item in setToRemove)
                polyline.Positions.Add(item);

            if(polyline.Positions.Count > 1)
                Device.BeginInvokeOnMainThread(() =>
                {
                    map.Polylines.Add(polyline);
                });
        }

        //private void DrawPolylines()
        //{
        //    var viewModel = (MapsPinsViewModel)BaseViewModel;

        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        foreach (var polyline in viewModel.Polylines)
        //        {
        //            map.Polylines.Add(polyline);
        //            await Task.Delay(50);
        //        }
        //    });

        //    DependencyService.Get<IDrawPolylines>().Draw(map, viewModel.Polylines);
        //}

        ~MapsPinsPage()
        {
            //MessagingCenter.Unsubscribe<MessageBus>(this, "isPolylinesReady");
            MessagingCenter.Unsubscribe<MessageBus, Position>(this, "initPositions");
        }

        void InitPositions(Position initPositions)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                map.InitialCameraUpdate = CameraUpdateFactory.NewPosition(initPositions);

                await map.AnimateCamera(CameraUpdateFactory.NewPosition(initPositions));
            });

            map.CameraIdled += Map_CameraIdled;
        }
    }
}