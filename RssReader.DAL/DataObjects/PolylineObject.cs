using System.Collections.Generic;

namespace RssReader.DAL.DataObjects
{
    public class PolylineObject
    {
        public Trkseg trkseg { get; set; }
    }

    public class Trkpt
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Trkseg
    {
        public IList<Trkpt> trkpt { get; set; }
    }
}
