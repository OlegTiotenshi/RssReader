using System.Collections.ObjectModel;
using Xamarin.Forms.GoogleMaps;

namespace RssReader.UI.Interfaces
{
    public interface IDrawPolylines
    {
        void Draw(Map map, ObservableCollection<Polyline> polylines);
    }
}
