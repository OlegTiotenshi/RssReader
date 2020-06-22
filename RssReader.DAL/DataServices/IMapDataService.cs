using RssReader.DAL.DataObjects;
using System.Threading;
using System.Threading.Tasks;

namespace RssReader.DAL.DataServices
{
    public interface IMapDataService
    {
        Task<RequestResult<PolylineObject>> GetPolylines(CancellationToken cts);
    }
}
