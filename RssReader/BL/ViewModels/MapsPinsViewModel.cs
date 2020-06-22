using RssReader.DAL.DataServices;
using RssReader.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace RssReader.BL.ViewModels
{
    public class MapsPinsViewModel : BaseViewModel
    {
        public ObservableCollection<Polyline> Polylines
        {
            get => Get<ObservableCollection<Polyline>>();
            set => Set(value);
        }

        public MapsPinsViewModel()
        {
            Polylines = new ObservableCollection<Polyline>();
        }

        protected override async Task LoadDataAsync()
        {
            var result = await DataServices.Map.GetPolylines(CancellationToken);

            var first = result.Data.trkseg.trkpt.First();
            var initPositions = new Position(first.lat, first.lon);
            MessageBus.SendMessage("initPositions", initPositions);

            var positions = result.Data.trkseg.trkpt.Select(x => new Position(x.lat, x.lon)).ToList();

            var portion = 1000;
            var count = positions.Count();
            var added = 0;
            while (count > added)
            {
                var polyline = new Polyline
                {
                    StrokeColor = Color.Blue,
                    StrokeWidth = 2
                };

                var left = count - added;
                var take = Math.Min(portion, left);

                for (var i = added; i < added + take; i++)
                {
                    polyline.Positions.Add(positions.ElementAt(i));
                }

                added += take;
                left -= take;

                if (left > 0)
                {
                    var seamPosition = positions.ElementAt(added);
                    polyline.Positions.Add(seamPosition);

                    if (left == 1)
                    {
                        added += 1;
                    }
                }

                Polylines.Add(polyline);
            }

            MessageBus.SendMessage("isPolylinesReady");
        }
    }
}
