using RssReader.UI.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

[assembly: Xamarin.Forms.Dependency(typeof(RssReader.Droid.Interfaces.DrawPolylinesImplementation))]
namespace RssReader.Droid.Interfaces
{
    public class DrawPolylinesImplementation : IDrawPolylines
    {
        public void Draw(Map map, ObservableCollection<Xamarin.Forms.GoogleMaps.Polyline> polylines)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                foreach (var polyline in polylines)
                {
                    map.Polylines.Add(polyline);
                    await Task.Delay(50);
                }

                //var decodedPoints = PolyUtil.Decode(polylines.ToString());
                //PolylineOptions options = new PolylineOptions();
                //options.AddAll(decodedPoints);

                //map.Polylines.Add();
            });
        }
    }
}