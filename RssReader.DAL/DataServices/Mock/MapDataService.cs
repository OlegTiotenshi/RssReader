using RssReader.DAL.DataObjects;
using System.Threading;
using System.Threading.Tasks;

namespace RssReader.DAL.DataServices.Mock
{
    public class MapDataService : BaseMockDataService, IMapDataService
    {
        public Task<RequestResult<PolylineObject>> GetPolylines(CancellationToken cts)
        {
            return GetMockData<PolylineObject>(@"RssReader.DAL.Resources.MockData.json");
        }
    }
}
