using RssReader.BL.ViewModels;
using RssReader.Helpers;
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

            MessagingCenter.Subscribe<MessageBus>(this, "isPolylinesReady", (obj) =>
             {
                 DrawPolylines();
             });
        }

        private void DrawPolylines()
        {
            var viewModel = (MapsPinsViewModel)BaseViewModel;

            Device.BeginInvokeOnMainThread(async() =>
            {
                foreach (var polyline in viewModel.Polylines)
                {
                    map.Polylines.Add(polyline);
                    await Task.Delay(50);
                }
            });
        }

        ~MapsPinsPage()
        {
            MessagingCenter.Unsubscribe<MessageBus>(this, "isPolylinesReady");
            MessagingCenter.Unsubscribe<MessageBus, Position>(this, "initPositions");
        }

        void InitPositions(Position initPositions)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                map.InitialCameraUpdate = CameraUpdateFactory.NewPosition(initPositions);

                await map.AnimateCamera(CameraUpdateFactory.NewPosition(initPositions));
            });
        }
    }
}